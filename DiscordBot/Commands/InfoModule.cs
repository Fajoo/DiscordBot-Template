using Discord.Commands;

namespace DiscordBot.Commands;

public class InfoModule : ModuleBase<SocketCommandContext>
{
	[Command("say")]
    [Discord.Commands.Summary("Echoes a message.")]
    public Task SayAsync([Remainder][Discord.Commands.Summary("The text to echo")] string echo)
        => ReplyAsync(echo);
}