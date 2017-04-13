
namespace Eylis.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    class EylisHostConsole : EylisHost
    {
        [DllImport("Kernel32")]
        static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        delegate bool HandlerRoutine(CtrlTypes CtrlType);
        enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        internal static readonly EylisConfig config = EylisConfig.LoadConfig();
        internal readonly IEnumerable<string> methodNameList;
        internal readonly Dictionary<string, Action> methods;

        internal void KickOutAll()
        {
            for (int i = 0; i < this.Count; this[i++].Close()) ;
        }
        internal void WhatIsMyIP()
            => Console.WriteLine($"IP Address : {(new WebClient().DownloadStringTaskAsync("https://api.ipify.org").Result)}");
        internal void Clear()
            => Console.Clear();
        internal void OpenConfig()
            => Process.Start("notepad", Environment.CurrentDirectory + "//" + EylisConfig.path);
        internal void OpenLog()
            => Process.Start("notepad", Environment.CurrentDirectory + "//host.log");
        internal void Exit()
            => Environment.Exit(0);
        internal void UserList()
        {
            Console.WriteLine($"Online User : {this.Count}");
            foreach (var client in this)
                Console.WriteLine($"    [{client.RemoteEndPoint}] Uid:{client.UID}");
        }
        internal void Broadcast()
        {
            Console.Write("Input message :");
            var msg = Console.ReadLine();
            Console.Write("Input 'Y' to send : ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                this.Broadcast(msg);
                Console.WriteLine("\nMessage sent !");
            }
        }
        internal void Multicast()
        {
            Console.Write("Input message :");
            var msg = Console.ReadLine();
            for (int i = 0; i < this.Count; i++)
            {
                Console.WriteLine($"    [Id : {i.ToString().PadRight(3)}] => {this[i].RemoteEndPoint}");
            }
            Console.Write("Select id :");
            var us = Console.ReadLine()
                            .Split(',')
                            .Select(v => int.TryParse(v.Trim(), out int q) ? this[q] : null)
                            .Where(v => v != null);
            Console.Write("Input 'Y' to send : ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                this.Multicast(msg, x => us.Contains(x));
                Console.WriteLine("\nMessage sent !");
            }
        }
        internal void Unicast()
        {
            Console.Write("Input message :");
            var msg2 = Console.ReadLine();
            for (int i = 0; i < this.Count; i++)
            {
                Console.WriteLine($"    [Id : {i.ToString().PadRight(3)}] => {this[i].RemoteEndPoint}");
            }
            Console.Write("Input id :");
            var msg3 = Console.ReadLine();
            int x = 0;
            if (int.TryParse(msg3, out x))
            {
                Console.Write("Input 'Y' to send : ");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    this.Unicast(msg2, this[x]);
                    Console.WriteLine("\nMessage sent !");
                }
            }
        }
        internal void ShowConfig()
            => Console.WriteLine($"Loading {EylisConfig.path}\n{config.ToString()}");
        
        internal void Help()
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            foreach (var item in this.methodNameList)
                Console.WriteLine(item);
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public EylisHostConsole() : base(config)
        {
            Console.BufferHeight = short.MaxValue - 1;
            SetConsoleCtrlHandler((sender) =>
            {
                this.Stop();
                return true;
            }, true);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((sender, e) => {
                this.Stop();
            });
            var t = typeof(Action);
            var filter = new[] { "Finalize", "MemberwiseClone", "GetEnumerator","b__" };
            var list = this.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m =>! filter.Any(q=>m.Name.Contains(q)));
            this.methodNameList = list.Select(x => $"[{x.Name}]");
            this.methods = list.ToDictionary(d => d.Name.ToLower(),
                             d => new Action(()=>d.Invoke(this,null)));
        }
        internal readonly static Action error = () => Console.WriteLine("[Error] Method undefine.");

        public Action this[string commmand]
            =>this.methods.TryGetValue(commmand.ToLower(),out Action v) ? v:error;
        
        public void Main()
        {
            this.Start();

            Gate:
           /* var tmp = Console.ReadKey();
            if (tmp.Key == ConsoleKey.Enter)
                goto Gate;
            tmp.KeyChar +*/
            this[Console.ReadLine().TrimEnd().ToLower()]();
            goto Gate;
        }

    }
}
