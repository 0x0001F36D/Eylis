
namespace Eylis.DAL.Model
{
    using Eylis.DAL.Validations;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class EntityBase
    {
        [Key]
        [Column("Id")]
        [Singularity]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; internal set; }

        protected EntityBase(int id)
            => this.Id = id;

        public EntityBase()
        {

        }
        
    }
}
