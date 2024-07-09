using System.ComponentModel.DataAnnotations;
namespace mechanical.Models.Entities
{
    public class CaseComment
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }    
        public Guid AuthorId { get; set; }  
        [StringLength(500)]
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public CreateUser? Author { get; set; }
        public Case? Case { get; set; }
    }
}
