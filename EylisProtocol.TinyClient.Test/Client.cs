
namespace EylisProtocol.TinyClient.Test
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using EylisProtocol.Object;

    class Client
    {
        public static void Main()
        {
            try
            {
                var user = EylisUser.Connect(EylisConfig.Localhost, (sender, e) => { Console.WriteLine(e.Message.ToString()); });
                while (true)
                {
                    var msg = Console.ReadLine();
                    if (msg.Trim().ToLower() == "esc") break;
                    user.Send(msg);
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();

        }

    }

}
