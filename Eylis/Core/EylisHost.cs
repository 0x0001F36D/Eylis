
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



    public class EylisHost : IReadOnlyCollection<EylisUser>, IReadOnlyList<EylisUser>
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

     //   public delegate void StartEventHandler(object sender, EventArgs e);
       // public event StartEventHandler OnStart;

        public EylisHost(EylisConfig config)
        {
            this.config = config ?? EylisConfig.LoadConfig();

            if (this.Detect(this.config.Port))
            {
                this.config.WriteLog($"Port:{this.config.Port} 已被占用");
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
                        var user = new EylisUser(this.host.AcceptTcpClient());

                        user.OnReceived += (sender, e) => this.Multicast(e.Message, x => !x.Equals(sender));
                        user.OnConnect += (sender) => this.users.Add(sender);
                        user.OnDisconnect += (sender) => this.users.Remove(sender);
                        this.config.Setup(user);

                        user.Connect();
                    }
                }
                catch
                {
                    this.Stop();
                }
            }, token.Token);
            listenTask.Start();
            this.config.WriteLog("[Server] Start.");
        }
        public virtual void Stop()
        {
            if (!this.token.IsCancellationRequested)
                this.token.Cancel(false);
            this.host.Stop();

            this.config.WriteLog("[Server] Stop.");
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
