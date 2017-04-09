
namespace EylisProtocol.TinyServer.Test
{ 
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
    using EylisProtocol.Infrastructure;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new EylisServer();
            server.Start();
            Console.ReadKey();
        }
        
    }
}//end namespace