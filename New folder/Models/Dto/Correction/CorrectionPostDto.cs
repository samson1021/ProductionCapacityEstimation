﻿using mechanical.Models.Entities;

namespace mechanical.Models.Dto.Correction
{
    public class CorrectionPostDto
    {
        public Guid Id { get; set; }=Guid.NewGuid();
        public Guid CollateralID { get; set; } 
        public Guid EquipmentId { get; set; } 
        public string Comment { get; set; }
        public string CommentedAttribute { get; set; } 
    }
}

