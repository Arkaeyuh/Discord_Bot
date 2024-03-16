using Newtonsoft.Json;

namespace College_Hotline;

public class JsonReader
{
    public string Token { get; set; }
    
    public string Prefix { get; set; }
    
    public string Hostname { get; set; }
    
    public string Port { get; set; }
    
    public string Password { get; set; }

    public async Task ReadJson()
    {
        using (StreamReader sr = new StreamReader("config.json"))
        {
            string json = await sr.ReadToEndAsync();
            JsonStructure data = JsonConvert.DeserializeObject<JsonStructure>(json);

            this.Token = data.Token;
            this.Prefix = data.Prefix;
            this.Hostname = data.Hostname;
            this.Port = data.Port;
            this.Password = data.Password;
        }
    }
}

internal sealed class JsonStructure
{   
    public string Token { get; set; }
    
    public string Prefix { get; set; }
    
    public string Hostname { get; set; }
    
    public string Port { get; set; }
    
    public string Password { get; set; }
}
