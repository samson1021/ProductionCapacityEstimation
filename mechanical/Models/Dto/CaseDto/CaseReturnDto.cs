﻿using mechanical.Models.Entities;

namespace mechanical.Models.Dto.CaseDto
{
    public class CaseReturnDto
    {
        public Guid Id { get; set; }
        public required string CaseNo { get; set; }
        public required string ApplicantName { get; set; }
        public required string Segment { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerEmail { get; set; } = string.Empty;
        public required string District { get; set; }
        public DateTime CreationAt { get; set; }
        public required string CaseOriginator { get; set; }
        public required string Status { get; set; }
        public int TotalNoOfCollateral { get; set; } = 0;
        public virtual UploadFile? BussinessLicence { get; set; }
    }
}
