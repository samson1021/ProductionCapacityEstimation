using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCECaseId))]
    public class PCECaseComment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PCECaseId { get; set; }
        public Guid AuthorId { get; set; }
        [StringLength(500)]
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }


        [ForeignKey("AuthorId")]
        public CreateUser? Author { get; set; }

        [ForeignKey("PCECaseId")]
        public PCECase? PCECase { get; set; }
    }
}
