
namespace Eylis.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Collections;
    using System.Net.NetworkInformation;
    using Eylis.Core.Protocol;
    using Eylis.Core.Extension;

    public class EylisHost : IReadOnlyCollection<EylisUser>,IReadOnlyList<EylisUser>
    {
        private HashSet<EylisUser> users;
        private TcpListener host;
        private CancellationTokenSource token;
        private EylisConfig config;

        public int Count => this.users.Count;

        public EylisUser this[int index] => this.users.ElementAt(index);

        private bool Detect(int port)
            => IPGlobalProperties.GetIPGlobalProperties()
                                    .GetActiveTcpListeners()
                                    .Any(x => x.Address.Equals(IPAddress.Any) & x.Port == port);
        

        public EylisHost(EylisConfig config)
        {
            this.config = config ?? EylisConfig.LoadConfig();

            if (this.Detect(this.config.Port))
            {
                $"Port:{this.config.Port} 已被占用".WriteLog(this.config.EnableLogger);
                Console.ReadKey();
                Environment.Exit(0);
            }
            
            this.users = new HashSet<EylisUser>();
            this.host = new TcpListener(IPAddress.Any, this.config.Port);
            this.token = new CancellationTokenSource();
        }
        public virtual void Start()
        {
            this.host.Start();
            var listenTask = new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        var client = this.host.AcceptTcpClient();
                        var user = new EylisUser(client);
                        /**/
                        user.OnDisconnecting += (sender) =>
                        {
                            this.users.Remove(sender);
                            sender.WriteLog(x => $"[Server] <{x.RemoteEndPoint}> offline.",this.config.EnableLogger);
                           // this.Broadcast($"[Server] '{sender.RemoteEndPoint}' is disconnect");
                        };
                        user.OnReceived += (sender, e) => this.Multicast( e.Message.WriteLog(v=>$"[{sender.RemoteEndPoint}] : {v.ToString()}", this.config.EnableLogger), x => !x.Equals(sender));

                        this.users.Add(user);
                        // this.Broadcast($"[Server] '{user.RemoteEndPoint}' is connect");
                        user.WriteLog(x => $"[Server] <{x.RemoteEndPoint}> online.", this.config.EnableLogger);
                    }
                }
                catch
                {
                    this.Stop();
                }
            }, token.Token);
            listenTask.Start();
            "[Server] Start.".WriteLog(this.config.EnableLogger);
        }
        public virtual void Stop()
        {
            if (!this.token.IsCancellationRequested)
                this.token.Cancel(false);
            this.host.Stop();
            "[Server] Stop.".WriteLog(this.config.EnableLogger);
        }
        

        public virtual void Unicast(EylisMessage message, EylisUser user)
            => this.users.FirstOrDefault(x => x.Equals(user))?.Send(message);

        public virtual void Multicast(EylisMessage message, Func<EylisUser, bool> selector)
            => this.users.Where(selector).ForEach(u => u.Send(message));
       

        public virtual void Broadcast(EylisMessage message)
            => this.users.ForEach(user => user.Send(message));

        public IEnumerator<EylisUser> GetEnumerator()
            => this.users.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
