﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace mechanical.Models.Entities
{
    public class CreateUser
    {
        //internal object Districts;

        [Key]
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string emp_ID { get; set; }
        public required string Email { get; set; }
        public required string PhoneNO { get; set; }
        public string? Branch { get; set; }
        [ForeignKey("CreateRole")]
        public Guid RoleId { get; set; }
        [ForeignKey("District")]
        public Guid DistrictId { get; set; }
        public string Department { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
        public Guid? SupervisorId { get; set; }

        public virtual CreateUser? Supervisor { get; set; }
        public virtual District? District { get; set; }
        public virtual CreateRole? Role { get; set; }
        
    }
}
