﻿using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Services.PCE.PCECaseTimeLineService;

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
                pceCase.RMUserId = UserId;
                pceCase.CreationDate = DateTime.Now;

                await _cbeContext.PCECases.AddAsync(pceCase);

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pceCase.Id,
                    Activity = $"<strong>A new case with ID {pceCase.CaseNo} has been created</strong>",
                    CurrentStage = "Relation Manager"
                });   

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return pceCase; 
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating case.");
            }

        }

        public async Task<PCECaseReturnDto> Edit(Guid UserId, PCECaseReturnDto caseDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var pceCase = await _cbeContext.PCECases.FirstOrDefaultAsync(c => c.Id == UserId);

                if (pceCase != null)
                {
                    pceCase.ApplicantName = caseDto.ApplicantName;
                    pceCase.CustomerEmail = caseDto.CustomerEmail;
                    pceCase.CustomerUserId = caseDto.CustomerUserId;

                    await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                    {
                        CaseId = pceCase.Id,
                        Activity = $"<strong>The case with case number {pceCase.CaseNo} has been edited</strong>",
                        CurrentStage = "Relation Manager"
                    });
                }                
                              
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                    
                return _mapper.Map<PCECaseReturnDto>(pceCase);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the case");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating the case.");
            }
        }

        public async Task<PCECaseReturnDto> GetPCECase(Guid UserId, Guid Id)
        {
            var assignedProductionCapacities = await _cbeContext.PCECaseAssignments
                                                                .AsNoTracking()
                                                                .Where(pca => pca.UserId == UserId && pca.ProductionCapacity.PCECaseId == Id)
                                                                .Select(pca => pca.ProductionCapacityId)
                                                                .ToListAsync();

            // if (!assignedProductionCapacities.Any())
            // {
            //     return null; 
            // }

            var pceCase = await _cbeContext.PCECases
                                            .AsNoTracking()
                                            .Include(res => res.BussinessLicence)
                                            .Include(res => res.District)
                                            .Include(res => res.ProductionCapacities
                                                .Where(pc => assignedProductionCapacities.Contains(pc.Id)))
                                            .FirstOrDefaultAsync(c => c.Id == Id && 
                                                (assignedProductionCapacities.Any() || c.RMUserId == UserId));

            return _mapper.Map<PCECaseReturnDto>(pceCase);
        }

        public async Task<IEnumerable<PCECaseReturnDto>> GetPCECases(Guid UserId, string Status = null, int? Limit = null)
        {            
            var pceCaseAssignments = await _cbeContext.PCECaseAssignments
                                                    .AsNoTracking()
                                                    .Include(pca => pca.ProductionCapacity)
                                                        .ThenInclude(pc => pc.PCECase)
                                                    .Where(pca => pca.UserId == UserId)       
                                                    .ToListAsync();
              
            //  res.CurrentStatus.Contains("Remark") && res.CurrentStage == "Maker Officer")                                      
            // if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            // {
            //     pceCaseAssignmentsQuery = pceCaseAssignmentsQuery.Where(ca => ca.Status == Status);
            // }
            // var pceCaseAssignments = await pceCaseAssignmentsQuery .ToListAsync();

            var uniquePCECases = pceCaseAssignments
                                                .Select(pca => pca.ProductionCapacity.PCECase)
                                                .DistinctBy(pc => pc.Id)
                                                .OrderByDescending(pc => pc.CreationDate)
                                                .ToList();

            var additionalPCECases = await _cbeContext.PCECases
                                                    .AsNoTracking()
                                                    .Where(pc => pc.RMUserId == UserId && !pc.ProductionCapacities.Any() &&
                                                                (string.IsNullOrEmpty(Status) ||                                                                 
                                                                Status.Equals("All", StringComparison.OrdinalIgnoreCase) || 
                                                                Status.Equals("New", StringComparison.OrdinalIgnoreCase)))
                                                    .ToListAsync();

            var filteredPCECases = uniquePCECases
                                                .Where(pceCase => pceCaseAssignments
                                                    .Any(pca => pca.ProductionCapacity.PCECaseId == pceCase.Id && 
                                                            (string.IsNullOrEmpty(Status) || 
                                                            Status.Equals("All", StringComparison.OrdinalIgnoreCase) || 
                                                            pca.Status == Status || pca.Status.Contains(Status))))
                                                .Concat(additionalPCECases)
                                                .DistinctBy(pc => pc.Id)
                                                .ToList();

            var returnDtos = filteredPCECases.Select(pceCase =>
            {
                var Dto = _mapper.Map<PCECaseReturnDto>(pceCase);
                // Dto.NoOfCollateral = string.IsNullOrEmpty(Status) || Status.Equals("All", StringComparison.OrdinalIgnoreCase)
                //                     ? productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id)
                //                     : productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id && pc.CurrentStatus == Status);
                // Dto.TotalNoOfCollateral = productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id);
                Dto.NoOfCollateral = pceCaseAssignments.Count(pca => pca.ProductionCapacity.PCECaseId == pceCase.Id && 
                                                                        (string.IsNullOrEmpty(Status) || 
                                                                        Status.Equals("All", StringComparison.OrdinalIgnoreCase) || 
                                                                        pca.Status == Status || pca.Status.Contains(Status)));
                Dto.TotalNoOfCollateral = pceCaseAssignments.Count(pca => pca.ProductionCapacity.PCECaseId == pceCase.Id);
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
            var statuses = new[] { "New", "Pending", "Completed", "Reestimate", "Reestimated", "Rejected" };
            
            var tasks = statuses.Select(status => GetPCECasesCountAsync(UserId, status)).ToList();

            var counts = await Task.WhenAll(tasks);

            return new PCECasesCountDto()
            {
                NewPCECasesCount = counts[0].DistinctPCECaseCount,
                NewProductionsCount = counts[0].ProductionsCount,

                PendingPCECasesCount = counts[1].DistinctPCECaseCount,
                PendingProductionsCount = counts[1].ProductionsCount,

                CompletedPCECasesCount = counts[2].DistinctPCECaseCount,
                CompletedProductionsCount = counts[2].ProductionsCount,

                ResubmittedPCECasesCount = counts[3].DistinctPCECaseCount, // Reestimate
                ResubmittedProductionsCount = counts[3].ProductionsCount,

                ReestimatedPCECasesCount = counts[4].DistinctPCECaseCount,
                ReestimatedProductionsCount = counts[4].ProductionsCount,

                RejectedPCECasesCount = counts[5].DistinctPCECaseCount,
                RejectedProductionsCount = counts[5].ProductionsCount,

                TotalPCECasesCount = counts.Sum(count => count.DistinctPCECaseCount),
                TotalProductionsCount = counts.Sum(count => count.ProductionsCount),
            };
        }

        private async Task<(int DistinctPCECaseCount, int ProductionsCount)> GetPCECasesCountAsync(Guid userId, string status)
        {
            var productions = await _cbeContext.PCECaseAssignments
                                                .AsNoTracking()
                                                .Include(res => res.ProductionCapacity)
                                                .Where(res => res.UserId == userId && res.Status == status)
                                                .ToListAsync();

            var distinctPCECaseCount = productions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count();
            var productionsCount = productions.Count;

            return (distinctPCECaseCount, productionsCount);
        }
        // public async Task<PCECasesCountDto> GetDashboardPCECasesCount(Guid UserId)
        // {
        //     var allProductions = await _cbeContext.PCECaseAssignments
        //                                     .AsNoTracking()
        //                                     .Include(res => res.ProductionCapacity)
        //                                     .Where(res => res.UserId == UserId)
        //                                     .ToListAsync();

        //     var newProductions = allProductions.Where(res => res.Status == "New").ToList();
        //     var pendingProductions = allProductions.Where(res => res.Status == "Pending").ToList();
        //     var completedProductions = allProductions.Where(res => res.Status == "Completed").ToList();
        //     var reestimatedProductions = allProductions.Where(res => res.Status == "Reestimated").ToList();
        //     var resubmittedProductions = allProductions.Where(res => res.Status == "Reestimate").ToList();
        //     var rejectedProductions = allProductions.Where(res => res.Status == "Rejected").ToList();

        //     return new PCECasesCountDto()
        //     {
        //         NewPCECasesCount = newProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         NewProductionsCount = newProductions.Count,

        //         PendingPCECasesCount = pendingProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         PendingProductionsCount = pendingProductions.Count,

        //         CompletedPCECasesCount = completedProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         CompletedProductionsCount = completedProductions.Count,

        //         ReestimatedPCECasesCount = reestimatedProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         ReestimatedProductionsCount = reestimatedProductions.Count,

        //         ResubmittedPCECasesCount = resubmittedProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         ResubmittedProductionsCount = resubmittedProductions.Count,

        //         RejectedPCECasesCount = rejectedProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         RejectedProductionsCount = rejectedProductions.Count,

        //         TotalPCECasesCount = allProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         TotalProductionsCount = allProductions.Count,
        //     };
        // }  

        public async Task<CreateNewCaseCountDto> GetDashboardPCECaseCount(Guid UserId)
        {
            var newPCECaseCount = await _cbeContext.PCECases
                                                    .Where(res => res.RMUserId == UserId &&
                                                        (res.ProductionCapacities.Any(pc => pc.CurrentStage == "Relation Manager" && 
                                                                                            pc.CurrentStatus == "New")
                                                         || !res.ProductionCapacities.Any()))
                                                    .CountAsync();

            var allProductions = await _cbeContext.PCECaseAssignments
                                          .AsNoTracking()
                                          .Include(res => res.ProductionCapacity)
                                          .Where(res => res.UserId == UserId)
                                          .ToListAsync();
            var pendingProductions = allProductions.Where(res => res.Status == "Pending").ToList();

            return new CreateNewCaseCountDto()
            {
                NewPCECaseCount = newPCECaseCount,
                NewPCECollateralCount = await _cbeContext.ProductionCapacities
                                                .Where(pc => pc.CurrentStage == "Relation Manager" && pc.CurrentStatus == "New").CountAsync(),
                PendingPCECaseCount = pendingProductions.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                PendingPCECollateralCount = pendingProductions.Count,

                //PendingPCECollateralCount = await _cbeContext.PCECaseAssignments
                //                            .Where(coll => coll.Status == "Pending" && coll.UserId== UserId)
                //                            .CountAsync(),
                CompletedPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == UserId && res.ProductionCapacities.Any(pce => pce.CurrentStage == "Relation Manager" && pce.CurrentStatus == "Completed")).CountAsync(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCapacities.Where(pc => pc.CurrentStage == "Relation Manager" && pc.CurrentStatus == "Completed").CountAsync(),
                TotalPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == UserId).CountAsync(),
                TotalPCECollateralCount = await _cbeContext.ProductionCapacities.Where(res => res.CreatedById == UserId).CountAsync(),
            };

        }         
     
        public async Task<IEnumerable<PCECaseReturnDto>> GetAssignedPCECases(Guid UserId)
        {
            var pceCaseAssignments = await _cbeContext.PCECaseAssignments
                                                        .Include(res => res.ProductionCapacity)
                                                            .ThenInclude(res => res.PCECase)
                                                        .Where(pca => pca.UserId == UserId && pca.Status != "Terminated")
                                                        .ToListAsync();
            var uniquePCECases = pceCaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var uniqueProductions = pceCaseAssignments.Select(ca => ca.ProductionCapacity).DistinctBy(c => c.Id).ToList();
            var pceCaseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(uniquePCECases);
            foreach (var pceCaseDto in pceCaseDtos)
            {
                pceCaseDto.NoOfCollateral = pceCaseAssignments
                                                    .Where(pca => pca.Status == "Pending")
                                                    .Select(ca => ca.ProductionCapacity)
                                                    .DistinctBy(c => c.Id)
                                                    .Count(pca => pca.PCECaseId == pceCaseDto.Id);
                pceCaseDto.TotalNoOfCollateral = pceCaseAssignments
                                                    .Select(ca => ca.ProductionCapacity)
                                                    .DistinctBy(c => c.Id)
                                                    .Count(pca => pca.PCECaseId == pceCaseDto.Id);
            }
            return pceCaseDtos;
        }

        public async Task<IEnumerable<PCECaseReturnDto>> GetLatestPCECases(Guid UserId)
        {
            var cases = await _cbeContext.PCECases
                    .Include(x => x.ProductionCapacities)
                    .Include(x => x.District)
                    .Where(res => res.RMUserId == UserId)
                    .OrderByDescending(res => res.CreationDate).Take(5).ToListAsync();          

            return  _mapper.Map<IEnumerable<PCECaseReturnDto>>(cases);
        }
        
        public async Task<IEnumerable<PCECaseReturnDto>> GetPCECasesReport(Guid UserId)
        {
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStage == "Relation Manager"))
                .Where(res => res.RMUserId == UserId).ToListAsync();

            var caseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(cases);
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

        public PCEReportDataDto GetPCECaseDetailReport(Guid UserId, Guid id)
        {
            try
            {
                var pceCaseResult = _cbeContext.PCECases
                                   .Include(res => res.District)
                                   .Include(res => res.BussinessLicence)
                                   .Where(c => c.Id == id && c.RMUserId == UserId)
                                   .FirstOrDefault();
                var productionCapacities = _cbeContext.ProductionCapacities
                                            .Where(pc => pc.PCECaseId == id && pc.CreatedById == UserId)
                                            .ToList();
                var evaluation = _cbeContext.PCEEvaluations.ToList();
               
                var pceCaseDto = new PCEReportDataDto
                {
                    PCESCase = pceCaseResult,
                    Productions = productionCapacities,
                    PCEEvaluations = evaluation, // Set to null since not used
                    PCECaseSchedule = null // Set to null since not used
                };

                return pceCaseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " my error");
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
                        .Include(res => res.ProductionCapacities)
                        .Include(res => res.BussinessLicence)
                        .Where(c => productions.Select(p => p.PCECaseId).Contains(c.Id))
                        .FirstOrDefault();
            var pceEvaluations = await _cbeContext.PCEEvaluations
                                     .Include(e => e.ShiftHours)
                                     .Include(e => e.TimeConsumedToCheck)
                                     .Include(res => res.Evaluator).Where(res => res.PCEId == Id).ToListAsync();
            var pceCaseSchedule = await _cbeContext.PCECaseSchedules
                                                    .Where(res => res.PCECaseId == Id && res.Status == "Approved")
                                                    .FirstOrDefaultAsync();                    

            return new PCEReportDataDto
            {
                PCESCase = pceCase,
                Productions = productions,
                PCEEvaluations = pceEvaluations,
                PCECaseSchedule = pceCaseSchedule
            };
        }
        public async Task<PCEReportDataDto> GetPCEAllReportData(Guid Id)
        {

            var pceCase = _cbeContext.PCECases
                        .Where(c => c.Id==Id)
                        .FirstOrDefault();
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == Id).ToListAsync();

            var pceEvaluations = await _cbeContext.PCEEvaluations
                                     .Include(e => e.ShiftHours)
                                     .Include(e => e.TimeConsumedToCheck)
                                     .Where(c=>productions.Select(d=>d.Id).Contains(c.PCEId)).ToListAsync();
         
            var pceCaseSchedule = await _cbeContext.PCECaseSchedules.Where(res => res.PCECaseId == Id && res.Status == "Approved").FirstOrDefaultAsync();
            return new PCEReportDataDto
            {
                PCESCase = pceCase,
                Productions = productions,
                PCEEvaluations = pceEvaluations,
                PCECaseSchedule = pceCaseSchedule
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
                pceCaseDto.NoOfCollateral = pceCaseAssignments.Count(pca => pca.ProductionCapacity.PCECaseId == pceCaseDto.Id && pca.Status.Contains("Remark"));
                pceCaseDto.TotalNoOfCollateral = pceCaseAssignments.Count(pca => pca.ProductionCapacity.PCECaseId == pceCaseDto.Id);
            }
            return pceCaseDtos;
        }

        // public async Task<IEnumerable<PCECaseReturnDto>> GetRemarkedPCECases(Guid UserId)
        // {
        //     var PCECases = await _cbeContext.PCECases
        //                                     .Include(pc => pc.ProductionCapacities
        //                                         .Where(res => res.CurrentStatus.Contains("Remark") && res.CurrentStage == "Maker Officer"))
        //                                     .Where(res => res.ProductionCapacities.Any(res => res.CurrentStatus.Contains("Remark") && 
        //                                                                                       res.CurrentStage == "Maker Officer"))
        //                                     // .Where(res => res.RMUserId == UserId)
        //                                     .ToListAsync();
        //     var pceCaseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(PCECases);
        //     foreach (var pceCaseDto in pceCaseDtos)
        //     {
        //         pceCaseDto.NoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == pceCaseDto.Id);
        //         pceCaseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == pceCaseDto.Id);
        //     }
        //     return pceCaseDtos;
        // }

        // public async Task<IEnumerable<PCECaseReturnDto>> GetPCENewCases(Guid UserId)
        // {
        //     var cases = await _cbeContext.PCECases
        //                                   .Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager"))
        //                                   .Where(res => res.RMUserId == UserId &&
        //                                       (res.ProductionCapacities.Any(pc => pc.CurrentStage == "Relation Manager" && pc.CurrentStatus == "New")
        //                                        || !res.ProductionCapacities.Any()))
        //                                    .ToListAsync();

        //     var caseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(cases);
        //     foreach (var caseDto in caseDtos)
        //     {
        //         caseDto.NoOfCollateral = _cbeContext.ProductionCapacities
        //             .Where(pc => pc.PCECaseId == caseDto.Id && pc.CurrentStage == "Relation Manager")
        //             .Count();
        //     }
        //     foreach (var caseDto in caseDtos)
        //     {
        //         caseDto.TotalNoOfCollateral = _cbeContext.ProductionCapacities
        //             .Where(pc => pc.PCECaseId == caseDto.Id)
        //             .Count();
        //     }
        //     return caseDtos;
        // }

        // public async Task<IEnumerable<PCECaseReturnDto>> GetPCEPendingCases(Guid UserId)
        // {
        //     var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => ( res.CurrentStage != "Relation Manager" && res.CurrentStatus != "Completed" )))
        //                .Where(res => res.RMUserId == UserId && (res.ProductionCapacities.Any(pc => ( pc.CurrentStage != "Relation Manager" && pc.CurrentStatus != "Completed"))))

        //                .ToListAsync();
        //     var caseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(cases);
        //     //foreach (var caseDto in caseDtos)
        //     //{
        //     //    caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
        //     //    caseDto.NoOfCollateral = await _cbeContext.ProductionCaseAssignments.CountAsync(res => res.UserId == caseDto.Id && res.Status == "Pending");
        //     //}
        //     return caseDtos;
        // }       

        // public async Task<IEnumerable<PCECaseReturnDto>> GetPCECompleteCases(Guid UserId)
        // {
        //     var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager"))
        //                         .Where(res => res.RMUserId == UserId && (res.ProductionCapacities.Any(res => res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager"))).ToListAsync();
        //     var caseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(cases);
        //     foreach (var caseDto in caseDtos)
        //     {
        //         caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
        //     }
        //     return caseDtos;
        // }

        // public async Task<IEnumerable<PCECaseReturnDto>> GetPCETotalCases(Guid UserId)
        // {
        //     var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => (res.CurrentStage != "Relation Manager") && ((res.CurrentStatus != "Completed" && res.CurrentStage != "Checker Officer"))))
        //                .Where(res => res.RMUserId == UserId)
        //                .ToListAsync();
        //     var caseDtos = _mapper.Map<IEnumerable<PCECaseReturnDto>>(cases);
            
        //     foreach (var caseDto in caseDtos)
        //     {
        //         caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
        //         caseDto.NoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(a=>a.CurrentStatus=="Completed" && a.PCECaseId == caseDto.Id);
        //     }

        //     return caseDtos;
        // }

        // public async Task<CreateNewCaseCountDto> GetMyDashboardCaseCount()
        // {
        //     var httpContext = _httpContextAccessor.HttpContext;
        //     var CompProduction = await _cbeContext.PCECaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("UserId")) && res.Status == "Completed").ToListAsync();
     

        //     return new CreateNewCaseCountDto()
        //     {
        //         PCSCompletedCaseCount = CompProduction.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
        //         CompletedPCECollateralCount = await _cbeContext.PCECaseAssignments.Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("UserId")) && res.Status == "Completed").CountAsync(),
        //     };
        // }
    }
}