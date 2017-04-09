
namespace Eylis.TinyServer.Test
{
    using Eylis.Core;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            
            var server = new EylisHost();
            server.Start();
            
            Console.ReadKey();
        }
        
    }
}