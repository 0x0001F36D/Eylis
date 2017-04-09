using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        
        public class Message
        {
            public static Message operator +(Message l, Message r)
                => new Message(l.Data.Concat(r.Data).ToArray());
            public static Message operator +(Message l, string r)
                => new Message(l.Data.Concat(l.Encoding.GetBytes(r)).ToArray());
            public static Message operator +(string l, Message r)
                => new Message(r.Encoding.GetBytes( l).Concat(r.Data).ToArray());

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
                => this.UID.GetHashCode();
            
            public override bool Equals(object obj)
            {
                var o = obj as User;
                return ( o != null) ? o.GetHashCode() == this.GetHashCode():false;
                
            }

            internal delegate void DisconnectEventHandler(User sender);
            internal event DisconnectEventHandler OnDisconnecting;

            public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);
            public event ReceiveEventHandler OnReceived;
            public User(TcpClient client,Encoding encoding = null , ReceiveEventHandler onReceived = null)
            {
                this.UID = Guid.NewGuid();
                this.client = client;
                this.Encoding = encoding ?? Encoding.UTF8;
                if (onReceived != null)
                    this.OnReceived += onReceived;
                this.ns = client.GetStream();
                this.reader = new StreamReader(this.ns,this.Encoding);
                this.writer = new StreamWriter(this.ns, this.Encoding);
                this.token = new CancellationTokenSource();
                this.Listen();
            }
            public readonly Guid UID;
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
                this.OnDisconnecting?.Invoke(this);
                if (!this.token.IsCancellationRequested)
                    this.token.Cancel(false);
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
                },this.token.Token);
                t.Start();
            }


        }
        class Server : HashSet<User>
        {
            public static Server CreateHost(ushort port = 11222)
                => new Server(port);
            private TcpListener host;
            // private Task listenTask;
            private CancellationTokenSource token;
            private Server(ushort port)
            {
                this.host = new TcpListener(IPAddress.Any, port);
                this.token = new CancellationTokenSource();
            }
            public async Task Start()
            {
                Console.WriteLine("server start");
                this.host.Start();
                var listenTask = new Task(() =>
                {
                    try
                    {
                        while (true)
                        {
                            var client = this.host.AcceptTcpClient();

                            var user = new User(client);
                            user.OnDisconnecting += (sender) =>
                            {
                                this.Remove(sender);
                                this.Broadcast((Message)$"[Server] '{sender.RemoteEndPoint}' is disconnect");
                            };
                            user.OnReceived += (sender, e) => this.Broadcast($"[{(sender as User).RemoteEndPoint}]"+e.Message);
                            this.Broadcast((Message)$"[Server] '{user.RemoteEndPoint}' is connect");
                            this.Add(user);

                        }
                    

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        this.Stop();
                    }
                }, token.Token);
                listenTask.Start();
                await listenTask;
            }
            public void Stop()
            {
                if (!this.token.IsCancellationRequested)
                    this.token.Cancel(false);
                this.host.Stop();
                Console.WriteLine("server stop");
            }
            public TLog Log<TLog>(TLog history)
            {
                Console.WriteLine($"[{DateTime.Now}] : {history}");
                return history;
            }
            public void Broadcast(Message message)
            {
                var list = this.ToList();
                    list.ForEach(user => user.Send(this.Log( message)));
            }
        }
        
        static void Main(string[] args)
        {
            Server server = Server.CreateHost();
            server.Start();
            Console.ReadKey();
        }
        
    }
}//end namespace