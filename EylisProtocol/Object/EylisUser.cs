
namespace EylisProtocol.Object
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net.Sockets;
    using System.Net;

    public class EylisUser
    {
        internal EylisUser(TcpClient client)
        {
            this.client = client;
            this.EndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        }
        private TcpClient client;
        public IPEndPoint EndPoint { get; private set; }
        public string Name { get; private set; }
        public DateTime ConnectedTime { get; private set; }
        public Socket Socket { get; private set; }
        public List<EylisMessage> Messages { get; private set; }
    }
}
