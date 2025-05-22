using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(CaseId))]
    public class CaseComment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }    
        public Guid AuthorId { get; set; }  
        [StringLength(500)]
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("AuthorId")]
        public User? Author { get; set; }
        [ForeignKey("CaseId")]
        public Case? Case { get; set; }
    }
}
