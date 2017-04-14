
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

        
    }
}
