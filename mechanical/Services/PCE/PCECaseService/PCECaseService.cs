using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.PCE.PCECaseService
{
    public class PCECaseService:IPCECaseService
    {

        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PCECaseService> _logger;

        public PCECaseService(ILogger<PCECaseService> logger, IHttpContextAccessor httpContextAccessor, CbeContext cbeContext, IMapper mapper, IPCECaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _IPCECaseTimeLineService = caseTimeLineService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        async Task<PCECase> IPCECaseService.PCECase(Guid userId, PCECaseDto pCECaseDto)
        {
            var user = _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefault(res => res.Id == userId);
            var httpContext = _httpContextAccessor.HttpContext;
            var loanCase = _mapper.Map<PCECase>(pCECaseDto);
            loanCase.Id = Guid.NewGuid();
            loanCase.CurrentStage = "Relation Manager";
            loanCase.CurrentStatus = "New";
            loanCase.DistrictId = new System.Guid("C191D650-72A8-4204-BB00-9435DAA758A6");
            loanCase.DistrictId = user.DistrictId;
            loanCase.RMUserId = userId;
            loanCase.CreationDate = DateTime.Now;
            await _cbeContext.PCECases.AddAsync(loanCase);
            await _cbeContext.SaveChangesAsync();
            await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
            {
                CaseId = loanCase.Id,
                Activity = $"<strong>A new case with ID {loanCase.CaseNo} has been created</strong>",
                CurrentStage = "Relation Manager"
            });
            return loanCase;

        }


       // Task<IEnumerable<PCECaseDto>> GetPCENewCases(Guid userId);
        public async Task<IEnumerable<PCENewCaseDto>> GetPCENewCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager").ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return caseDtos;
        }


        public PCECaseReturntDto GetPCECase(Guid userId, Guid id)
        {
            try
            {
                _logger.LogInformation("in the service class");
                var result = _cbeContext.PCECases.Where(c => c.Id == id).FirstOrDefault();
                return _mapper.Map<PCECaseReturntDto>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " my error");
                throw;
            }
        }

        public async Task<PCECaseReturntDto> PCEEdit(Guid userId, PCECaseReturntDto caseDto)
        {
            var pceCase = await _cbeContext.PCECases.FirstOrDefaultAsync(c => c.Id == userId);

            if (pceCase != null)
            {
                pceCase.ApplicantName = caseDto.ApplicantName;
                pceCase.CustomerEmail = caseDto.CustomerEmail;
                pceCase.CustomerUserId = caseDto.CustomerUserId;

                await _cbeContext.SaveChangesAsync();

                return _mapper.Map<PCECaseReturntDto>(pceCase);
            }
            else
            {
                // If the PCECase is not found, return null
                return _mapper.Map<PCECaseReturntDto>(pceCase);
            }
        }


        public async Task<IEnumerable<PCENewCaseDto>> GetPCEPendingCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Where(res => res.CurrentStatus == "Pending" && res.CurrentStage == "Relation Manager").ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return caseDtos;
        }




        public async Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Where(res => res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager").ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return caseDtos;
        }



        public async Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Where(res => res.CurrentStage == "Relation Manager").ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return caseDtos;
        }

  

   
        public async Task<CreateNewCaseCountDto> GetDashboardPCSCaseCount()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return new CreateNewCaseCountDto()
            {
                PCSNewCaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.CurrentStage == "Relation Manager" && res.CurrentStatus == "New").CountAsync(),
                PCSPendingCaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.CurrentStage != "Relation Manager" && !(res.CurrentStage == "Checker Manager" && res.CurrentStatus == "Complete")).CountAsync(),
                PCSCompletedCaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.CurrentStage == "Checker Manager" && res.CurrentStatus == "New").CountAsync()
            };
        }

    }
}
