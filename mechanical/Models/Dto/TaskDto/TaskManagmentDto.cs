namespace mechanical.Models.Dto.TaskDto
{
    public class TaskManagmentDto
    {
        //public Guid Id { get; set; }
        //public DateTime CreationAt { get; set; }
        //public required string CaseNo { get; set; }
        //public required string RequestingUnit { get; set; }
        //public required string Segement { get; set; }
        //public required string ApplicantName { get; set; }
        //public required string CustomerId { get; set; }
        //public string District {  get; set; } = string.Empty;
        //public required string Status { get; set; }
        //public int NoOfCollateral { get; set; } = 0;
        //public int TotalNoOfCollateral { get; set; } = 0;

        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid AssignedId { get; set; }
        public required Guid CaseOrginatorId { get; set; }
        public required string TaskName { get; set; }
        public string SharingReason { get; set; } = string.Empty;

        public required string TaskStatus { get; set; }



    }
}
