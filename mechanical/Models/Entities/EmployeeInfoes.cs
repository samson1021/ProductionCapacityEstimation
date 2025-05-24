using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Entities
{
    public class EmployeeInfoes
    {
        [Key]
        public int Id { get; set; }
        public string emp_ID { get; set; }=string.Empty;
        public string location { get; set; }=string.Empty;
        public string locationId { get; set; }=string.Empty;
        public string position { get; set; }=string.Empty;
        public string pr_Number { get; set; } = string.Empty;
        public string supervisorEmpId { get; set; } = string.Empty;
        public string supervisorID { get; set; } = string.Empty;
        public string supervisor_name { get; set; } = string.Empty;
    }
}
