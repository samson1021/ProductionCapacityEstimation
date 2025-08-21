using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Enum;
namespace mechanical.Models.Entities
{
    [Index(nameof(SupervisorId))]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string emp_ID { get; set; }
        public required string Email { get; set; }
        public required string PhoneNO { get; set; }
        public string? Branch { get; set; }
        public Guid RoleId { get; set; }
        public Guid DistrictId { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
        public Guid? SupervisorId { get; set; }
        public string? title { get; set; }
        public string? company { get; set; }
        public Segment? BroadSegment { get; set; }
        public SubSegment? Unit { get; set; }

        [ForeignKey("SupervisorId")]
        public virtual User? Supervisor { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District? District { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        public virtual Signatures? Signatures { get; set; }
    }
}
