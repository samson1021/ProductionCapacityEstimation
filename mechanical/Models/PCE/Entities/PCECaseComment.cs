using mechanical.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Entities
{
    public class PCECaseComment
    {
        public Guid Id { get; set; }
        public Guid PCECaseId { get; set; }
        public Guid AuthorId { get; set; }
        [StringLength(500)]
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public CreateUser? Author { get; set; }
        public PCECase? PCECase { get; set; }
    }
}
