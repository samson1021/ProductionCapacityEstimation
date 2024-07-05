using mechanical.Models.Entities;
using mechanical.Models.Login;

namespace mechanical.Services.AuthenticatioinService
{
    public  interface IAuthenticationService
    {
        List<UserAdAttribute> GetUserDataAD(string domainName, string userName, string password, string EnterdUsername);

    }
}
