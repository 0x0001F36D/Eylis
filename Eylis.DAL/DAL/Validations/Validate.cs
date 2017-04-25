using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Eylis.DAL.Model;

namespace Eylis.DAL.Validations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SingularityAttribute : Attribute 
    {
    }

    public class En : EntityBase
    {

        [Singularity]
        public int MyProperty { get; set; }
    }

    public class Validator
    {
        [Singularity]
        public int MyProperty2 { get; set; }
        [Singularity]
        public object MyProperty3 { get; set; }


        public Validator()
        {
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
        }
    }

}
