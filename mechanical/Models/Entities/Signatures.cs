using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(UserId))]
    public class Signatures
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Emp_Id { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string SignatureBase64String { get; set; } = string.Empty;
        public Guid SignatureFileId { get; set; }

        [ForeignKey("SignatureFileId")]
        public virtual UploadFile? SignatureFile { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
