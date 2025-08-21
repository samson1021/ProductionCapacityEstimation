using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(UserId))]
    public class ConsecutiveNumber
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int NextNumber { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

    }
}
