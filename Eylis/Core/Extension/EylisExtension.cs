
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


        public static TLog WriteLog<TLog>(this TLog history,string path = "host.log")
            => WriteLog(history, x => x.ToString(),path);

        public static TLog WriteLog<TLog>(this TLog history, Func<TLog, string> displayformat, string path = "host.log")
        {
            var msg = $"[{DateTime.Now}] : { displayformat(history)}";
            using (var sw = new StreamWriter(path, true, Encoding.UTF8))
            {
                sw.WriteLine(msg);
            }
            Console.WriteLine(msg);
            return history;
        }
    }
}
