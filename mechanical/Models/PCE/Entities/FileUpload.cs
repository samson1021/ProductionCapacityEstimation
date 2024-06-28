using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Enum.PCEEnums.File;

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

        public Guid? PCEId { get; set; } 
        [ForeignKey("PCEId")]
        
        [JsonIgnore]
        public virtual ProductionCapacityEstimation PCE { get; set; }

        public DocumentType Type { get; set; }
        public DateTime UploadAt { get; set; }
        public Guid UploadedBy { get; set; }

    }
}