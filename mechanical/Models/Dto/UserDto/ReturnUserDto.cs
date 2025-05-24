using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.Dto.UserDto
{
    public class ReturnUserDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string emp_ID { get; set; }
        public required string Email { get; set; }
        public required string PhoneNO { get; set; }
        public string? Branch { get; set; }
        public string Department { get; set; }
        public string? RoleName { get; set; }
        public string? DistrictName { get; set; }
        
        public Guid RoleId { get; set; }   
        public Guid DistrictId { get; set; }
        public Guid? SupervisorId { get; set; }

        public virtual User? Supervisor { get; set; }
        public virtual District? District { get; set; }
        public virtual Role? Role { get; set; }
        
    }
}
