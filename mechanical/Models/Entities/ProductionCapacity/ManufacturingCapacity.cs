

using mechanical.Models.Enum;
using mechanical.Models.Enum.ProductionCapacity;

using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Entities.ProductionCapacity
{
    public class ManufacturingCapacity
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required string PropertyOwner { get; set; }
        public required string Role { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public ManufacturingSector ManufacturingSector { get; set; }
        [Display(Name = "Manufacturing Sub-Sector")]
        public Aerospace Aerospace { get; set; }
         public string? Purpose { get; set; }
        public string? ModelNo { get; set; }
        public string BussinessLicence { get; set; }
        public virtual UploadFile? UploadFile { get; set; }
        public virtual CreateUser? CreatedBy { get; set; }
        public virtual Case? Case { get; set; }



    }
}
