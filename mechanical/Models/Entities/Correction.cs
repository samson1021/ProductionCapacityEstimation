using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(CaseId))]
    public class Correction
    {
        [Key]
        public Guid Id { get; set; } 
        public Guid CaseId { get; set; } = Guid.Empty;
        public Guid CollateralID { get; set; }= Guid.Empty;
        public Guid EquipmentId { get; set; } = Guid.Empty;  
        public string Comment { get; set; }=string.Empty;
        public string CommentedAttribute { get; set; }=string.Empty;
        public Guid CommentedByUserId { get; set; }= Guid.Empty;
        public virtual User? CommentedByUserIds { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
