
namespace Eylis.TinyClient.Test
{
    using Eylis.Core;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    class Client
    {
        public static void Main()
        {
            var user = EylisUser.Bind(new EylisConfig("api.lollipo.pw"), 
                (sender, e) => Console.WriteLine(e.Message.ToString()),
                (s) => Console.WriteLine("connect"), 
                (s) => Console.WriteLine("disconnect"));

            while (true)
            {
                Console.Write("輸入訊息 : ");
                var msg = Console.ReadLine();
                if (msg.Trim().ToLower().Contains("exit"))
                    break;
                user.Send(msg);
            }
            Console.ReadKey();
            

        }

    }

}
