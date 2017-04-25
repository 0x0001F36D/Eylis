
namespace Eylis.DAL.Helper
{
    using Eylis.DAL.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class EntityHelper
    {
        public static IDictionary<string, object> Dump<TEntity>(this TEntity obj) where TEntity : EntityBase
            => typeof(TEntity)
                .GetProperties((BindingFlags)17301375)
                .Aggregate(new Dictionary<string, object>
                {
                    [nameof(obj.Id)] = obj.Id
                },
                (dict, x) =>
                {
                    dict.Add(x.Name, x?.GetValue(obj));
                    return dict;
                });
        
    }
}
