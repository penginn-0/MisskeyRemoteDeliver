using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.Json;
    
{
   
        var cfg = LoadConfig();
        var Remote = new MisskeyRemoteDeliver.Misskey();
        var Local  = new MisskeyRemoteDeliver.Misskey();
        Remote.Setting(cfg.Remote_Token, cfg.Remote_Host);
        Local.Setting(cfg.Local_Token, cfg.Local_Host);

        var Counts = new List<Counts>();
    foreach (var id in cfg.UserIDs)
    {
        var Count = new Counts() { UserId = id, Count = 0 };
        var i = 0;
        var Request_R1 = new MisskeyRemoteDeliver.NotesRequest_Rootobject();
        Request_R1.limit = 100;
        Request_R1.userId = id;
        var Respone = Remote.Post("users/notes", System.Text.Json.JsonSerializer.Serialize(Request_R1));
        try
        {
            var Result = JsonConvert.DeserializeObject<dynamic>(Respone);
            if (Result is null)
            { continue; }
            string untilID = "";
            var Request_L = new MisskeyRemoteDeliver.APShowRequest_Rootobject();
            foreach (var obj in (Result as Newtonsoft.Json.Linq.JArray))
            {
                i++;
                Request_L.uri = $"https://{cfg.Remote_Host}/notes/{obj.Value<string>("id")}";
                Request_L.i = cfg.Local_Token;
                Respone = Local.Post("ap/show", System.Text.Json.JsonSerializer.Serialize(Request_L));
                Console.WriteLine("NoteId:" + obj.Value<string>("id") + $" / Count:{i}\r\n");
                untilID = obj.Value<string>("id");
                Task.Delay(cfg.Delay).Wait();
            }
            /**/
            if (untilID == "")
            { continue; }
            while (true)
            {
                var Request_R2 = new MisskeyRemoteDeliver.NotesRequest2_Rootobject();
                Request_R2.limit = 100;
                Request_R2.userId = id;
                Request_R2.untilId = untilID;
                Respone = Remote.Post("users/notes", System.Text.Json.JsonSerializer.Serialize(Request_R2));
                if (Respone == null) { break; }
                if (Respone == "[]") { break; }
                Result = JsonConvert.DeserializeObject<dynamic>(Respone);
                foreach (var obj in (Result as Newtonsoft.Json.Linq.JArray))
                {
                    i++;
                    Request_L.uri = $"https://{cfg.Remote_Host}/notes/{obj.Value<string>("id")}";
                    Request_L.i = cfg.Local_Token;
                    Respone = Local.Post("ap/show", System.Text.Json.JsonSerializer.Serialize(Request_L));
                    Console.WriteLine("NoteId:" + obj.Value<string>("id") + $" / Count:{i}\r\n");
                    untilID = obj.Value<string>("id");
                    Task.Delay(cfg.Delay).Wait();
                }
            }

            Count.Count = i;
            Counts.Add(Count);
        }
        catch (Exception e)
        {
            Count.Count = i;
            Counts.Add(Count);
            Console.WriteLine(e.Message);
            continue;
        }

    }
    
    foreach(var c in Counts)
    {
            Console.WriteLine($"RemoteUserId:{c.UserId} / Count:{c.Count}");
    }
        
        Console.WriteLine("エンターで終了");
        Console.ReadLine();
    }
    static AppConfig LoadConfig()
    {
       var config = new AppConfig();
        var Cfg = new ConfigurationBuilder()
    .AddIniFile(".\\config.ini")
    .Build();
        var sec_Remote = Cfg.GetSection("Remote");
        config.Remote_Token = sec_Remote["Token"];
        config.Remote_Host = sec_Remote["Host"];
        var Cnt = Convert.ToInt32(sec_Remote["UserCount"]);
        for (var i = 0; i < Cnt; i++)
        {
            var sec_RemoteUser = Cfg.GetSection($"Remote_User_{i + 1}");
            config.UserIDs.Add(sec_RemoteUser["id"]);
        }
        var sec_Local = Cfg.GetSection("Local");
        config.Local_Token = sec_Local["Token"];
        config.Local_Host = sec_Local["Host"];

        var sec_App = Cfg.GetSection("App");
        config.Delay = Convert.ToInt32(sec_App["Delay"]);
        return config;
    
}
public class AppConfig
{
    public string Remote_Token { get; set; }
    public string Remote_Host { get; set; }
    public string Local_Token { get; set; }
    public string Local_Host { get; set; }
    public List<string> UserIDs { get; set; } = new List<string>();
    public int Delay { get; set; }
}
public class Counts
{
    public string UserId { get; set; }
    public int Count { get; set; }
}
