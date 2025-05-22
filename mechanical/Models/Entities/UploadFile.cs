using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Entities
{
    public class UploadFile
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Extension { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        
        public DateTime UploadDateTime { get; set; }
        public Guid userId { get; set; }
        public Guid CaseId { get; set; }
        public Guid? CollateralId { get; set; }

    }
}
