
namespace Eylis.DAL.Model
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Configuration;

    public sealed class Entity<TPoco> : DbContext where TPoco : PocoBase
    {
        public static Entity<TPoco> Load(string password = null)
        {
            var name = typeof(TPoco).Name;
            var connectionStrings = string.Join(";", new Dictionary<string, string>
            {
                ["Data Source"] = name + ".sdf",
                ["Password"] = password ?? typeof(TPoco).GUID.ToString()
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
            return new Entity<TPoco>(connectionStrings);
        }

        private Entity(string connectionString) : base(connectionString)
        {
            
        }

        public DbSet<TPoco> EntityList { get; set; }

        public int Count => this.EntityList.Count();
    }
}
