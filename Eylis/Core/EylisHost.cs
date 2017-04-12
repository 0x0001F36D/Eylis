
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

    public class EylisHost : IReadOnlyCollection<EylisUser>
    {
        private HashSet<EylisUser> users;
        private TcpListener host;
        private CancellationTokenSource token;

        public int Count => this.users.Count;

        private bool Detect()
            => IPGlobalProperties.GetIPGlobalProperties()
                                    .GetActiveTcpListeners()
                                    .Any(x => x.Address.Equals(IPAddress.Any) & x.Port == EylisConfig.port);
        

        public EylisHost()
        {
            if (this.Detect())
                Environment.Exit(0);
            
            this.users = new HashSet<EylisUser>();
            this.host = new TcpListener(IPAddress.Any, EylisConfig.port);
            this.token = new CancellationTokenSource();
        }
        public void Start()
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
                           // this.Broadcast($"[Server] '{sender.RemoteEndPoint}' is disconnect");
                        };
                        user.OnReceived += (sender, e) => this.Multicast($"[{sender.RemoteEndPoint}]" + e.Message, x => !x.Equals(sender));

                        this.users.Add(user);
                       // this.Broadcast($"[Server] '{user.RemoteEndPoint}' is connect");

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    this.Stop();
                }
            }, token.Token);
            listenTask.Start();

        }
        public void Stop()
        {
            if (!this.token.IsCancellationRequested)
                this.token.Cancel(false);
            this.host.Stop();
        }

        private TLog Log<TLog>(TLog history)
        {
            Console.WriteLine($"[{DateTime.Now}] : {history}");
            return history;
        }

        public void Unicast(EylisMessage message, EylisUser user)
            => this.users.FirstOrDefault(x => x.Equals(user))?.Send(message);

        public void Multicast(EylisMessage message, Func<EylisUser, bool> selector)
            => this.users.Where(selector).ForEach(u => u.Send(message));

        public void Broadcast(EylisMessage message)
            => this.users.ForEach(user => user.Send(message));

        public IEnumerator<EylisUser> GetEnumerator()
            => this.users.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
