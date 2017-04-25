
namespace Eylis.DAL.Model
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Configuration;
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Linq.Expressions;
    using System.ComponentModel;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public sealed class EntityFactory<TEntity> : DbContext where TEntity : EntityBase ,new()
    { 
        public static EntityFactory<TEntity> Load(string password = null)
        {
            var name = typeof(TEntity).Name;
            var connectionStrings = string.Join(";", new Dictionary<string, string>
            {
                ["Data Source"] = name ,
                ["Password"] = password ?? typeof(TEntity).GUID.ToString()
            }
            .Select(x => $"{x.Key}={x.Value}"));
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection(nameof(connectionStrings));
            if (connectionStringsSection.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(x => x.Name == name) == null)
            {
                connectionStringsSection.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionStrings));
                config.Save();
                ConfigurationManager.RefreshSection(nameof(connectionStrings));
            }
            return new EntityFactory<TEntity>(connectionStrings);
        }

        private EntityFactory(string connectionString) : base(connectionString)
        {
        }
 
        public TEntity Generate()
            => new TEntity { Id = this.EntityList.Count() };
        
        public IDbSet<TEntity> EntityList { get; set; }
        
    }
}
