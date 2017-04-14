
namespace Eylis.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.IO;
    using static Eylis.Core.EylisUser;

    [JsonObject("Config")]
    public sealed class EylisConfig
    {
        [JsonProperty("Host")]
        public string Host { get; private set; }

        [JsonProperty("Port")]
        public ushort Port { get; private set; }

        [JsonIgnore]
        private readonly DnsEndPoint endPoint;
        
        [JsonConstructor]
        public EylisConfig(string host , ushort port = 11222 , bool enable = true)
        {
            this.endPoint = new DnsEndPoint(host, port);
            this.Port = port;
            this.Host = host;
            this.Enable = enable;
            this.OnReceived = (sender, e) => this.WriteLog($"[{sender.RemoteEndPoint}]: {e.Message}");
            this.OnConnecting = (sender) => this.WriteLog($"[{sender.RemoteEndPoint}]: Connected");
            this.OnDisconnecting = (sender) => this.WriteLog($"[{sender.RemoteEndPoint}]: Disconnected");
        }

        public override string ToString()
            => new StringBuilder()
                .AppendLine($"Host   : {Host}")
                .AppendLine($"Port   : {Port}")
                .AppendLine($"Logger Status : { this.Status }")
                .ToString();


        [JsonIgnore]
        public const string log = "history.log";
        [JsonIgnore]
        public const string config = "config.json";

        public static EylisConfig LoadConfig()
        {
            if (!File.Exists(config))
            {
                var config = new EylisConfig("127.0.0.1"); ;
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(EylisConfig.config, json);
                return config;
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject<EylisConfig>(File.ReadAllText(config));
                }
                catch
                {
                    Console.WriteLine($"檔案 {config} 毀損");
                    Environment.Exit(-1);
                    return null;
                }
            }
        }

        [JsonProperty("Enable Log")]
        public bool Enable { get; set; }

        [JsonIgnore]
        public string Status => this.Enable ? "Enable" : "Disable";
        
        [JsonIgnore]
        internal ConnectEventHandler OnConnecting;
        [JsonIgnore]
        internal DisconnectEventHandler OnDisconnecting;
        [JsonIgnore]
        internal ReceiveEventHandler OnReceived;
        
        internal void Setup(EylisUser user)
        {
            user.OnConnecting += OnConnecting;
            user.OnDisconnecting += OnDisconnecting;
            user.OnReceived += OnReceived;
        }

        internal void WriteLog(string message)
        {
            if (Enable)
                using (var sw = new StreamWriter(log,true))
                    sw.WriteLine($"[{DateTime.Now.ToString("yyyy/mm/dd hh:mm:ss.ffffff tt")}] {message}");
            Console.WriteLine(message);
        }

    }
}
