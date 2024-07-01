using mechanical.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.PCE.Entities
{
    public class PCEUploadFile
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Catagory { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Extension { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime UploadDateTime { get; set; }
        public Guid userId { get; set; }
        [ForeignKey("userId")]
        public CreateUser CreateUser { get; set; }

        public Guid PCECaseId { get; set; }
        [ForeignKey("PCECaseId")]
        public PCECase pcecase { get; set; }


        public Guid? PlantCapacityEstimationId { get; set; }
        [ForeignKey("PlantCapacityEstimationId")]
        public PlantCapacityEstimation pce { get; set; }

    }
}
