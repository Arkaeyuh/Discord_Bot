using DSharpPlus;

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

            var discord = new DiscordClient(discordConfig);
            
            discord.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("ping"))
                    await e.Message.RespondAsync("pong!");
            };
            
            await discord.ConnectAsync();
            await Task.Delay(-1);
            

        }
    }
}
