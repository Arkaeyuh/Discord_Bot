using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;

namespace College_Hotline.Commands;

public class Music : BaseCommandModule

{
    [Command("join")]
    public async Task Join(CommandContext ctx)
    {
        var lava = ctx.Client.GetLavalink();

        var channel = ctx.Member?.VoiceState.Channel;
        
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.RespondAsync("Connection failed");
            return;
        }

        var node = lava.ConnectedNodes.Values.First();

        if (channel!.Type != ChannelType.Voice)
        {
            await ctx.RespondAsync("Failed to join");
            return;
        }

        await node.ConnectAsync(channel);
        await ctx.RespondAsync($"Joined {channel.Name}");
    }
    
    [Command("leave")]
    public async Task Leave(CommandContext ctx)
    {
        var lava = ctx.Client.GetLavalink();

        var channel = ctx.Member?.VoiceState.Channel;
        
        if (!lava.ConnectedNodes.Any())
        {
            await ctx.RespondAsync("Connection failed");
            return;
        }

        var node = lava.ConnectedNodes.Values.First();

        if (channel!.Type != ChannelType.Voice)
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
        if (ctx.Member!.VoiceState.Channel is null)
        {
            await ctx.RespondAsync("Please connect to a voice channel first");
            return;
        }
    
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
    
        if (conn is null)
        {
            await Join(ctx);
            conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
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

    [Command("playing")]
    public async Task Playing(CommandContext ctx)
    {
        var lava = ctx.Client.GetLavalink();
        var node = lava.ConnectedNodes.Values.First();
        var conn = node.GetGuildConnection(ctx.Member!.VoiceState.Guild);

        if (conn is null)
        {
            await ctx.RespondAsync("Bot is not currently connected to a voice channel");
            return;
        }

        if (conn.CurrentState.CurrentTrack is null)
        {
            await ctx.RespondAsync("Bot is not currently playing a track");
            return;
        }
        
        await ctx.RespondAsync($"Currently playing {conn.CurrentState.CurrentTrack.Title}");

    }

    [Command("pause")]
    public async Task Pause(CommandContext ctx)
    {
        if (ctx.Member!.VoiceState.Channel is null)
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

    [Command("stop")]
    public async Task Stop(CommandContext ctx)
    {
        await Pause(ctx);
        await Leave(ctx);
    }
}
