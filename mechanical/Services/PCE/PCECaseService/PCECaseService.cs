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
using DocumentFormat.OpenXml.Spreadsheet;

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
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                var user = _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefault(res => res.Id == userId);
                var httpContext = _httpContextAccessor.HttpContext;
                var loanCase = _mapper.Map<PCECase>(pCECaseDto);
                loanCase.Id = Guid.NewGuid();
                // loanCase.CurrentStatus = "New";
                loanCase.Status = "New";
                loanCase.DistrictId = user.DistrictId;
                loanCase.RMUserId = userId;
                loanCase.CreationDate = DateTime.Now;
                await _cbeContext.PCECases.AddAsync(loanCase);

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = loanCase.Id,
                    Activity = $"<strong>A new case with ID {loanCase.CaseNo} has been created</strong>",
                    CurrentStage = "Relation Manager"
                });   

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return loanCase; 
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating case.");
            }

        }


       // Task<IEnumerable<PCECaseDto>> GetPCENewCases(Guid userId);
        public async Task<IEnumerable<PCENewCaseDto>> GetPCENewCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases
                                          .Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager"))
                                          .Where(res => res.RMUserId == userId &&
                                              (res.ProductionCapacities.Any(pc => pc.CurrentStage == "Relation Manager" && pc.CurrentStatus == "New")
                                               || !res.ProductionCapacities.Any()))
                                           .ToListAsync();

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
        public async Task<IEnumerable<PCENewCaseDto>> GetPCECasesReport(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStage == "Relation Manager"))
                .Where(res => res.RMUserId == userId).ToListAsync();

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



            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => ( res.CurrentStage != "Relation Manager")&&((res.CurrentStatus != "Completed" && res.CurrentStatus != "New"))))
                       .Where(res => res.RMUserId == userId && (res.ProductionCapacities.Any(collateral => ( collateral.CurrentStage != "Relation Manager") && ((collateral.CurrentStatus != "Completed" && collateral.CurrentStatus != "New")))))

                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
                caseDto.NoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id && res.CurrentStatus == "Pending");
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


        public PCEReportDataDto GetPCECaseDetailReport(Guid userId, Guid id)
        {

            try
            {
                var pceCaseResult = _cbeContext.PCECases
                                   .Include(res => res.District)
                                   .Include(res => res.BussinessLicence)
                                   .Where(c => c.Id == id && c.RMUserId == userId)
                                   .FirstOrDefault();
                var productionCapacities = _cbeContext.ProductionCapacities
                                            .Where(pc => pc.PCECaseId == id && pc.CreatedById == userId)
                                            .ToList();
                var evaluation = _cbeContext.PCEEvaluations.ToList();
                // Create the PCEReportDataDto
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










        public async Task<PCECaseReturntDto> PCEEdit(Guid userId, PCECaseReturntDto caseDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var pceCase = await _cbeContext.PCECases.FirstOrDefaultAsync(c => c.Id == userId);

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
                    
                return _mapper.Map<PCECaseReturntDto>(pceCase);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the case");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating the case.");
            }
        }





        public async Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager"))
                                .Where(res => res.RMUserId == userId && (res.ProductionCapacities.Any(res => res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager"))).ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
            }
            return caseDtos;
        }

        public async Task<IEnumerable<PCENewCaseDto>> GetPCERejectedCases(Guid userId)
        {
            //var cases = await _cbeContext.PCECases.Where(res => res.CurrentStatus == "Rejected" && res.CurrentStage == "Relation Manager").ToListAsync();
            //var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            //return caseDtos;

            var pceCaseswithCounts = await _cbeContext.PCECases
                        .Where(p => p.RMUserId == userId && p.ProductionCapacities.Any(p => p.CurrentStatus == "Rejected" && p.CurrentStage == "Relation Manager"))
                        .GroupJoin(
                            _cbeContext.ProductionCapacities.Where(pc => pc.CurrentStatus == "Rejected" && pc.CurrentStage == "Relation Manager"),
                        c => c.Id,
                        pc => pc.PCECaseId,
                        (pceCase, productionCapacities) => new
                        {
                            Case = pceCase,
                            TotalNoOfCollateral = productionCapacities.Count()
                        }
                        )
                        .ToListAsync();
            
            var pceCaseDtos = pceCaseswithCounts.Select(pceCase =>
            {
                var dto = _mapper.Map<PCENewCaseDto>(pceCase.Case);
                dto.TotalNoOfCollateral = pceCase.TotalNoOfCollateral;
                return dto;
            });
            return pceCaseDtos;
        }


        public async Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => (res.CurrentStage != "Relation Manager") && ((res.CurrentStatus != "Completed" && res.CurrentStage != "Checker Officer"))))
                       .Where(res => res.RMUserId == userId)
                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
                caseDto.NoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(a=>a.CurrentStatus=="Completed" && a.PCECaseId == caseDto.Id);
            }

            return caseDtos;
        }

  

   
        public async Task<CreateNewCaseCountDto> GetDashboardPCECaseCount(Guid userId)
        {
            var newPCECaseCount = await _cbeContext.PCECases
                                                    .Where(res => res.RMUserId == userId &&
                                                        (res.ProductionCapacities.Any(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New")
                                                         || !res.ProductionCapacities.Any()))
                                                    .CountAsync();

            var allPCEs = await _cbeContext.ProductionCaseAssignments
                                          .AsNoTracking()
                                          .Include(res => res.ProductionCapacity)
                                          .Where(res => res.UserId == userId)
                                          .ToListAsync();
            var pendingPCEs = allPCEs.Where(res => res.Status == "Pending").ToList();

            return new CreateNewCaseCountDto()
            {
                NewPCECaseCount = newPCECaseCount,
                NewPCECollateralCount = await _cbeContext.ProductionCapacities
                                                .Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New").CountAsync(),
                PendingPCECaseCount = pendingPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                PendingPCECollateralCount = pendingPCEs.Count,

                //PendingPCECollateralCount = await _cbeContext.ProductionCaseAssignments
                //                            .Where(coll => coll.Status == "Pending" && coll.UserId== userId)
                //                            .CountAsync(),
                CompletedPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => coll.CurrentStage == "Relation Manager" && coll.CurrentStatus == "Completed")).CountAsync(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCapacities.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "Completed").CountAsync(),
                TotalPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId).CountAsync(),
                TotalPCECollateralCount = await _cbeContext.ProductionCapacities.Where(res => res.CreatedById == userId).CountAsync(),
            };

        }
        public async Task<CreateNewCaseCountDto> GetMyDashboardCaseCount()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            // var NewCollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            // var PendCollateral = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Status == "Pending").ToListAsync();
            var CompCollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.Status == "Completed").ToListAsync();
     

            return new CreateNewCaseCountDto()
            {

                PCSCompletedCaseCount = CompCollateral.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.Status == "Completed").CountAsync(),

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
        
        public async Task<PCEReportDataDto> GetPCEReportData(Guid Id)
        {
            // the following code are used to access each production based on  Single pce
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.Id == Id && res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager").ToListAsync();            
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
            var pceCaseSchedule = await _cbeContext.PCECaseSchedules.Where(res => res.PCECaseId == Id && res.Status == "Approved").FirstOrDefaultAsync();
                     


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
     
        public async Task<IEnumerable<PCENewCaseDto>> GetMyAssignmentPCECases(Guid UserId)
        {
            var productionCaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(pca => pca.UserId == UserId && pca.Status != "Terminate").ToListAsync();
            var uniquePCECases = productionCaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var pceCaseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(uniquePCECases);
            foreach (var PCEcaseDto in pceCaseDtos)
            {
                PCEcaseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == PCEcaseDto.Id);
            }
            return pceCaseDtos;
        }

        public async Task<IEnumerable<PCENewCaseDto>> GetRemarkedPCECases(Guid UserId)
        {
            var PCECases = await _cbeContext.PCECases
                                            .Include(pc => pc.ProductionCapacities.Where(res => res.CurrentStatus.Contains("Remark") && res.CurrentStage == "Maker Officer"))
                                            .Where(res => res.ProductionCapacities.Any(res => res.CurrentStatus.Contains("Remark") && res.CurrentStage == "Maker Officer"))
                                            // .Where(res => res.PCECaseOriginatorId == UserId && (res.ProductionCapacities.Any(res => res.CurrentStatus.Contains("Remark") && res.CurrentStage == "Maker Officer")))
                                            .ToListAsync();
            var PCECaseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(PCECases);
            foreach (var PCECaseDto in PCECaseDtos)
            {
                PCECaseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == PCECaseDto.Id);
            }
            return PCECaseDtos;
        }

        public async Task<IEnumerable<PCECaseTerminateDto>> GetCaseTerminates(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities)
                    //    .Where(res => res.RMUserId == userId && res.CurrentStatus == "Terminate")
                       .Where(res => res.RMUserId == userId && res.Status == "Terminate")
                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCECaseTerminateDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TerminationReason = (await _cbeContext.PCECaseTerminates.Where(res => res.PCECaseId == caseDto.Id).FirstOrDefaultAsync()).Reason;
            }
            return caseDtos;
        }

        public async Task<PCECaseReturntDto> GetCaseDetail(Guid id)
        {
            var loanCase = await _cbeContext.PCECases
                           .Include(res => res.District).Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<PCECaseReturntDto>(loanCase);
        }
        public async Task<PCECaseTerminate> ApproveCaseTermination(Guid id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var caseTerminate = await _cbeContext.PCECaseTerminates.FindAsync(id);
                if (caseTerminate == null)
                {
                    throw new Exception("case Schedule not Found");
                }
                caseTerminate.Status = "Approved";
                // caseTerminate.CurrentStatus = "Approved";
                _cbeContext.Update(caseTerminate);

                var cases = await _cbeContext.PCECases.FindAsync(caseTerminate.PCECaseId);
                // cases.CurrentStatus = "Terminate";
                cases.Status = "Terminate";
                var collaterals = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == caseTerminate.PCECaseId).ToListAsync();

                foreach (var collateral in collaterals)
                {
                    collateral.CurrentStage = "Relation Manager";
                    collateral.CurrentStatus = "Terminate";
                    var caseAssignments = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == collateral.Id).ToListAsync();
                    foreach (var caseAssignment in caseAssignments)
                    {
                        caseAssignment.Status = "Terminate";

                    }
                    _cbeContext.ProductionCaseAssignments.UpdateRange(caseAssignments);
                }

                _cbeContext.ProductionCapacities.UpdateRange(collaterals);
          
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return caseTerminate;    
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving case termination");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while approving case termination.");
            }
        }



        public async Task<IEnumerable<PCENewCaseDto>> GetRmLatestPCECases(Guid userId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var cases = await _cbeContext.PCECases
                    .Include(x => x.ProductionCapacities)
                    .Include(x => x.District)
                    .Where(res => res.RMUserId == userId)
                    .OrderByDescending(res => res.CreationDate).Take(5).ToListAsync();

          
            foreach (var cas in cases)
            {
                var status = "New";
                var iterationcount = 0;
                var completcount = 0;
                foreach (var c in cas.ProductionCapacities)
                {
                    iterationcount = iterationcount + 1;

                    if (c.CurrentStatus == "New")
                    {
                    }
                    else if(c.CurrentStatus == "Completed")
                    {
                        completcount = completcount + 1;
                    }
                    else
                    {
                        status = "Pending";
                    }
                }
                if(completcount == iterationcount)
                {
                    status = "Completed";
                }
                if (iterationcount ==0)
                {
                    status = "New";
                }
                // cas.CurrentStatus = status;
                cas.Status = status;
            }

            var a =  _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return a;
        }


    }
}