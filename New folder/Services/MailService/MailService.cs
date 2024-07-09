using mechanical.Models.Dto.MailDto;
using System.Net.Mail;

namespace mechanical.Services.MailService
{
    public class MailService : IMailService
    {
        public async Task<bool> SendEmail(MailPostDto mailPostDto)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(mailPostDto.SenderEmail);
                //message.From = new MailAddress("motherlife075@gmail.com");
                message.To.Add(mailPostDto.RecipantEmail);
                message.Subject = mailPostDto.Subject;
                message.Body = "<h3 style=\"color:purple\">" + "Case Valuation Schedule!: " + "</h3><ul><li style=\"color:black\">" + mailPostDto.Body + "</li></ul>";
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("mail.cbe.com.et", 587);
                //SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(mailPostDto.SenderEmail, mailPostDto.SenderPassword);
                //client.Credentials = new System.Net.NetworkCredential("motherlife075@gmail.com", "qgazypzgipyrfnuj");

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
    }
}
