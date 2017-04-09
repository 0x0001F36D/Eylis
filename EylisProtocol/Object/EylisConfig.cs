
namespace EylisProtocol.Object
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EylisConfig : DnsEndPoint
    {
        public new int Port => port;
        internal const ushort port = 11222;
        
        public EylisConfig(string host) : base(host, EylisConfig.port)
        {

        }

        public readonly static EylisConfig Localhost = new EylisConfig("127.0.0.1");

    }
}
