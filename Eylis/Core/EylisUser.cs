
namespace Eylis.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net.Sockets;
    using System.Net;
    using System.Threading;
    using System.IO;
    using Eylis.Core.Protocol;

    public class EylisUser
    {
        public class ReceiveEventArgs : EventArgs
        {
            internal protected ReceiveEventArgs(EylisMessage message)
            {
                this.Message = message;
            }

            public EylisMessage Message { get; private set; }
        }

        public override int GetHashCode()
            => this.UID.GetHashCode();

        public override bool Equals(object obj)
        {
            var o = obj as EylisUser;
            return (o != null) ? o.GetHashCode() == this.GetHashCode() : false;
        }

        internal delegate void DisconnectEventHandler(EylisUser sender);
        internal event DisconnectEventHandler OnDisconnecting;

        public delegate void ReceiveEventHandler(EylisUser sender, ReceiveEventArgs e);
        internal event ReceiveEventHandler OnReceived;
        
        public static EylisUser Connect(EylisConfig config , ReceiveEventHandler onReceived = null)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect( config.Host,config.Port);
            var user = new EylisUser(tcpClient);
            if (onReceived != null)
                user.OnReceived += onReceived;
            return user;
        }

        internal EylisUser(TcpClient client)
        {
            this.UID = Guid.NewGuid();
            this.client = client;
            this.ns = client.GetStream();
            this.reader = new StreamReader(this.ns, this.Encoding);
            this.writer = new StreamWriter(this.ns, this.Encoding);
            this.token = new CancellationTokenSource();
            this.Listen();
        }
        public readonly Guid UID;
        public EndPoint RemoteEndPoint => this.client?.Client?.RemoteEndPoint;
        public Encoding Encoding => Encoding.UTF8;
        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        private CancellationTokenSource token;
        private TcpClient client;
        public void Send(EylisMessage message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }
        public void Close()
        {
            this.OnDisconnecting?.Invoke(this);
            if (!this.token.IsCancellationRequested)
                this.token.Cancel(false);
            if (this.client.Connected)
                this.client.Close();
        }
        private void Listen()
            => Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var data = reader.ReadLine();
                        if (data != null & data.Trim() != string.Empty)
                            this.OnReceived?.Invoke(this, new ReceiveEventArgs(data));
                    }
                }
                catch
                {
                    this.Close();
                }
            }, this.token.Token);
    }

}

