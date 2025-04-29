﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Services.PCE.PCECaseTimeLineService;
using Humanizer;

namespace mechanical.Services.PCE.PCECaseService
{
    public class PCECaseService:IPCECaseService
    {

        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PCECaseService> _logger;

        public PCECaseService(ILogger<PCECaseService> logger, IHttpContextAccessor httpContextAccessor, CbeContext cbeContext, IMapper mapper, IPCECaseTimeLineService PCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _IPCECaseTimeLineService = PCECaseTimeLineService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        async Task<PCECase> IPCECaseService.PCECase(Guid UserId, PCECaseDto pceCaseDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                var user = _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefault(res => res.Id == UserId);
                var pceCase = _mapper.Map<PCECase>(pceCaseDto);
                
                pceCase.Id = Guid.NewGuid();
                pceCase.Status = "New";
                pceCase.DistrictId = user.DistrictId;
                pceCase.PCECaseOriginatorId = UserId;
                pceCase.CreatedAt = DateTime.Now;
                //pceCase.Segment = "Retail";

                await _cbeContext.PCECases.AddAsync(pceCase);

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    PCECaseId = pceCase.Id,
                    Activity = $"<strong>A new PCE case with ID {pceCase.CaseNo} has been created</strong>",
                    CurrentStage = "Relation Manager"
                });   

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return pceCase; 
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PCE case");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating PCE case.");
            }

        }

        public async Task<PCECaseReturnDto> Edit(Guid Id, PCECaseReturnDto pceCaseDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var pceCase = await _cbeContext.PCECases.FirstOrDefaultAsync(c => c.Id == Id);

                if (pceCase != null)
                {
                    pceCase.ApplicantName = pceCaseDto.ApplicantName;
                    pceCase.CustomerEmail = pceCaseDto.CustomerEmail;
                    pceCase.CustomerId = pceCaseDto.CustomerId;
                    pceCase.Segment = pceCaseDto.Segment;
                    //_mapper.Map(pceCaseDto, pceCase);
                    //_cbeContext.PCECases.Update(pceCase);

                    await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                    {
                        PCECaseId = pceCase.Id,
                        Activity = $"<strong>The PCE case with case number {pceCase.CaseNo} has been edited</strong>",
                        CurrentStage = "Relation Manager"
                    });
                }                
                              
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                    
                return _mapper.Map<PCECaseReturnDto>(pceCase);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the PCE case");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating the PCE case.");
            }
        }

        public async Task<PCECaseReturnDto> GetPCECase(Guid UserId, Guid Id)
        {
            var assignedProductionCapacities = await _cbeContext.PCECaseAssignments
                                                                .AsNoTracking()
                                                                .Where(pca => pca.UserId == UserId && pca.ProductionCapacity.PCECaseId == Id)
                                                                .Select(pca => pca.ProductionCapacityId)
                                                                .ToListAsync();
     

            var pceCase = await _cbeContext.PCECases
                                            .AsNoTracking()
                                            .Include(res => res.District)
                                            .Include(res => res.BusinessLicense)
                                            .Include(res => res.ProductionCapacities
                                                .Where(pc => assignedProductionCapacities.Contains(pc.Id)))
                                            .FirstOrDefaultAsync(c => c.Id == Id && 
                                                (assignedProductionCapacities.Any() || c.PCECaseOriginatorId == UserId));

            return _mapper.Map<PCECaseReturnDto>(pceCase);
        }

        public async Task<IEnumerable<PCECaseReturnDto>> GetPCECases(Guid UserId, string Status = null, int? Limit = null)
        {
            var unfilteredQuery = _cbeContext.PCECaseAssignments
                                            .AsNoTracking()
                                            .Include(pca => pca.ProductionCapacity)
                                                .ThenInclude(pc => pc.PCECase)
                                            .Where(pca => pca.UserId == UserId);

            IQueryable<PCECaseAssignment> filteredQuery = unfilteredQuery;

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                filteredQuery = filteredQuery.Where(pca => pca.Status == Status || pca.Status.Contains(Status));
            }

            var distinctPCECases = await filteredQuery
                                                .GroupBy(pca => pca.ProductionCapacity.PCECase)
                                                .Select(p => new
                                                {
                                                    PCECase = p.Key,
                                                    LatestAssignmentDate = p.Max(x => x.AssignmentDate),
                                                    NoOfProductions = p.Count(),
                                                    TotalNoOfProductions = unfilteredQuery
                                                                            .Where(unfPca => unfPca.ProductionCapacity.PCECaseId == p.Key.Id)
                                                                            .Select(ca => ca.ProductionCapacity.Id)
                                                                            .Distinct()
                                                                            .Count()
                                                })
                                                .OrderByDescending(x => x.LatestAssignmentDate)
                                                .ToListAsync();

            var additionalPCECases = await _cbeContext.PCECases
                                                    .AsNoTracking()
                                                    .Where(pc => pc.PCECaseOriginatorId == UserId && !pc.ProductionCapacities.Any() &&
                                                                (string.IsNullOrEmpty(Status) ||
                                                                    Status.Equals("All", StringComparison.OrdinalIgnoreCase) ||
                                                                    Status.Equals("New", StringComparison.OrdinalIgnoreCase)))
                                                    .ToListAsync();

            var allPCECases = distinctPCECases
                                        .Select(x => x.PCECase)
                                        .Concat(additionalPCECases)
                                        .DistinctBy(pc => pc.Id)
                                        .OrderByDescending(pc => pc.CreatedAt)
                                        .ToList();

            var returnDtos = allPCECases.Select(pceCase =>
            {
                var dto = _mapper.Map<PCECaseReturnDto>(pceCase);

                var pceCaseCounts = distinctPCECases.FirstOrDefault(x => x.PCECase.Id == pceCase.Id);

                if (pceCaseCounts != null)
                {
                    dto.NoOfProductions = pceCaseCounts.NoOfProductions;
                    dto.TotalNoOfProductions = pceCaseCounts.TotalNoOfProductions;
                }
                else
                {
                    dto.NoOfProductions = 0;
                    dto.TotalNoOfProductions = 0;
                }

                return dto;
            });

            if (Limit.HasValue && Limit.Value > 0)
            {
                returnDtos = returnDtos.Take(Limit.Value).ToList();
            }

            return returnDtos.ToList();
        }

        public async Task<PCECaseCountDto> GetDashboardPCECaseCount(Guid UserId)
        {
            var statuses = new[] { "New", "Pending", "Completed", "Reestimate", "Reestimated", "Returned", "All" };
            // var tasks = statuses.Select(status => GetPCECaseCountAsync(UserId, status)).ToList();
            // var counts = await Task.WhenAll(tasks);

            var counts = new List<(int DistinctPCECaseCount, int ProductionCount)>();

            foreach (var status in statuses)
            {
                var count = await GetPCECaseCountAsync(UserId, status);
                counts.Add(count);
            }

            return new PCECaseCountDto()
            {
                NewPCECaseCount = counts[0].DistinctPCECaseCount,
                NewProductionCount = counts[0].ProductionCount,

                PendingPCECaseCount = counts[1].DistinctPCECaseCount,
                PendingProductionCount = counts[1].ProductionCount,

                CompletedPCECaseCount = counts[2].DistinctPCECaseCount,
                CompletedProductionCount = counts[2].ProductionCount,

                ResubmittedPCECaseCount = counts[3].DistinctPCECaseCount, // Reestimate
                ResubmittedProductionCount = counts[3].ProductionCount,

                ReestimatedPCECaseCount = counts[4].DistinctPCECaseCount,
                ReestimatedProductionCount = counts[4].ProductionCount,

                ReturnedPCECaseCount = counts[5].DistinctPCECaseCount,
                ReturnedProductionCount = counts[5].ProductionCount,

                TotalPCECaseCount = counts[6].DistinctPCECaseCount,
                TotalProductionCount = counts[6].ProductionCount,
            };
        }

        private async Task<(int DistinctPCECaseCount, int ProductionCount)> GetPCECaseCountAsync(Guid UserId, string status)
        {
            var assignmentsQuery = _cbeContext.PCECaseAssignments
                                            .AsNoTracking()
                                            .Where(res => res.UserId == UserId);

            if (!(string.IsNullOrEmpty(status) || status.Equals("All", StringComparison.OrdinalIgnoreCase)))
            {
                assignmentsQuery = assignmentsQuery.Where(res => res.Status == status);
            }

            var distinctPCECaseIds = await assignmentsQuery.Select(res => res.ProductionCapacity.PCECaseId).Distinct().ToListAsync();

            var productionCount = await assignmentsQuery.CountAsync();

            var additionalPCECaseCount = await _cbeContext.PCECases
                                                        .AsNoTracking()
                                                        .Where(pc => pc.PCECaseOriginatorId == UserId && !pc.ProductionCapacities.Any() &&
                                                                    (string.IsNullOrEmpty(status) ||
                                                                        status.Equals("All", StringComparison.OrdinalIgnoreCase) ||
                                                                        status.Equals("New", StringComparison.OrdinalIgnoreCase)))                                                       
                                                        .CountAsync();          
                                                    
            var distinctPCECaseCount = distinctPCECaseIds.Count + additionalPCECaseCount;

            return (distinctPCECaseCount, productionCount);
        }

        public async Task<IEnumerable<PCECaseReturnDto>> GetAssignedPCECases(Guid UserId)
        {
            var pceCaseAssignments = await _cbeContext.PCECaseAssignments
                                                        .AsNoTracking()
                                                        .Include(res => res.ProductionCapacity)
                                                            .ThenInclude(res => res.PCECase)
                                                        .Where(pca => pca.UserId == UserId && pca.Status != "Terminated")
                                                        .OrderByDescending(pca => pca.AssignmentDate)
                                                        .ToListAsync();
            var uniquePCECases = pceCaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var pceCaseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(uniquePCECases);
            
            foreach (var pceCaseDto in pceCaseDtos)
            {
                var uniqueProductions = pceCaseAssignments
                                                    .Where(ca => ca.ProductionCapacity.PCECaseId == pceCaseDto.Id)
                                                    .Select(ca => ca.ProductionCapacity)
                                                    .DistinctBy(c => c.Id);
                pceCaseDto.NoOfProductions = uniqueProductions
                                                    .Count(pc => pceCaseAssignments
                                                        .Any(pca => pca.ProductionCapacityId == pc.Id && pca.Status == "Pending"));
                pceCaseDto.TotalNoOfProductions = uniqueProductions.Count();
            }

            return pceCaseDtos;
        }

        public async Task<IEnumerable<PCECaseReturnDto>> GetLatestPCECases(Guid UserId)
        {
            var pceCases = await _cbeContext.PCECases
                                            .Include(x => x.District)
                                            .Include(res => res.BusinessLicense)
                                            .Include(x => x.ProductionCapacities)
                                            .Where(res => res.PCECaseOriginatorId == UserId)
                                            .OrderByDescending(res => res.CreatedAt)
                                            .Take(5)
                                            .ToListAsync();          

            return  _mapper.Map<IEnumerable<PCECaseReturnDto>>(pceCases);
        }
        
        public async Task<IEnumerable<PCECaseReturnDto>> GetPCECasesReport(Guid UserId)
        {
            var pceCases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStage == "Relation Manager"))
                .Where(res => res.PCECaseOriginatorId == UserId).ToListAsync();

            var pceCasesDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(pceCases);
            foreach (var pceCasesDto in pceCasesDtos)
            {
                pceCasesDto.NoOfProductions = _cbeContext.ProductionCapacities
                    .Where(pc => pc.PCECaseId == pceCasesDto.Id && pc.CurrentStage == "Relation Manager")
                    .Count();
            }
            foreach (var pceCasesDto in pceCasesDtos)
            {
                pceCasesDto.TotalNoOfProductions = _cbeContext.ProductionCapacities
                    .Where(pc => pc.PCECaseId == pceCasesDto.Id)
                    .Count();
            }
            return pceCasesDtos;
        }


        public async Task<PCEReportDataDto> GetPCECaseDetailReport(Guid UserId, Guid id)
        {
            try
            {
                var pceCaseResult = await _cbeContext.PCECases
                                                    .Include(res => res.District)
                                                    .Include(res => res.BusinessLicense)
                                                    .FirstOrDefaultAsync(c => c.Id == id && c.PCECaseOriginatorId == UserId);

                var productionCapacities = await _cbeContext.ProductionCapacities
                    .Where(pc => pc.PCECaseId == id && pc.CreatedById == UserId)
                    .ToListAsync();

                // Adjust this to fetch evaluations related to the current pce case
                var evaluations = await (from pc in _cbeContext.ProductionCapacities
                                         join pe in _cbeContext.PCEEvaluations.Include(e => e.Evaluator)
                                         on pc.Id equals pe.PCEId // Correct property
                                         where pc.PCECaseId == id
                                         select pe).ToListAsync();

                var pceCaseDto = new PCEReportDataDto
                {
                    PCECases = pceCaseResult,
                    Productions = productionCapacities,
                    PCEEvaluations = evaluations, // Use the evaluations retrieved from the join
                    PCECaseSchedule = null // Set to null since not used
                };

                return pceCaseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving PCE case detail report.");
                throw;
            }
        }

        public async Task<PCEReportDataDto> GetPCEReportData(Guid Id)
        {
            // the following code are used to access each production based on  Single pce
            var productions = await _cbeContext.ProductionCapacities
                                            .Where(res => res.Id == Id && res.CurrentStatus == "Completed" && 
                                                          res.CurrentStage == "Relation Manager")
                                            .ToListAsync();            
            var pceCase = _cbeContext.PCECases
                                    .Include(res => res.District)
                                    .Include(res => res.BusinessLicense)
                                    .Include(res => res.ProductionCapacities)
                                    .Where(c => productions.Select(p => p.PCECaseId).Contains(c.Id))
                                    .FirstOrDefault();
            //var pceEvaluations = await _cbeContext.PCEEvaluations
            //                                        .Include(e => e.TimeConsumedToCheck)
            //                                        .Include(res => res.Evaluator).Where(res => res.PCEId == Id).ToListAsync();

            // Get PCE evaluations with related data including production line evaluations
            var pceEvaluations = await _cbeContext.PCEEvaluations
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.Evaluator)
                                                .Include(e => e.productionLineEvaluations)
                                                    .ThenInclude(ple => ple.Evaluator)
                                                .Where(res => res.PCEId == Id)
                                                .ToListAsync();

            var pceCaseSchedule = await _cbeContext.PCECaseSchedules
                                                    .Where(res => res.PCECaseId == Id && res.Status == "Approved")
                                                    .FirstOrDefaultAsync();                    

            return new PCEReportDataDto
            {
                PCECases = pceCase,
                Productions = productions,
                PCEEvaluations = pceEvaluations,
                PCECaseSchedule = pceCaseSchedule,
                ProductionLineEvaluations = pceEvaluations
                                             .SelectMany(e => e.productionLineEvaluations ?? Enumerable.Empty<ProductionLineEvaluation>())
                                             .ToList()
            };


        }
        public async Task<PCEReportDataDto> GetPCEAllReportData(Guid Id)
        {

            var pceCase = _cbeContext.PCECases.Where(c => c.Id==Id).FirstOrDefault();
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == Id).ToListAsync();
            //var pceEvaluations = await _cbeContext.PCEEvaluations
            //                                    .Include(e => e.TimeConsumedToCheck)
            //                                    .Where(c=>productions.Select(d=>d.Id).Contains(c.PCEId)).ToListAsync();
            var pceEvaluations = await _cbeContext.PCEEvaluations
                                    .Include(e => e.TimeConsumedToCheck)
                                    .Include(e => e.Evaluator)
                                    .Include(e => e.productionLineEvaluations)
                                        .ThenInclude(ple => ple.Evaluator)
                                    .Where(res => res.PCEId == Id)
                                    .ToListAsync();

            var pceCaseSchedule = await _cbeContext.PCECaseSchedules.Where(res => res.PCECaseId == Id && res.Status == "Approved").FirstOrDefaultAsync();
            return new PCEReportDataDto
            {
                PCECases = pceCase,
                Productions = productions,
                PCEEvaluations = pceEvaluations,
                PCECaseSchedule = pceCaseSchedule,
                                ProductionLineEvaluations = pceEvaluations
                                             .SelectMany(e => e.productionLineEvaluations ?? Enumerable.Empty<ProductionLineEvaluation>())
                                             .ToList()
            };
        }
        
        public async Task<IEnumerable<PCECaseReturnDto>> GetRemarkedPCECases(Guid UserId)
        {
            var pceCaseAssignments = await _cbeContext.PCECaseAssignments
                                                        .Include(res => res.ProductionCapacity)
                                                        .ThenInclude(res => res.PCECase)
                                                        // .ThenInclude(res => res.PCECaseOriginator)
                                                        .Where(pca => pca.UserId == UserId && pca.Status.Contains("Remark"))
                                                        .ToListAsync();
            var uniquePCECases = pceCaseAssignments.Select(pca => pca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var pceCaseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(uniquePCECases);
            
            foreach (var pceCaseDto in pceCaseDtos)
            {
                pceCaseDto.NoOfProductions = pceCaseAssignments.Count(pca => pca.ProductionCapacity.PCECaseId == pceCaseDto.Id && pca.Status.Contains("Remark"));
                pceCaseDto.TotalNoOfProductions = pceCaseAssignments.Count(pca => pca.ProductionCapacity.PCECaseId == pceCaseDto.Id);
            }
            return pceCaseDtos;
        }
    }
}