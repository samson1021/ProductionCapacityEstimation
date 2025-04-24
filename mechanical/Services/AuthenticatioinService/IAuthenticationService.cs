using mechanical.Models.Entities;
using mechanical.Models.Login;

namespace mechanical.Services.AuthenticatioinService
{
    public  interface IAuthenticationService
    {
        UserAdAttribute GetUserDataAD(string EnterdUsername);
        UserAdAttribute GetEmployeeInfo(string EmpId);
        bool AuthenticateUserByAD(string EnterdUsername, string password);

    }
}
