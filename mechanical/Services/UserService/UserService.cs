using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.UserDto;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.UserService
{
    public class UserService:IUserService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadFileService _uploadFileService;
        public UserService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _uploadFileService = uploadFileService;
        }

        public async Task<UserReturnDto> GetUser(Guid id)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<UserReturnDto>(user);
        }

        public async Task<UserReturnDto> GetUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(c => c.Id == Guid.Parse(httpContext.Session.GetString("userId")));
            return _mapper.Map<UserReturnDto>(user);
        }

        public async Task<ReturnUserDto> GetUserById(Guid Id)
        {
            var user = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.Role).Include(res => res.District).FirstOrDefaultAsync(res => res.Id == Id);             
            return _mapper.Map<ReturnUserDto>(user);
        }       
        public async Task<IEnumerable<ReturnUserDto>> GetRMs(Guid userId)
        {
            var user = await _cbeContext.CreateUsers
                                     .AsNoTracking()
                                     .Where(u => u.Id == userId)
                                     .Select(u => new { u.SupervisorId, u.Department })
                                     .FirstOrDefaultAsync();

            if (user == null)
                return Enumerable.Empty<ReturnUserDto>();

            var rms = await _cbeContext.CreateUsers
                                        .AsNoTracking()
                                        // .Where(u => u.SupervisorId == user.SupervisorId
                                        //             && u.Department == user.Department
                                        //             && u.Role.Name == "Relation Manager"
                                        // )
                                        .Include(u => u.Role)
                                        .ToListAsync();
            return _mapper.Map<IEnumerable<ReturnUserDto>>(rms);
        }
    }
}
