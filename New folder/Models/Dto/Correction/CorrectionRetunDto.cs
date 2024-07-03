namespace mechanical.Models.Dto.Correction
{
    public class CorrectionRetunDto
    {
        public Guid CaseId { get; set; }
        public  Guid CollateralID { get; set; }
        public  Guid EquipmentId { get; set; }
        public  Guid CommentedByUserId { get; set; }
        public  string? Comment { get; set; }
        public  string? CommentedAttribute { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
