
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
    using Newtonsoft.Json;
    using System.IO;
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

        private void KickOut()
        {
            this.UserList();
            Console.Write("Select id :");
            var us = Console.ReadLine()
                            .Select(x => char.IsDigit(x) ? x.ToString() : " ")
                            .Aggregate((x, y) => x += y)
                            .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => int.TryParse(v.Trim(), out int q) & q < this.Count? q : -1)
                            .Where(v => v != -1);
            Console.Write("Input 'Y' to kick : ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                foreach (var u in us)
                    this[u].Disconnect();
                Console.WriteLine("\nAll kicked out!");
            }
        }
        private void Unblock()
        {

        }
        private void Block()
        {
            this.UserList();
            Console.Write("Input id :");
            var msg3 = Console.ReadLine();

            if (int.TryParse(msg3, out int x) & x < this.Count)
            {
                Console.Write("Input 'Y' to block : ");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    File.AppendText(this[x].RemoteEndPoint.ToString());
                    this[x].Disconnect();
                    Console.WriteLine("\nUser blocked !");
                }
            }
        }

        private void KickOutAll()
        {
            Console.Write("Input 'Y' to kick all: ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                for (int i = 0; i < this.Count; this[i++].Disconnect()) ;
                Console.WriteLine("All kicked out!");
            }
        }
        private void WhatIsMyIP()
            => Console.WriteLine($"IP Address : {(new WebClient().DownloadStringTaskAsync("https://api.ipify.org").Result)}");
        private void Clear()
            => Console.Clear();
        private void OpenConfig()
            => Process.Start("notepad", Environment.CurrentDirectory + "//" + EylisConfig.path);
        private void OpenLog()
            => Process.Start("notepad", Environment.CurrentDirectory + "//host.log");
        private void Exit()
            => Environment.Exit(0);
        private void UserList()
        {
            Console.WriteLine($"Online User : {this.Count}");
            for (int i = 0 ; i < this.Count; Console.WriteLine($"    [ID: {i.ToString().PadRight(3)}] : {this[i++].RemoteEndPoint}")) ;
        }
        private void Broadcast()
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
        private void Multicast()
        {
            Console.Write("Input message :");
            var msg = Console.ReadLine();
            this.UserList();
            Console.Write("Select id :");
            var us = Console.ReadLine()
                            .Select(x => char.IsDigit(x) ? x.ToString() : " ")
                            .Aggregate((x, y) => x += (y))
                            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => int.TryParse(v.Trim(), out int q) & q < this.Count? this[q] : null)
                            .Where(v => v != null);
            Console.Write("Input 'Y' to send : ");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                this.Multicast(msg, x => us.Contains(x));
                Console.WriteLine("\nMessage sent !");
            }
        }
        private void Unicast()
        {
            Console.Write("Input message :");
            var msg2 = Console.ReadLine();
            this.UserList();
            Console.Write("Input id :");
            var msg3 = Console.ReadLine();

            if (int.TryParse(msg3, out int x) & x < this.Count)
            {
                Console.Write("Input 'Y' to send : ");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    this.Unicast(msg2, this[x]);
                    Console.WriteLine("\nMessage sent !");
                }
            }
        }
        private void ShowConfig()
            => Console.WriteLine($"Loading {EylisConfig.path}\n{config.ToString()}");

        private void Help()
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

        public override void Start()
        {
            base.Start();
            Gate:
            this[Console.ReadLine().TrimEnd().ToLower()]();
            goto Gate;
        }
        

    }
}
