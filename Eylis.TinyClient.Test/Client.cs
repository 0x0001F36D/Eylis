
namespace Eylis.TinyClient.Test
{
    using Eylis.Core;
    using System;

    class Client
    {
        public static void Main()
        {
                var user = EylisUser.Connect(new EylisConfig("lollipo.pw"), (sender, e) => { Console.WriteLine(e.Message.ToString()); });
                while (true)
                {
                    var msg = Console.ReadLine();
                    if (msg.Trim().ToLower().Contains("exit")) break;
                    user.Send(msg);
                }
                Console.ReadKey();
           
            Console.ReadKey();

        }

    }

}
