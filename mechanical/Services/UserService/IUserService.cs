using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.UserDto;

namespace mechanical.Services.UserService
{
    public interface IUserService
    {
        Task<UserReturnDto> GetUser(Guid id);
        Task<UserReturnDto> GetUser();
    }
}
