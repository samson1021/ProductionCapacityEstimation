using mechanical.Models.Entities;
using mechanical.Models.Enum;
namespace mechanical.Models.Dto.CaseDto
{ 
    public class CaseCorrectionHistoryRetunDto
    {

        public Guid Id { get; set; }
        public string? CommentedFieldName { get; set; }
        public Guid? CollateralId { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid CommentByUserId { get; set; }
        public string? MessageType { get; set; }
        public string? Status { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CommentHistory CommentHistorys { get; set; }

    }
}