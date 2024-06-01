using mechanical.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities.ProductionCapacity
{
    public class CapacityEstimationCase
    {
          
     
     

        public Guid Id { get; set; }        
        public required string CapacityEstimationNo { get; set; }
        public required string ApplicantName { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime CreationAt { get; set; }
        public Guid DistrictId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public required Guid CaseOriginatorId { get; set; }
        public virtual ManufacturingCapacity? ManufacturingCapacity { get; set; }
        public virtual PlantCapacity? PlantCapacity { get; set; }
        public virtual District? District { get; set; }
        public virtual CreateUser? CaseOriginatord { get; set; }        
        

        //public virtual UploadFile? BussinessLicence { get; set; }
        //public virtual ICollection<Collateral>? Collaterals { get; set; }
        //[ForeignKey("BussinessLicence")]
        //public Guid? BussinessLicenceId { get; set; }
    }
}
