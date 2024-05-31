﻿using System.ComponentModel.DataAnnotations;

namespace Signature_management.Model.Entities
{
    public class Signatures
    {
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Emp_Id { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set;} = DateTime.Now;
        public string  CreatedBy { get; set;}= string.Empty;
        public string SignatureBase64String { get; set;}= string.Empty;
        public Guid SignatureFileId { get; set; }
    }
}
