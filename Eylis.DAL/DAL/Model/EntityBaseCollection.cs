
namespace Eylis.DAL.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal class EntityBaseCollection<TEntity> : DbSet<TEntity>,IDbSet<TEntity>, IQueryable<TEntity> where TEntity : EntityBase
    {
        private readonly DbSet<TEntity> _internalSet;

        IQueryProvider Provider => (_internalSet as IDbSet<TEntity>).Provider;
        Type ElementType => ((IDbSet<TEntity>)_internalSet).ElementType;
        Expression Expression => ((IDbSet<TEntity>)_internalSet).Expression;

        internal EntityBaseCollection(DbSet<TEntity> internalSet)
            => _internalSet = internalSet;
        
        public override ObservableCollection<TEntity> Local
            => _internalSet.Local;
        
        public override DbQuery<TEntity> AsNoTracking()
            => _internalSet.AsNoTracking();

        public override DbQuery<TEntity> Include(string path)
            => _internalSet.Include(path);

        public override TEntity Add(TEntity entity)
            => _internalSet.Add(entity);

        public override TEntity Attach(TEntity entity)
            => _internalSet.Attach(entity);

        public override TEntity Create()
            => _internalSet.Create();

        public override TEntity Find(params object[] keyValues)
            => _internalSet.Find(keyValues);

        public override TEntity Remove(TEntity entity)
            => _internalSet.Remove(entity);

        public override TDerivedEntity Create<TDerivedEntity>()
            => _internalSet.Create<TDerivedEntity>();
        
        public override IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
            => _internalSet.AddRange(entities);
        
        public override Task<TEntity> FindAsync(params object[] keyValues)
            => _internalSet.FindAsync(keyValues);
        
        public override Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
            => _internalSet.FindAsync(cancellationToken, keyValues);

    }
}
