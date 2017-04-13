
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

    [JsonObject("Config")]
    public sealed class EylisConfig
    {
        [JsonProperty("Host")]
        public string Host { get; private set; }

        [JsonProperty("Port")]
        public ushort Port { get; private set; }


        [JsonProperty("Enable Logger")]
        public bool EnableLogger { get; private set; }

        [JsonIgnore]
        private readonly DnsEndPoint endPoint;
        
        [JsonConstructor]
        public EylisConfig(string host , ushort port = 11222 , bool enableLogger = true)
        {
            this.endPoint = new DnsEndPoint(host, port);
            this.Port = port;
            this.Host = host;
            this.EnableLogger = enableLogger;
        }

        public override string ToString()
            => new StringBuilder()
                .AppendLine($"Host   : {Host}")
                .AppendLine($"Port   : {Port}")
                .AppendLine($"Logger : { (EnableLogger ? "Enable" : "Disable") }").ToString();
           
        

        [JsonIgnore]
        public const string path = "config.json";

        public static EylisConfig LoadConfig()
        {
            if (!File.Exists(path))
            {
                var config = new EylisConfig("127.0.0.1"); ;
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(path, json);
                return config;
            }
            else
            {

                try
                {
                    return JsonConvert.DeserializeObject<EylisConfig>(File.ReadAllText(path));
                }
                catch
                {
                    Console.WriteLine($"檔案 {path} 毀損");
                    Environment.Exit(-1);
                    return null;
                }
            }
        }
    }
}
