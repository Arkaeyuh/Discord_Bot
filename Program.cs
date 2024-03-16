using College_Hotline.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;


namespace College_Hotline
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            var jsonReader = new JsonReader();
            await jsonReader.ReadJson();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            var endpoint = new ConnectionEndpoint
            { 
                Hostname = jsonReader.Hostname,
                Port = int.Parse(jsonReader.Port)
            };

            var lavalinkconfig = new LavalinkConfiguration
            {
                Password = jsonReader.Password,
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var discord = new DiscordClient(discordConfig);
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            var lavalink = discord.UseLavalink();
            
            commands.RegisterCommands<Music>();
            
            await discord.ConnectAsync();
            
            await lavalink.ConnectAsync(lavalinkconfig);
                
            await Task.Delay(-1);
            

        }
    }
}
