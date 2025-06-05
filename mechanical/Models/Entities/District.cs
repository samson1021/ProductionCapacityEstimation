using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Entities
{
    public class District
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static implicit operator District(string v)
        {
            throw new NotImplementedException();
        }
    }
}
