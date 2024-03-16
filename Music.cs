using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace College_Hotline.Commands;

public class Music : BaseCommandModule

//rewrite using different lavalink
{
    [Command("join")]
    public async Task Join(CommandContext ctx, DiscordChannel channel)
    {
        var lava = ctx.Client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.RespondAsync("Connection failed");
            return;
        }

        var node = lava.ConnectedNodes.Values.First();

        if (channel.Type != ChannelType.Voice)
        {
            await ctx.RespondAsync("Cannot join a text channel.");
            return;
        }

        await node.ConnectAsync(channel);
        await ctx.RespondAsync($"Joined {channel.Name}");
    }
    
    [Command("leave")]
    public async Task Leave(CommandContext ctx, DiscordChannel channel)
    {
        var lava = ctx.Client.GetLavalink();
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.RespondAsync("Connection failed");
            return;
        }

        var node = lava.ConnectedNodes.Values.First();

        if (channel.Type != ChannelType.Voice)
        {
            await ctx.RespondAsync("Cannot leave a text channel.");
            return;
        }

        var conn = node.GetGuildConnection(channel.Guild);

        if (conn == null)
        {
            await ctx.RespondAsync("Not connected");
            return;
        }

        await conn.DisconnectAsync();
        await ctx.RespondAsync($"Left {channel.Name}");
    }

    [Command("play")]
    public async Task Play(CommandContext ctx, [RemainingText] string search)
    {
        if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        {
            await ctx.RespondAsync("Not connected to a voice channel");
            return;
        }
    
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
    
        if (conn == null)
        {
            await ctx.RespondAsync("Not connected");
            return;
        }
    
        var loadResult = await node.Rest.GetTracksAsync(search, LavalinkSearchType.SoundCloud);
    
        if (loadResult.LoadResultType is LavalinkLoadResultType.NoMatches or LavalinkLoadResultType.LoadFailed)
        {
            await ctx.RespondAsync($"No tracks found for {search}.");
            return;
        }
    
        var track = loadResult.Tracks.First();
    
        await conn.PlayAsync(track);
    
        await ctx.RespondAsync($"Now playing {track.Title}.");
    
    }

    [Command("pause")]
    public async Task Pause(CommandContext ctx)
    {
        if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
        {
            await ctx.RespondAsync("You are not in a voice .");
            return;
        }

        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

        if (conn == null)
        {
            await ctx.RespondAsync("Not currently connected to a channel.");
            return;
        }

        if (conn.CurrentState.CurrentTrack == null)
        {
            await ctx.RespondAsync("The bot is not currently playing music.");
            return;
        }

        await conn.PauseAsync();
    }
}
