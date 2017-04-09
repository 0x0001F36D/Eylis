
namespace EylisProtocol.TinyClient.Test
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    class Client
    {

        public class Message
        {
            public byte[] Data { get; private set; }
            public Encoding Encoding { get; private set; }
            public static implicit operator Message(string message)
                => new Message(Encoding.UTF8.GetBytes(message));
            public static implicit operator Message(byte[] data)
                => new Message(data);

            public Message(byte[] data, Encoding encoding)
            {
                if (data == null | data.Length < 2)
                    throw new ArgumentNullException(nameof(data));
                this.Encoding = encoding ?? Encoding.UTF8;
                this.Data = data;
            }
            public Message(byte[] data) : this(data, null)
            {

            }
            public override string ToString()
                => this.Encoding.GetString(this.Data);

        }
        public class User
        {
            public class ReceiveEventArgs : EventArgs
            {
                public ReceiveEventArgs(Message message)
                {
                    this.Message = message;
                }

                public Message Message { get; private set; }
            }

            public override int GetHashCode()
                => this.client.Client.LocalEndPoint.ToString().GetHashCode();

            public override bool Equals(object obj)
            {
                var o = obj as User;
                return (o != null) ? o.GetHashCode() == this.GetHashCode() : false;

            }

            internal delegate void DisconnectEventHandler(User sender);
            internal event DisconnectEventHandler OnDisconnecting;

            public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);
            public event ReceiveEventHandler OnReceived;
            public User(TcpClient client, Encoding encoding = null, ReceiveEventHandler onReceived = null)
            {
                this.client = client;
               
                this.Encoding = encoding ?? Encoding.UTF8;
                if (onReceived != null)
                    this.OnReceived += onReceived;
                this.ns = client.GetStream();
                this.reader = new StreamReader(this.ns, this.Encoding);
                this.writer = new StreamWriter(this.ns, this.Encoding);
                this.token = new CancellationTokenSource();
                this.Listen();
            }
            public EndPoint RemoteEndPoint => this.client?.Client?.RemoteEndPoint;
            public Encoding Encoding { get; private set; }
            private NetworkStream ns;
            private StreamReader reader;
            private StreamWriter writer;
            private CancellationTokenSource token;
            private TcpClient client;
            public void Send(Message message)
            {
                writer.WriteLine(message);
                writer.Flush();
            }
            public void Close()
            {
                if (!this.token.IsCancellationRequested)
                    this.token.Cancel(false);
                this.OnDisconnecting?.Invoke(this);
                if (this.client.Connected)
                    this.client.Close();
            }
            private void Listen()
            {
                Task t = new Task(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var data = reader.ReadLine();
                            if (data != null & data.Trim() != string.Empty)
                                this.OnReceived?.Invoke(this, new ReceiveEventArgs((Message)data));
                        }
                    }
                    catch
                    {
                        //遠端連線終止
                        this.Close();
                    }
                }, this.token.Token);
                t.Start();
            }


        }
        public static void Main()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11222));
                User user = new User(client, null, (sender, e) => { Console.WriteLine(e.Message.ToString()); });
                while (true)
                {
                    Console.Write("輸入訊息 :");
                    var msg = Console.ReadLine();
                    if (msg.Trim().ToLower() == "esc") break;
                    user.Send(msg);
                }
                Console.ReadKey();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
                Console.ReadKey();
            
        }

    }

}
