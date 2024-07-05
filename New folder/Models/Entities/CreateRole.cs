using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Entities
{
    public class CreateRole
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
