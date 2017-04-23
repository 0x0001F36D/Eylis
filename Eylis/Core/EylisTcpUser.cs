
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
                => this.Message = message;
            public EylisMessage Message { get; private set; }
        }

        public override int GetHashCode()
            => this.client.Client.RemoteEndPoint.GetHashCode() + this.client.Client.LocalEndPoint.GetHashCode();

        public override bool Equals(object obj)
            => (obj is EylisUser) ? (obj as EylisUser).GetHashCode() == this.GetHashCode() : base.Equals(this);

        public delegate void ConnectEventHandler(EylisUser user);
        internal event ConnectEventHandler OnConnect;

        public delegate void DisconnectEventHandler(EylisUser user);
        internal event DisconnectEventHandler OnDisconnect;

        public delegate void ReceiveEventHandler(EylisUser sender, ReceiveEventArgs e);
        internal event ReceiveEventHandler OnReceived;

        public static EylisUser Bind(EylisConfig config, ReceiveEventHandler onReceive = null, ConnectEventHandler onConnecting = null , DisconnectEventHandler onDisconnecting = null )
        {
            var tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(config.Host, config.Port);
            }
            catch(SocketException s)
            {
                throw s.InnerException;
            }
            var user = new EylisUser(tcpClient,onReceive,onConnecting,onDisconnecting);
            return user;
        }

        internal EylisUser(TcpClient client, ReceiveEventHandler onReceive = null, ConnectEventHandler onConnecting = null, DisconnectEventHandler onDisconnecting = null)
        {
            this.OnReceived += onReceive;
            this.OnConnect += onConnecting;
            this.OnDisconnect += onDisconnecting;
            this.IsConnected = false;
            this.client = client;
            this.ns = client.GetStream();
            this.reader = new StreamReader(this.ns, this.Encoding);
            this.writer = new StreamWriter(this.ns, this.Encoding);
            this.token = new CancellationTokenSource();
        }

        public bool IsConnected { get; private set; }

        public EndPoint RemoteEndPoint => this.client?.Client?.RemoteEndPoint;
        public Encoding Encoding => Encoding.UTF8;

        private NetworkStream ns;
        private StreamReader reader;
        private StreamWriter writer;
        private CancellationTokenSource token;
        private TcpClient client;

        public void Send(EylisMessage message)
        {
            try
            {
                writer.WriteLine(message);
                writer.Flush();
            }
            catch
            {
                this.Disconnect();
            }
        }
        public void Disconnect()
        {
            this.OnDisconnect?.Invoke(this);
            if (!this.token.IsCancellationRequested)
                this.token.Cancel(false);
            if (this.client.Connected)
                this.client.Close();
            this.IsConnected = false;
        }
        public void Connect()
        {
            if (!this.IsConnected)
            {
                this.IsConnected = true;
                Task.Run(() =>
                {
                    try
                    {
                        this.OnConnect?.Invoke(this);
                        while (true)
                        {
                            var data = reader.ReadLine();
                            if (!string.IsNullOrWhiteSpace(data))
                                this.OnReceived?.Invoke(this, new ReceiveEventArgs(data));
                        }
                    }
                    finally
                    {
                        this.Disconnect();
                    }
                }, this.token.Token);
            }
        }

    }
}

