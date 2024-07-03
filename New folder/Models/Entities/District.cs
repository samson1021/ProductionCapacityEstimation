using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;


namespace mechanical.Models.Entities
{
    public class District
    {   
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
