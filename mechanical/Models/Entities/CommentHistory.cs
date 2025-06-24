using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mechanical.Models.Enum;
namespace mechanical.Models.Entities
{
    public class CommentHistory
    {
        [Key]
        public Guid Id { get; set; }
        public string? CommentedFieldName { get; set; }
        public Guid? CollateralId { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid CommentByUserId { get; set; }
        public MessageType? MessageType { get; set; }
        public string? Status { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Timestamp]
        public byte[]? Version { get; set; }

        [ForeignKey("CommentByUserId")]
        public virtual User? CommentBy { get; set; }

        [ForeignKey("CollateralId")]
        public virtual Collateral? Collateral { get; set; }
        [ForeignKey("CaseId")]
        public virtual Case? Case { get; set; }

    }

}