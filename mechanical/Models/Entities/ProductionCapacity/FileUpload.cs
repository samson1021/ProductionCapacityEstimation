using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.File;

namespace mechanical.Models.Entities.ProductionCapacity
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
        
        public DocumentType Type { get; set; }
        public Guid? PCEId { get; set; } 
        // [ForeignKey("Id")]
        public virtual ProductionCapacityEstimation PCE { get; set; }

        public DateTime UploadAt { get; set; }
        public Guid UploadedBy { get; set; }

    }
}