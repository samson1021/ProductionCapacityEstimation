using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.MailDto;

namespace mechanical.Services.MailService
{
    public interface IMailService
    {
        Task<bool> SendEmail(MailPostDto mailPostDto);

    }
}
