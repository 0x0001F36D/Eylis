
namespace EylisProtocol.Infrastructure
{
    using EylisProtocol.Object;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;

    public class EylisServer //: List<EylisUser>, IObserver<EylisUser>
    {
        public string Name { get; private set; }
        private TcpListener host;
        public static EylisServer Prepare(ushort port,string serverName)
            =>new EylisServer(port,serverName);
        
        private EylisServer(ushort port ,string serverName)
        {
            this.host = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            this.Name = serverName;
        }

        public async Task Start()
        {
            host.Start();
            Task listen = new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        var client = this.host.AcceptTcpClient();
                        if (client.Connected)
                        {
                            Console.WriteLine("Connect : "+client.Client.RemoteEndPoint);
                            var ns = client.GetStream();
                            if(ns.CanRead)
                            {
                                using (var reader = new BinaryReader(ns))
                                {
                                    var fromClient = reader.ReadBytes(client.ReceiveBufferSize);
                                    var toClient = Encoding.UTF8.GetBytes("FromServer : ").Concat(fromClient).ToArray();
                                    if (ns.CanWrite)
                                    {
                                        using (var writer = new BinaryWriter(ns))
                                        {
                                            writer.Write(toClient);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch(SocketException se)
                {
                    Console.WriteLine(se.Message);
                }
            });
            listen.Start();
            
            await listen;
        }

        public void Stop()
        {
            host.Stop();
        }

        public void OnNext(EylisUser value)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
