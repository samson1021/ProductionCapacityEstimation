using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Enum.File;

namespace mechanical.Models.PCE.Entities
{
    public class FileUpload
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Extension { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public Category Category { get; set; }
        public DateTime UploadedAt { get; set; }
        public Guid UploadedBy { get; set; }

        public Guid? PCEEId { get; set; } 
        [ForeignKey("PCEEId")]
        // [JsonIgnore]
        public virtual PCEEvaluation PCEEvaluation { get; set; }
    }
}