namespace mechanical.Models.Dto.MailDto
{
    public class MailPostDto
    {
        public required string  Subject { get; set; }
        public required string  Body { get; set; }
        public required string  SenderEmail { get; set; }
        public required string  SenderPassword { get; set; }
        public required string  RecipantEmail { get; set; }
    }
}
