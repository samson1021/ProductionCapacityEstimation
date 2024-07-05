﻿using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PCECaseTimeLineDto
{
    public class PCECaseTimeLineReturnDto
    {
     
        public required Guid CaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }
        public virtual CreateUser? User { get; set; }

    }
}
