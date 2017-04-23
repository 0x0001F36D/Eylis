
namespace Eylis.DAL.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    
    public class PocoBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; private set; }

        protected PocoBase(int id)
            => this.Id = id;

        public PocoBase() { }

        internal static TPoco Generate<TPoco>(Entity<TPoco> provider) where TPoco : PocoBase, new()
            => new TPoco() { Id = provider.Count };

    }
}
