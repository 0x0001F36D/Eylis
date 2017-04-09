
namespace Eylis.TinyClient.Test
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Eylis.Core;

    class Client
    {
        public static void Main()
        {
            try
            {
                var user = EylisUser.Connect(new EylisConfig("lollipo.pw"), (sender, e) => { Console.WriteLine(e.Message.ToString()); });
                while (true)
                {
                    var msg = Console.ReadLine();
                    if (msg.Trim().ToLower().Contains("exit")) break;
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
