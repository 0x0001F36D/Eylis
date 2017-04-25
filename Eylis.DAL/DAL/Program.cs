using Eylis.DAL.Model;
using Eylis.DAL.Model.SimpleObjects;
using Eylis.DAL.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eylis.DAL.Helper;

namespace Eylis.DAL
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.BufferHeight = short.MaxValue - 1;

            var properties = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(EntityBase)))
                .SelectMany(x => x.GetProperties((BindingFlags)17301375)
                                .Select(v => new
                                {
                                    Type = x.UnderlyingSystemType,
                                    PropertyName = v.Name,
                                    HasAttr = v.GetCustomAttribute<SingularityAttribute>() != null
                                }).Where(v => v.HasAttr));
            foreach (var p in properties)
            {
                Console.WriteLine(p);
            }

            Console.ReadKey();
        }
    }
}
