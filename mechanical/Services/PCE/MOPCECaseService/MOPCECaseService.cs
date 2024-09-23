using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.PCECaseAssignmentDto;
using mechanical.Services.UploadFileService;
using mechanical.Services.PCE.PCEEvaluationService;

namespace mechanical.Services.PCE.MOPCECaseService
{
    public class MOPCECaseService : IMOPCECaseService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<MOPCECaseService> _logger;
        private readonly IUploadFileService _UploadFileService;
        private readonly IPCEEvaluationService _PCEEvaluationService;

        public MOPCECaseService(CbeContext context, IMapper mapper, ILogger<MOPCECaseService> logger, IUploadFileService UploadFileService, IPCEEvaluationService PCEEvaluationService)
        {
            _cbeContext = context;
            _mapper = mapper;
            _logger = logger;
            _UploadFileService = UploadFileService;
            _PCEEvaluationService = PCEEvaluationService;
        }

        ///////// PCE Case //////////////
        public async Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id)
        {
            var pCECase = await _cbeContext.PCECases
                                            .AsNoTracking()
                                            .Include(res => res.BussinessLicence)
                                            .Include(res => res.District)
                                            .Include(res => res.ProductionCapacities)
                                            .FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<PCECaseReturntDto>(pCECase);
        }
        
        public async Task<IEnumerable<PCENewCaseDto>> GetPCECases(Guid UserId, string Status = null, int? Limit = null)
        {
            var PCECaseAssignmentsQuery = _cbeContext.PCECaseAssignments
                                                    .AsNoTracking()
                                                    .Include(ca => ca.ProductionCapacity)
                                                    .ThenInclude(pc => pc.PCECase)
                                                    .Where(ca => ca.UserId == UserId);

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                PCECaseAssignmentsQuery = PCECaseAssignmentsQuery.Where(ca => ca.Status == Status);
            }

            var PCECaseAssignments = await PCECaseAssignmentsQuery.ToListAsync();
            var UniquePCECases = PCECaseAssignments
                                .Select(ca => ca.ProductionCapacity.PCECase)
                                .DistinctBy(c => c.Id)
                                .OrderByDescending(c => c.CreationDate)
                                .ToList();

            var productionCapacities = await _cbeContext.ProductionCapacities
                                                        .AsNoTracking()
                                                        .Where(pc => UniquePCECases.Select(c => c.Id).Contains(pc.PCECaseId) &&
                                                                    _cbeContext.PCECaseAssignments
                                                                                .Any(ca => ca.ProductionCapacityId == pc.Id && ca.UserId == UserId))
                                                        .ToListAsync();

            var returnDtos = UniquePCECases.Select(pceCase =>
            {
                var Dto = _mapper.Map<PCENewCaseDto>(pceCase);
                Dto.NoOfCollateral = string.IsNullOrEmpty(Status) || Status.Equals("All", StringComparison.OrdinalIgnoreCase)
                                    ? productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id)
                                    : productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id && pc.CurrentStatus == Status);
                Dto.TotalNoOfCollateral = productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id);
                return Dto;
            });
            if (Limit.HasValue && Limit.Value > 0)
            {
                returnDtos = returnDtos.Take(Limit.Value);
            }

            return returnDtos.ToList();
        }


        public async Task<PCECasesCountDto> GetDashboardPCECasesCount(Guid UserId)
        {
            var allPCEs = await _cbeContext.PCECaseAssignments
                                            .AsNoTracking()
                                            .Include(res => res.ProductionCapacity)
                                            .Where(res => res.UserId == UserId)
                                            .ToListAsync();

            var newPCEs = allPCEs.Where(res => res.Status == "New").ToList();
            var pendingPCEs = allPCEs.Where(res => res.Status == "Pending").ToList();
            var completedPCEs = allPCEs.Where(res => res.Status == "Completed").ToList();
            var reestimatedPCEs = allPCEs.Where(res => res.Status == "Reestimated").ToList();
            var resubmittedPCEs = allPCEs.Where(res => res.Status == "Reestimate").ToList();
            var rejectedPCEs = allPCEs.Where(res => res.Status == "Rejected").ToList();

            return new PCECasesCountDto()
            {
                NewPCECasesCount = newPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                NewPCEsCount = newPCEs.Count,

                PendingPCECasesCount = pendingPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                PendingPCEsCount = pendingPCEs.Count,

                CompletedPCECasesCount = completedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                CompletedPCEsCount = completedPCEs.Count,

                ReestimatedPCECasesCount = reestimatedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                ReestimatedPCEsCount = reestimatedPCEs.Count,

                ResubmittedPCECasesCount = resubmittedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                ResubmittedPCEsCount = resubmittedPCEs.Count,

                RejectedPCECasesCount = rejectedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                RejectedPCEsCount = rejectedPCEs.Count,

                TotalPCECasesCount = allPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                TotalPCEsCount = allPCEs.Count,
            };
        }

        //////////////////////////////// PCE /////////////////////////////////////////

        public async Task<IEnumerable<ReturnProductionDto>> GetPCEs(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            // var query = _cbeContext.ProductionCapacities.AsNoTracking()
            //                                             .Where(pc => pc.PCECaseAssignments
            //                                             .Any(pca => pca.UserId == UserId || pc.EvaluatorUserID == UserId));

            var query = _cbeContext.ProductionCapacities
                                    .AsNoTracking()
                                    .Include(pc => pc.PCECase)
                                    .Join(
                                        _cbeContext.PCECaseAssignments,
                                        pc => pc.Id,
                                        pca => pca.ProductionCapacityId,
                                        (pc, pca) => new { ProductionCapacity = pc, PCECaseAssignment = pca }
                                        )
                                        .Where(x => (x.PCECaseAssignment.UserId == UserId || x.ProductionCapacity.EvaluatorUserID == UserId)
                                                && (Status == null || Status == "All" || x.PCECaseAssignment.Status == Status))
                                    .Select(x => x.ProductionCapacity); 

            if (PCECaseId.HasValue)
            {
                query = query.Where(pc => pc.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(pc => pc.CurrentStage == Stage);
            }

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(pc => pc.CurrentStatus == Status);
            }
            else
            {
                query = query.Where(pc => pc.CurrentStatus != "Rejected");
            }

            var productions = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        // public async Task<int> GetPCEsCount(Guid UserId, Guid? PCECaseId, string Stage = null, string Status = null)
        // {
        //     return (await GetPCEs(UserId, PCECaseId, Stage, Status)).Count();
        // }

        public async Task<int> GetPCEsCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            var query = _cbeContext.ProductionCapacities
                .AsNoTracking()
                .Join(
                    _cbeContext.PCECaseAssignments,
                    pc => pc.Id,
                    pca => pca.ProductionCapacityId,
                    (pc, pca) => new { ProductionCapacity = pc, PCECaseAssignment = pca }
                )
                .Where(x => (x.PCECaseAssignment.UserId == UserId || x.ProductionCapacity.EvaluatorUserID == UserId)
                        && (Status == null || Status == "All" || x.PCECaseAssignment.Status == Status))
                .Select(x => x.ProductionCapacity); 

            if (PCECaseId.HasValue)
            {
                query = query.Where(x => x.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(x => x.CurrentStage == Stage);
            }

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.CurrentStatus == Status);
            }
            else
            {
                query = query.Where(pc => pc.CurrentStatus != "Rejected");
            }

            return await query.CountAsync();
        }

        public async Task<PCEsCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null)
        {
            var Statuses = new[] { "New", "Pending", "Completed", "Reestimate", "Reestimated" };
            var tasks = Statuses.Select(Status => GetPCEsCountAsync(UserId, PCECaseId, Stage, Status)).ToList();

            var counts = await Task.WhenAll(tasks);

            return new PCEsCountDto()
            {
                NewPCEsCount = counts[0],
                PendingPCEsCount = counts[1],
                CompletedPCEsCount = counts[2],
                ResubmittedPCEsCount = counts[3],
                ReestimatedPCEsCount = counts[4],
                TotalPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage)
            };
        }
    
        // public async Task<IEnumerable<ReturnProductionDto>> GetReturnedPCEs(Guid UserId)
        // {
       
        //     var rejectedCapacitiesQuery = from reject in _cbeContext.ProductionRejects
        //                                   join capacity in _cbeContext.ProductionCapacities
        //                                   on reject.PCEId equals capacity.Id
        //                                   // .AsNoTracking()
        //                                   where reject.RejectedBy == UserId
        //                                   select capacity;

        //     var rejectedCapacities = await rejectedCapacitiesQuery.ToListAsync();


        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(rejectedCapacities); ;
        // }

        public async Task<PCEDetailDto> GetPCEDetails(Guid UserId, Guid PCEId)
        {

            var pce = await _cbeContext.ProductionCapacities.AsNoTracking().Include(pc => pc.PCECase).FirstOrDefaultAsync(res => res.Id == PCEId);
            var reestimation = await _cbeContext.ProductionCapacityReestimations.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId); 
            var rejectedProduction = await _cbeContext.ProductionRejects.AsNoTracking().FirstOrDefaultAsync(res => res.PCEId == PCEId);
            var relatedFiles = await _UploadFileService.GetUploadFileByCollateralId(PCEId);          
            var valuationHistory = await GetValuationHistory(UserId, PCEId);
     
            var remark = pce;   
            CreateUser user = null;     
            if (rejectedProduction != null)
            {
                user = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.Role).FirstOrDefaultAsync(rea => rea.Id == rejectedProduction.RejectedBy);
            }

            return new PCEDetailDto
            {
                PCECase = pce.PCECase,
                ProductionCapacity = _mapper.Map<ReturnProductionDto>(pce),
                PCEValuationHistory = valuationHistory,
                Reestimation = reestimation,
                RelatedFiles = relatedFiles,
                RejectedProduction = rejectedProduction,
                RejectedBy = user

            };
        }                       
    
        public async Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId)
        {
            var pce = await _cbeContext.ProductionCapacities.AsNoTracking().FirstOrDefaultAsync(res => res.Id == PCEId);                 
            
            PCEEvaluationReturnDto latestEvaluation = null;

            if (pce != null && pce.CurrentStatus != "New" && pce.CurrentStatus != "Reestimate")
            {  
                latestEvaluation = await _PCEEvaluationService.GetPCEEvaluationByPCEId(UserId, PCEId);
            }

            var previousEvaluations = await _cbeContext.PCEEvaluations
                                                        .AsNoTracking()
                                                        .Include(p => p.PCE)
                                                        .ThenInclude(pc => pc.PCECase)
                                                        .Where(res => res.PCEId == PCEId && (latestEvaluation == null || res.Id != latestEvaluation.Id))
                                                        .ToListAsync();
            
            return new PCEValuationHistoryDto
            {
                LatestEvaluation = latestEvaluation,
                PreviousEvaluations = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(previousEvaluations)
            };
        }           

        public async Task<IEnumerable<PCENewCaseDto>> GetRemarkedPCECases(Guid UserId)
        {

            var PCECaseAssignments = await _cbeContext.PCECaseAssignments
                                                        .Include(res => res.ProductionCapacity)
                                                        .ThenInclude(res => res.PCECase)
                                                        // .ThenInclude(res => res.PCECaseOriginator)
                                                        .Where(pca => pca.UserId == UserId && pca.Status == "Remark")
                                                        .ToListAsync();
            var uniquePCECases = PCECaseAssignments.Select(pca => pca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var PCECaseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(uniquePCECases);
            foreach (var PCECaseDto in PCECaseDtos)
            {
                PCECaseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == PCECaseDto.Id);
            }
            return PCECaseDtos;
        }

        public async Task<CreateUser> GetUser(Guid UserId)
        {
            var user = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.Role).Include(res => res.District).FirstOrDefaultAsync(res => res.Id == UserId);             
            return _mapper.Map<CreateUser>(user);
        }

    }
}