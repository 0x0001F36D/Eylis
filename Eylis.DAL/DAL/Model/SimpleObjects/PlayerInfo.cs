
namespace Eylis.DAL.Model.SimpleObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Eylis.DAL.Validations;

    public class PlayerInfo : EntityBase
    {
        [Singularity]
        public string Name { get; set; }
        [Singularity]
        public string Email { get; set; }

        public string Password { get; set; }
        public short Level { get; set; } = 1;
        public long HealthPoint { get; set; } = 100;
        public long ManaPoint { get; set; } = 100;
        public long ExperiencePoint { get; set; } = 100;
    }
}
