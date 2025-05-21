namespace mechanical.Models.Dto
{
    public class ResultDto
    {
        public required int StatusCode { get; set; }
        public required bool Success { get; set; }
        public required string Message { get; set; }
    }
}
