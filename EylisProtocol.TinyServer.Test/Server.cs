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
        public class ReceiveEventArgs : EventArgs
        {
            public ReceiveEventArgs(Message message)
            {
                this.Message = message;
            }

            public Message Message { get; private set; }
        }
        public class Message
        {
            public byte[] Data { get; private set; }
            public Encoding Encoding { get; private set; }
            public static explicit operator Message(string message)
                => new Message(Encoding.UTF8.GetBytes(message));
            public static explicit operator Message(byte[] data)
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
        class User 
        {
            public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);
            public event ReceiveEventHandler OnReceived;
            public User(TcpClient client,Encoding encoding = null , ReceiveEventHandler onReceived = null)
            {
                this.client = client;
                this.Encoding = encoding ?? Encoding.UTF8;
                if (onReceived != null)
                    this.OnReceived += onReceived;
                var ns = client.GetStream();
                this.reader = new StreamReader(ns,this.Encoding);
                this.writer = new StreamWriter(ns, this.Encoding);
                this.token = new CancellationTokenSource(0);
            }
            public EndPoint Host => this.client?.Client?.RemoteEndPoint;
            public Encoding Encoding { get; private set; }
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
                    this.token.Cancel();
                if(this.client.Connected)
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
                        if (!token.IsCancellationRequested)
                        token.Cancel();
                    }
                },this.token.Token);
                t.Start();
            }
            
        }


        class Server:List<User>
        {
            public static Server CreateHost(ushort port = 11222)
                => new Server(port);
            private TcpListener host;
            private Task listenTask;
            private CancellationTokenSource token;
            private Server(ushort port)
            {
                this.host = new TcpListener(IPAddress.Any, port);
                this.token = new CancellationTokenSource(0);
            }
            public void Start()
            {
                this.host.Start();
                listenTask = new Task(() => 
                {
                    var client = default(TcpClient);
                    while(true)
                    {
                        client = this.host.AcceptTcpClient();
                        this.Add(new User(client));
                    }
                }, token.Token);
                listenTask.Start();
            }
            public void Stop()
            {
                this.token.Cancel(false);
                this.host.Stop();
            }
        }
        public static Hashtable clientsList = new Hashtable();

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 11222);
            TcpClient clientSocket = default(TcpClient);
            int counter = 0;
            serverSocket.Start();
            while ((true))
            {
                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();

                byte[] bytesFrom = new byte[10025];
                string dataFromClient = null;

                NetworkStream networkStream = clientSocket.GetStream();
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.Length).Trim();

                clientsList.Add(dataFromClient, clientSocket);

                broadcast(dataFromClient + " Joined ", dataFromClient, false);

                Console.WriteLine(dataFromClient + " Joined chat room ");
                handleClinet client = new handleClinet();
                client.startClient(clientSocket, dataFromClient, clientsList);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void broadcast(string msg, string uName, bool flag)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                NetworkStream broadcastStream = broadcastSocket.GetStream();
                Byte[] broadcastBytes = null;

                if (flag == true)
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(uName + " says : " + msg);
                }
                else
                {
                    broadcastBytes = Encoding.ASCII.GetBytes(msg);
                }

                broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                broadcastStream.Flush();
            }
        }  //end broadcast function
    }//end Main class


    public class handleClinet
    {
        TcpClient clientSocket;
        string clNo;
        Hashtable clientsList;

        public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clientsList = cList;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.Trim().Length);
                    Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                    rCount = Convert.ToString(requestCount);

                    Program.broadcast(dataFromClient, clNo, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    break;
                }
            }//end while
        }//end doChat
    } //end class handleClinet
}//end namespace