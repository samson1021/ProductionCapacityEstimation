

using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;
using Microsoft.AspNetCore.Http;
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
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager"))
                .Where(res => res.RMUserId == userId && res.CurrentStatus == "New").ToListAsync();

            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.NoOfCollateral = _cbeContext.ProductionCapacities
                    .Where(pc => pc.PCECaseId == caseDto.Id && pc.CurrentStage == "Relation Manager")
                    .Count();
            }
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = _cbeContext.ProductionCapacities
                    .Where(pc => pc.PCECaseId == caseDto.Id)
                    .Count();
            }
            return caseDtos;
        }


        public async Task<IEnumerable<PCENewCaseDto>> GetPCEPendingCases(Guid userId)
        {


            //var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager"))
            //   .Where(res => res.RMUserId == userId && res.CurrentStatus == "New").ToListAsync();
            //var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);

            //foreach (var caseDto in caseDtos)
            //{      
            //    var PendingNoOfCollaterals = 0;
            //    var cols = await _cbeContext.ProductionCapacities.Where(c => c.PCECaseId == caseDto.Id).ToListAsync();
            //    foreach(var col in cols)
            //    {
            //        var pendingCount =  _cbeContext.ProductionCaseAssignments
            //                .Where(pc => pc.UserId == userId && pc.ProductionCapacityId == col.Id && pc.Status == "Pending")
            //                .ToListAsync();
            //        PendingNoOfCollaterals = PendingNoOfCollaterals+ pendingCount.Count();
            //    }
            //    caseDto.TotalNoOfCollateral = _cbeContext.ProductionCapacities
            //        .Where(pc => pc.PCECaseId == caseDto.Id)
            //        .Count();

            //    caseDto.NoOfCollateral = PendingNoOfCollaterals;
            //}
            //return caseDtos;
            //PendingPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => (coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete") && coll.CurrentStage != "Relation Manager")).CountAsync(),
            //    PendingPCECollateralCount = await _cbeContext.ProductionCapacities.Where(coll => coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete" && coll.CurrentStage != "Relation Manager").CountAsync(),
            //    CompletedPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => coll.CurrentStage == "Relation Manager" && coll.CurrentStatus == "Complete")).CountAsync(),
            //    CompletedPCECollateralCount = await _cbeContext.ProductionCapacities.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "Complete").CountAsync(),
            //    TotalPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId).CountAsync(),
            //    TotalPCECollateralCount = await _cbeContext.ProductionCapacities.Where(res => res.CreatedById == userId).CountAsync(),

            //casedto

            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => ( res.CurrentStage != "Relation Manager")&&((res.CurrentStatus != "Complete" && res.CurrentStage != "Checker Officer"))))
                       .Where(res => res.RMUserId == userId && (res.ProductionCapacities.Any(collateral => ( collateral.CurrentStage != "Relation Manager") && ((collateral.CurrentStatus != "Complete" && collateral.CurrentStage != "Checker Officer")))))

                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
            }
            return caseDtos;
        }










        public PCECaseReturntDto GetPCECase(Guid userId, Guid id)
        {

            try
            {
                var result = _cbeContext.PCECases.Include(res => res.District)
                    .Include(res=>res.ProductionCapacities).Include(res=>res.BussinessLicence)
                    .Where(c => c.Id == id && c.RMUserId==userId).FirstOrDefault();
                var lastResult = _mapper.Map<PCECaseReturntDto>(result);
                return lastResult;

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





        public async Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid userId)
        {
            //var cases = await _cbeContext.PCECases.Where(res => res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager").ToListAsync();
            //var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            //return caseDtos;
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer"))
           .Where(res => res.RMUserId == userId && (res.ProductionCapacities.Any(res => res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer"))).ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
            }
            return caseDtos;
        }



        public async Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Where(res => res.CurrentStage == "Relation Manager").ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return caseDtos;
        }

  

   
        public async Task<CreateNewCaseCountDto> GetDashboardPCECaseCount(Guid userId)
        {
            return new CreateNewCaseCountDto()
            {
                //PCSNewCaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.CurrentStage == "Relation Manager" && res.CurrentStatus == "New").CountAsync(),
                //PCSPendingCaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.CurrentStage != "Relation Manager" && !(res.CurrentStage == "Checker Manager" && res.CurrentStatus == "Complete")).CountAsync(),
                //PCSCompletedCaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.CurrentStage == "Checker Manager" && res.CurrentStatus == "New").CountAsync()
                NewPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New")).CountAsync(),
                NewPCECollateralCount = await _cbeContext.ProductionCapacities.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New").CountAsync(),
                PendingPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => (coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete") && coll.CurrentStage != "Relation Manager")).CountAsync(),
                PendingPCECollateralCount = await _cbeContext.ProductionCapacities.Where(coll => coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete" && coll.CurrentStage != "Relation Manager").CountAsync(),
                CompletedPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => coll.CurrentStage == "Relation Manager" && coll.CurrentStatus == "Complete")).CountAsync(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCapacities.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "Complete").CountAsync(),
                TotalPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId).CountAsync(),
                TotalPCECollateralCount = await _cbeContext.ProductionCapacities.Where(res => res.CreatedById == userId).CountAsync(),
            };

        }
        public async Task<CreateNewCaseCountDto> GetMyDashboardCaseCount()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            // var NewCollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            // var PendCollateral = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Status == "Pending").ToListAsync();
            var CompCollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.Status == "Complete").ToListAsync();
           // var totalcollatera = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId).ToListAsync();

            return new CreateNewCaseCountDto()
            {
                //NewCaseCount = NewCollateral.Select(res => res.Collateral.CaseId).Distinct().Count(),
                //NewCollateralCount = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.Status == "New").CountAsync(),

                //PendingCaseCount = PendCollateral.Select(res => res.Collateral.CaseId).Distinct().Count(),
                //PendingCollateralCount = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.Status == "Pending").CountAsync(),

                PCSCompletedCaseCount = CompCollateral.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.Status == "Complete").CountAsync(),

                //TotalCaseCount = totalcollatera.Select(res => res.Collateral.CaseId).Distinct().Count(),           
                //TotalCollateralCount = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId).CountAsync(),
           
            };
        }
        //abdu end
        public async Task<PCECaseReturntDto> GetProductionCaseDetail(Guid id)
        {
            var loanCase = await _cbeContext.PCECases
                            .Include(res => res.BussinessLicence).Include(res => res.District).Include(res => res.ProductionCapacities)
                            .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<PCECaseReturntDto>(loanCase);
        }

        public async Task<PCECaseReturntDto> GetCase(Guid userId, Guid id)
        {
            var loanCase = await _cbeContext.PCECases
                           .Include(res => res.BussinessLicence).Include(res => res.District).Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == id && c.RMUserId == userId);
            return _mapper.Map<PCECaseReturntDto>(loanCase);
        }

       
    }
}
