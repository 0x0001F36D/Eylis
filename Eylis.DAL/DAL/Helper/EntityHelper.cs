
namespace Eylis.DAL.Helper
{
    using Eylis.DAL.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EntityHelper
    {
        public static TPoco Generate<TPoco>(this Entity<TPoco> provider) where TPoco : PocoBase, new()
            => PocoBase.Generate(provider);
        
    }
}
