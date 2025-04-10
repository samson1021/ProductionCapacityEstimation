using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Entities
{
    public class Signatures
    {
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CreateUserId { get; set; }
        public string Emp_Id { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set;} = DateTime.Now;
        public string  CreatedBy { get; set;}= string.Empty;
        public string SignatureBase64String { get; set;}= string.Empty;
        public Guid SignatureFileId { get; set; }
        public virtual UploadFile? SignatureFile { get; set; }

        public virtual CreateUser? CreateUser { get; set; }
    }
}
