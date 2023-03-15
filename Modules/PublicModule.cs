using Discord;
using Discord.Commands;
using TextCommandFramework.Services;

namespace TextCommandFramework.Modules
{
	// Modules must be public and inherit from an IModuleBase
	public class PublicModule : ModuleBase<SocketCommandContext>
	{
		// Dependency Injection will fill this value in for us
		public PictureService PictureService { get; set; } = default!;

		[Command("ping", RunMode = RunMode.Async)]
		[Alias("pong", "hello")]
		public async Task PingAsync() =>
			await ReplyAsync("pong", messageReference: new MessageReference(Context.Message.Id));


		[Command("cat")]
		public async Task CatAsync()
		{
			// Get a stream containing an image of a cat
			var stream = await PictureService.GetCatPictureAsync();
			// Streams must be seeked to their beginning before being uploaded!
			stream.Seek(0, SeekOrigin.Begin);
			await Context.Channel.SendFileAsync(stream, "cat.png");
		}

		// Get info on a user, or the user who invoked the command if one is not specified
		[Command("userinfo")]
		public async Task UserInfoAsync(IUser user = null!)
		{
			user ??= Context.User;

			await ReplyAsync(user.ToString());
		}

		// Ban a user
		[Command("ban")]
		[RequireContext(ContextType.Guild)]
		[RequireUserPermission(GuildPermission.BanMembers)]
		[RequireBotPermission(GuildPermission.BanMembers)]
		public async Task BanUserAsync(IGuildUser user, [Remainder] string reason = null!)
		{
			await user.Guild.AddBanAsync(user, reason: reason);
			await ReplyAsync("ok!");
		}

		// [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
		[Command("echo")]
		public Task EchoAsync([Remainder] string text)
			// Insert a ZWSP before the text to prevent triggering other bots!
			=> ReplyAsync(text);

		// 'params' will parse space-separated elements into a list
		[Command("list")]
		public Task ListAsync(params string[] objects)
			=> ReplyAsync("You listed: " + string.Join("; ", objects));

		// Setting a custom ErrorMessage property will help clarify the precondition error
		[Command("guild_only")]
		[RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
		public Task GuildOnlyCommand()
			=> ReplyAsync("Nothing to see here!");
	}
}