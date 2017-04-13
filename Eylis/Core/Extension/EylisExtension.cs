
namespace Eylis.Core.Extension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EylisExtensions
    {
        public static void ForEach<R>(this IEnumerable<R> collection, Action<R> action)
        {
            foreach (var item in collection)
                action(item);
        }


        public static TLog WriteLog<TLog>(this TLog history, bool write2log = false, string path = "host.log")
            => WriteLog(history, x => x.ToString() , write2log,path);

        public static TLog WriteLog<TLog>(this TLog history, Func<TLog, string> displayformat, bool write2log = false, string path = "host.log")
        {
            var msg = $"[{DateTime.Now}] : { displayformat(history)}";
            if (write2log)
                using (var sw = new StreamWriter(path, true, Encoding.UTF8))
                {
                    sw.WriteLine(msg);
                }
            Console.WriteLine(msg);
            return history;
        }
    }
}
