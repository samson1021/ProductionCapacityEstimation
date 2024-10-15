﻿namespace mechanical.Models.PCE.Entities
{
    public class ProductionReject
    {
        public Guid Id { get; set; }
        public required Guid PCEId { get; set; }
        public required Guid RejectedBy { get; set; }
        // public required Guid ReturnedBy { get; set; }
        public required string RejectionComment { get; set; }
        // public required string ReturnComment { get; set; }
        public DateTime CreationDate { get; set; }

        // public CreateUser ReturnedByUser { get; set; }
    }
}