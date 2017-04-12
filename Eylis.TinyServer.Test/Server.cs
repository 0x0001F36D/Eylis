
namespace Eylis.TinyServer.Test
{
    using Eylis.Core;
    using Eylis.Plugin.Command;
    using Eylis.Plugin.Command.Common;
    using Eylis.Plugin.Package;
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    
    class Program
    {


        static void Main(string[] args)
        {
           
            EylisHost host = new EylisHost();
            host.Start();
                
            Console.ReadKey();
            return;

            var chat = new Chat(Header.Version.V1, Header.Method.Anonymous_User, 1, 1, "", "hello");
            var bytes = chat.ToBytes();

            var chat2 = bytes.To<Chat>();
            Console.WriteLine(chat2.Version);
            Console.WriteLine(chat2.Method);
            Console.WriteLine(chat2.Part);
            Console.WriteLine(chat2.Total);
            Console.WriteLine(chat2.Name);
            Console.WriteLine(chat2.Payload);

            Console.ReadKey();
        }
        
    }
}