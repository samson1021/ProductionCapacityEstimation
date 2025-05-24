using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Entities
{
    [Index(nameof(CollateralId))]
    public class Reject
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid CollateralId { get; set; }
        public required Guid RejectedBy { get; set; }
        public required string RejectionComment { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
