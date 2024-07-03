﻿using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using mechanical.Models.Dto.CaseTimeLineDto;
using Microsoft.CodeAnalysis.Operations;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http.HttpResults;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.CaseScheduleDto;

namespace mechanical.Services.CaseServices
{
    public class CaseService:ICaseService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IUploadFileService _uploadFileService;
        
        
        public CaseService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
            _uploadFileService = uploadFileService;
        }
        public async Task<IEnumerable<CaseDto>> GetRmRemarkedCases(Guid userId)
        {
            var cases = await _cbeContext.Cases.Include(x => x.Collaterals.Where(res => res.CurrentStatus.Contains("Remark") && res.CurrentStage == "Maker Officer"))
           .Where(res => res.CaseOriginatorId == userId && (res.Collaterals.Any(res => res.CurrentStatus.Contains("Remark") && res.CurrentStage == "Maker Officer"))).ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<Case> CreateCase(Guid userId, CasePostDto createCaseDto)
        {
            var user = _cbeContext.CreateUsers.Include(res => res.District).Include(res=>res.Role).FirstOrDefault(res => res.Id == userId);
            if(user == null)
            {
                throw new Exception("user not found");
            }
            var loanCase = _mapper.Map<Case>(createCaseDto);
            loanCase.Id = Guid.NewGuid();
            if(createCaseDto.BussinessLicence != null)
            {
                var BussinessLicence = new CreateFileDto()
                {
                    File = createCaseDto.BussinessLicence ?? throw new ArgumentNullException(nameof(createCaseDto.BussinessLicence)),
                    CaseId = loanCase.Id,
                    Catagory = "Bussiness Licence"
                };
                loanCase.BussinessLicenceId = await _uploadFileService.CreateUploadFile(userId,BussinessLicence);
            }
            loanCase.CaseOriginatorId = userId;
            loanCase.CreationAt = DateTime.Now;
            loanCase.DistrictId = user.DistrictId;
            loanCase.Status = "New";
            await _cbeContext.Cases.AddAsync(loanCase);
            await _cbeContext.SaveChangesAsync();

            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = loanCase.Id,
                Activity = $"<strong>A new case with ID {loanCase.CaseNo} has been created</strong>",
                CurrentStage = user.Role.Name
            });
          
            return loanCase;
        }
        public async Task<CaseReturntDto> GetCase(Guid userId, Guid id)
        {
            var loanCase = await _cbeContext.Cases
                           .Include(res => res.BussinessLicence).Include(res => res.District).Include(res=>res.Collaterals)
                           .FirstOrDefaultAsync(c => c.Id == id && c.CaseOriginatorId == userId);
            return _mapper.Map<CaseReturntDto>(loanCase);
        }
        public async Task<CaseReturntDto> GetCaseDetail(Guid id)
        {
            var loanCase = await _cbeContext.Cases
                           .Include(res => res.BussinessLicence).Include(res => res.District).Include(res => res.Collaterals)
                           .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<CaseReturntDto>(loanCase);
        }
        public async Task<IEnumerable<CaseDto>> GetNewCases(Guid userId)
        {
            var cases = await _cbeContext.Cases.Include(x => x.Collaterals.Where(res=>res.CurrentStatus=="New" && res.CurrentStage == "Relation Manager"))
                       .Where(res => res.CaseOriginatorId == userId && res.Status == "New")
                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(cases);
            foreach(var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos ;
        }
        public async Task<IEnumerable<CaseTerminateDto>> GetTerminatedCases(Guid userId)
        {
            var cases = await _cbeContext.Cases.Include(x => x.Collaterals)
                       .Where(res => res.CaseOriginatorId == userId && res.Status == "Terminate")
                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<CaseTerminateDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TerminationReason = (await _cbeContext.CaseTerminates.Where(res => res.CaseId == caseDto.Id).FirstOrDefaultAsync()).Reason;
            }
            return caseDtos;
        }
        public async Task<IEnumerable<CaseDto>> GetRejectedCases(Guid userId)
        {
            var cases = await _cbeContext.Cases.Include(x => x.Collaterals.Where(res => res.CurrentStatus == "Reject" && res.CurrentStage == "Relation Manager"))
                       .Where(res => res.CaseOriginatorId == userId && res.Status == "New").ToListAsync();
                  var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<IEnumerable<CaseDto>> GetRmPendingCases(Guid userId)
        {
            var cases = await _cbeContext.Cases.Include(x => x.Collaterals.Where(res => ( res.CurrentStage != "Relation Manager")&&((res.CurrentStatus != "Complete" && res.CurrentStage != "Checker Officer"))))
                       .Where(res => res.CaseOriginatorId == userId && (res.Collaterals.Any(collateral => ( collateral.CurrentStage != "Relation Manager") && ((collateral.CurrentStatus != "Complete" && collateral.CurrentStage != "Checker Officer")))))

                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<Case> EditCase(Guid userId,Guid id, CasePostDto createCaseDto)
        {
            var loanCase = await _cbeContext.Cases
                .Include(res => res.BussinessLicence)
                .FirstOrDefaultAsync(c => c.Id == id && c.CaseOriginatorId == userId);
            if (loanCase != null)
            {
                _mapper.Map(createCaseDto, loanCase);
                _cbeContext.Update(loanCase);
                await _cbeContext.SaveChangesAsync();
                return loanCase;
            }
            throw new Exception("case with this Id is not found");
        }
        public async Task<IEnumerable<CaseDto>> GetMmLatestCases(Guid userId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var NewCollateral = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res=>res.Case).ThenInclude(res=>res.CaseOriginator).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            var cases = NewCollateral.Select(res => res.Collateral.Case).Distinct().OrderByDescending(res => res.CreationAt).Take(7);
            return _mapper.Map<IEnumerable<CaseDto>>(cases);
        }
        public async Task<IEnumerable<CaseDto>> GetRmLatestCases(Guid userId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var cases = await _cbeContext.Cases
                    .Include(x => x.Collaterals)
                    .Include(x => x.District)
                    .Where(res => res.CaseOriginatorId == userId)
                    .OrderByDescending(res => res.CreationAt).Take(7).ToListAsync();
            return _mapper.Map<IEnumerable<CaseDto>>(cases);
        }
        public async Task<CaseCountDto> GetDashboardCaseCount(Guid userId)
        {
            return new CaseCountDto()
            {
                NewCaseCount = await _cbeContext.Cases.Where(res => res.CaseOriginatorId == userId && res.Collaterals.Any(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New")).CountAsync(),
                NewCollateralCount = await _cbeContext.Collaterals.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New").CountAsync(),
                PendingCaseCount = await _cbeContext.Cases.Where(res => res.CaseOriginatorId == userId && res.Collaterals.Any(collateral => (collateral.CurrentStage != "Checker Officer" && collateral.CurrentStatus != "Complete") && collateral.CurrentStage != "Relation Manager" )).CountAsync(),
                PendingCollateralCount = await _cbeContext.Collaterals.Where(collateral => collateral.CurrentStage != "Checker Officer" && collateral.CurrentStatus != "Complete" && collateral.CurrentStage != "Relation Manager").CountAsync(),
                CompletedCaseCount = await _cbeContext.Cases.Where(res => res.CaseOriginatorId == userId && res.Collaterals.Any(collateral => collateral.CurrentStage == "Checker Officer" && collateral.CurrentStatus == "Complete")).CountAsync(),
                CompletedCollateralCount = await _cbeContext.Collaterals.Where(collateral => collateral.CurrentStage == "Checker Officer" && collateral.CurrentStatus == "Complete").CountAsync(),
                TotalCaseCount = await _cbeContext.Cases.Where(res => res.CaseOriginatorId == userId).CountAsync(),
                TotalCollateralCount = await _cbeContext.Collaterals.Where(res=>res.CreatedById == userId).CountAsync(),
            };
        }
        public async Task<CaseCountDto> GetMyDashboardCaseCount(Guid userId)
        {
            var  NewCollateral = await _cbeContext.CaseAssignments.Include(res=>res.Collateral).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            var  PendCollateral = await _cbeContext.CaseAssignments.Include(res=>res.Collateral).Where(res => res.UserId == userId && res.Status == "Pending").ToListAsync();
            var  CompCollateral = await _cbeContext.CaseAssignments.Include(res=>res.Collateral).Where(res => res.UserId == userId && res.Status == "Complete").ToListAsync();
            var totalcollatera = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId ).ToListAsync();

            return new CaseCountDto()
            {
                NewCaseCount = NewCollateral.Select(res => res.Collateral.CaseId) .Distinct().Count(),
                NewCollateralCount = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.Status == "New").CountAsync(),

                PendingCaseCount = PendCollateral.Select(res => res.Collateral.CaseId).Distinct().Count(),
                PendingCollateralCount = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.Status == "Pending").CountAsync(),

                CompletedCaseCount = CompCollateral.Select(res => res.Collateral.CaseId).Distinct().Count(),
                CompletedCollateralCount = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.Status == "Complete").CountAsync(),

                TotalCaseCount = totalcollatera.Select(res => res.Collateral.CaseId).Distinct().Count(),
                TotalCollateralCount = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId).CountAsync(),
            };
        }


        public async Task<bool> SendRejection(MoRejectCaseDto moRejectCaseDto)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var assignedCases = await _cbeContext.Collaterals.FirstOrDefaultAsync(res => res.Id == moRejectCaseDto.CollateralId);
            if (assignedCases == null)
            {
                return false;
            }
            var reject = _mapper.Map<Reject>(moRejectCaseDto);
            reject.CreationDate = DateTime.Now;
            reject.RejectedBy = Guid.Parse(httpContext.Session.GetString("userId"));
            await _cbeContext.Rejects.AddAsync(reject);
           

            assignedCases.CurrentStage = "Relation Manager";
            assignedCases.CurrentStatus = "Reject";
            _cbeContext.Collaterals.Update(assignedCases);
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = assignedCases.CaseId,
                Activity = $"<strong>Collateral is rejected.</strong> <br> <i class='text-purple'>",
                CurrentStage = "Maker Manager",
            });

            var caseAssignment = await _cbeContext.CaseAssignments.FirstOrDefaultAsync(res => res.CollateralId == moRejectCaseDto.CollateralId && res.UserId == reject.RejectedBy);
            caseAssignment.Status = "Reject";
            _cbeContext.Update(caseAssignment);
            await _cbeContext.SaveChangesAsync();
            
            //var maker = await _cbeContext.CreateUsers.FirstOrDefaultAsync(res => res.DistrictId == assignedCases.DistrictId && res.Role.Name == "Maker Manager");
            //await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            //{
            //    CaseId = CaseId,
            //    Activity = $"<strong>Case send for evaluation to Maker Unit.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
            //    CurrentStage = "Relation Manager"
            //});
            //await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            //{
            //    CaseId = CaseId,
            //    Activity = $"<strong>New Case assigned for evaluation.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
            //    CurrentStage = "Maker Manager",
            //    UserId = maker.Id
            //});
            return true;
        }

        //public async Task<RmNewCaseDto> GetRmNewCase(Guid Id)
        //{
        //    var loanCase = await _cbeContext.Cases.Include(res => res.District).FirstOrDefaultAsync(c => c.Id == Id);
        //    return _mapper.Map<RmNewCaseDto>(loanCase);
        //}

        //public async Task<RmNewCaseDto> GetCONewCase(Guid Id)
        //{
        //    var loanCase = await _cbeContext.Cases.Include(res => res.District).FirstOrDefaultAsync(c => c.Id == Id);
        //    return _mapper.Map<RmNewCaseDto>(loanCase);
        //}

        public async Task<IEnumerable<CaseDto>> GetRmCompleteCases(Guid userId)
        {
            var cases = await _cbeContext.Cases.Include(x => x.Collaterals.Where(res =>res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer"))
           .Where(res => res.CaseOriginatorId == userId && (res.Collaterals.Any(res => res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer"))).ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<IEnumerable<CaseDto>> GetRmTotalCases(Guid userId)
        {
            var cases = await _cbeContext.Cases.Include(x => x.Collaterals)
           .Where(res => res.CaseOriginatorId == userId ).ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        
        public async Task<IEnumerable<CaseDto>> GetTotalCases(Guid userId)
        {
            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).Where(Ca => Ca.UserId == userId).ToListAsync();
            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }

        //public async Task<IEnumerable<RMCaseDto>> GetRmPendingCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var cases = await _cbeContext.Cases
        //               .Include(x => x.Collaterals)
        //               .Include(x => x.District)
        //               .Where(res => res.RMUserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.CurrentStage != "Relation Manager" && !( res.CurrentStage == "Checker Manager" && res.CurrentStatus == "Complete"))
        //               .ToListAsync();
        //    return _mapper.Map<IEnumerable<RMCaseDto>>(cases);
        //}


        public async Task<bool> DeleteBussinessLicence(Guid Id)
        {
            var cases = await _cbeContext.Cases.FindAsync(Id);
            if (cases == null)
            {
                return false;
            }
            if (cases.BussinessLicenceId != null && await _uploadFileService.DeleteFile(cases.BussinessLicenceId.Value))
            {
                cases.BussinessLicenceId = null;
                _cbeContext.Update(cases);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> UploadBussinessLicence(Guid userId,IFormFile file, Guid caseId)
        {
            var cases = await _cbeContext.Cases.FindAsync(caseId);
            if (cases == null)
            {
                return false;
            }
            var BussinessLicence = new CreateFileDto()
            {
                File = file ?? throw new ArgumentNullException(nameof(file)),
                CaseId = caseId,
                Catagory = "Bussiness Licence"
            };
            cases.BussinessLicenceId = await _uploadFileService.CreateUploadFile(userId,BussinessLicence);
            _cbeContext.Update(cases);
            await _cbeContext.SaveChangesAsync();
            return true;
        }

        //public async Task<IEnumerable<RMCaseDto>> GetMTLPendingCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Pending").ToListAsync();

        //    List<RMCaseDto> mTlPendingCaseDtos = new List<RMCaseDto>();
        //    if (caseAssignments != null)
        //    {
        //        foreach (var caseAssignment in caseAssignments)
        //        {
        //            var caseDetail = await _cbeContext.Cases.Include(x=> x.District).Include(x => x.Collaterals).FirstOrDefaultAsync(c => c.Id == caseAssignment.CaseId);
        //            if (caseDetail != null)
        //            {
        //                if (!mTlPendingCaseDtos.Any(dto => dto.Id == caseDetail.Id))
        //                {
        //                    RMCaseDto caseDto = new RMCaseDto
        //                    {
        //                        Id = caseDetail.Id,
        //                        CreationDate = caseDetail.CreationDate,
        //                        Center = caseDetail.District.Name,
        //                        CaseNo = caseDetail.CaseNo,
        //                        ApplicantName = caseDetail.ApplicantName,
        //                        CustomerId = caseDetail.CustomerId,
        //                        CurrentStage = caseDetail.CurrentStage,
        //                        CurrentStatus = caseDetail.CurrentStatus,
        //                        NoOfCollateral = await _cbeContext.CaseAssignments.CountAsync(ca => ca.CaseId == caseDetail.Id && ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Pending")

        //                    };
        //                    mTlPendingCaseDtos.Add(caseDto);
        //                }
        //            }
        //        }
        //    }
        //    return _mapper.Map<IEnumerable<RMCaseDto>>(mTlPendingCaseDtos);
        //}


        //public async Task<IEnumerable<MMNewCaseDto>> GetCheckerNewCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var user = await _cbeContext.CreateUsers.FirstOrDefaultAsync(ca => ca.Id == Guid.Parse(httpContext.Session.GetString("userId")) && ca.Role.Name == "Checker Manager");
        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    var cases = await _cbeContext.Cases.Include(x => x.Collaterals.Where(co => co.Status == "Complete")).Where(Ca => Ca.DistrictId == user.DistrictId && Ca.Collaterals.Any(co => co.Status == "Complete")).ToListAsync();

        //    return _mapper.Map<IEnumerable<MMNewCaseDto>>(cases);
        //}
        //public async Task<IEnumerable<MMNewCaseDto>> GetMTLNewCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId"))&& ca.status == "Maker TeamLeader").ToListAsync();

        //    List<MMNewCaseDto> mTlNewCaseDtos = new List<MMNewCaseDto>();
        //    if (caseAssignments != null)
        //    {
        //        foreach (var caseAssignment in caseAssignments)
        //        {
        //            var caseDetail = await _cbeContext.Cases.Include(x => x.Collaterals).FirstOrDefaultAsync(c => c.Id == caseAssignment.CaseId);
        //            if (caseDetail != null)
        //            {
        //                if (!mTlNewCaseDtos.Any(dto => dto.Id == caseDetail.Id))
        //                {
        //                    MMNewCaseDto caseDto = new MMNewCaseDto
        //                    {
        //                        Id = caseDetail.Id,
        //                        CreationDate = caseDetail.CreationDate,
        //                        MakerAssignmentDate = caseAssignment.AssignmentDate,
        //                        CaseNo = caseDetail.CaseNo,
        //                        ApplicantName = caseDetail.ApplicantName,
        //                        CustomerId = caseDetail.CustomerId,
        //                        RMUserId = caseDetail.RMUserId,
        //                        CurrentStage = caseDetail.CurrentStage,
        //                        CurrentStatus = caseDetail.CurrentStatus,
        //                        NoOfCollateral = await _cbeContext.CaseAssignments.CountAsync(ca => ca.CaseId == caseDetail.Id && ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Maker TeamLeader")
        //                    };
        //                    mTlNewCaseDtos.Add(caseDto);
        //                } 
        //            }
        //        }
        //    }
        //    return mTlNewCaseDtos;
        //}
        //public async Task<IEnumerable<CONewCaseDto>> GetCONewCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Checker Officer").ToListAsync();

        //    List<CONewCaseDto> mTlNewCaseDtos = new List<CONewCaseDto>();
        //    if (caseAssignments != null)
        //    {
        //        foreach (var caseAssignment in caseAssignments)
        //        {
        //            var caseDetail = await _cbeContext.Cases.Include(x => x.Collaterals).FirstOrDefaultAsync(c => c.Id == caseAssignment.CaseId);
        //            if (caseDetail != null)
        //            {
        //                if (!mTlNewCaseDtos.Any(dto => dto.Id == caseDetail.Id))
        //                {
        //                    CONewCaseDto caseDto = new CONewCaseDto
        //                    {
        //                        Id = caseDetail.Id,
        //                        CreationDate = caseDetail.CreationDate,
        //                        CaseNo = caseDetail.CaseNo,
        //                        ApplicantName = caseDetail.ApplicantName,
        //                        CustomerId = caseDetail.CustomerId,
        //                        RMUserId = caseDetail.RMUserId,
        //                        CurrentStage = caseDetail.CurrentStage,
        //                        CurrentStatus = caseDetail.CurrentStatus,
        //                        NoOfCollateral = await _cbeContext.CaseAssignments.CountAsync(ca => ca.CaseId == caseDetail.Id && ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Checker Officer")
        //                    };
        //                    mTlNewCaseDtos.Add(caseDto);
        //                }
        //            }
        //        }
        //    }
        //    return mTlNewCaseDtos;
        //}
        //public async Task<bool> SendEvaluation(Guid CaseId)
        //{
        //    var assignedCases = await _cbeContext.Cases.Include(res=>res.District).FirstOrDefaultAsync( res=> res.Id == CaseId);
        //    if (assignedCases == null)
        //    {
        //        return false;
        //    }
        //    assignedCases.CurrentStage = "Maker";
        //    assignedCases.CurrentStatus = "New";
        //    assignedCases.MakerAssignmentDate = DateTime.Now;
        //    _cbeContext.Cases.Update(assignedCases);
        //    await _cbeContext.SaveChangesAsync();
        //    var maker = await _cbeContext.CreateUsers.FirstOrDefaultAsync(res => res.DistrictId == assignedCases.DistrictId && res.Role.Name == "Maker Manager");
        //    await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
        //    {
        //        CaseId = CaseId,
        //        Activity = $"<strong>Case send for evaluation to Maker Unit.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
        //        CurrentStage = "Relation Manager"
        //    });
        //    await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
        //    {
        //        CaseId = CaseId,
        //        Activity = $"<strong>New Case assigned for evaluation.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
        //        CurrentStage = "Maker Manager",
        //        UserId = maker.Id
        //    });
        //    return true;
        //}
        //public async Task<bool> CheckedAndSendToRM(Guid CaseId)
        //{
        //    var assignedCases = await _cbeContext.Cases.Include(res => res.District).FirstOrDefaultAsync(res => res.Id == CaseId);
        //    if (assignedCases == null)
        //    {
        //        return false;
        //    }
        //    assignedCases.CurrentStage = "Maker";
        //    assignedCases.CurrentStatus = "New";
        //    assignedCases.MakerAssignmentDate = DateTime.Now;
        //    _cbeContext.Cases.Update(assignedCases);
        //    await _cbeContext.SaveChangesAsync();
        //    var maker = await _cbeContext.CreateUsers.FirstOrDefaultAsync(res => res.DistrictId == assignedCases.DistrictId && res.Role.Name == "Maker Manager");
        //    await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
        //    {
        //        CaseId = CaseId,
        //        Activity = $"<strong>Case send for evaluation to Maker Unit.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
        //        CurrentStage = "Relation Manager"
        //    });
        //    await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
        //    {
        //        CaseId = CaseId,
        //        Activity = $"<strong>New Case assigned for evaluation.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
        //        CurrentStage = "Maker Manager",
        //        UserId = maker.Id
        //    });
        //    return true;
        //}
        //public async Task<bool> CheckedAndRejectedSendToMO(Guid CaseId)
        //{
        //    var assignedCases = await _cbeContext.Cases.Include(res => res.District).FirstOrDefaultAsync(res => res.Id == CaseId);
        //    if (assignedCases == null)
        //    {
        //        return false;
        //    }
        //    assignedCases.CurrentStage = "Maker";
        //    assignedCases.CurrentStatus = "New";
        //    assignedCases.MakerAssignmentDate = DateTime.Now;
        //    _cbeContext.Cases.Update(assignedCases);
        //    await _cbeContext.SaveChangesAsync();
        //    var maker = await _cbeContext.CreateUsers.FirstOrDefaultAsync(res => res.DistrictId == assignedCases.DistrictId && res.Role.Name == "Maker Manager");
        //    await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
        //    {
        //        CaseId = CaseId,
        //        Activity = $"<strong>Case send for evaluation to Maker Unit.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
        //        CurrentStage = "Relation Manager"
        //    });
        //    await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
        //    {
        //        CaseId = CaseId,
        //        Activity = $"<strong>New Case assigned for evaluation.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedCases.District.Name}.",
        //        CurrentStage = "Maker Manager",
        //        UserId = maker.Id
        //    });
        //    return true;
        //}
        //#region Correction case parts
        //public async Task<Collateral> MyReturnedCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var cases = _cbeContext.Collaterals.Where(res => res.Status == "correction").FirstOrDefault();
        //    return cases;
        //}
        //public async Task<Collateral> MyResubmitedCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var cases = _cbeContext.Collaterals.Where(res => res.Status == "Resubmited").FirstOrDefault();
        //    return cases;
        //}
        //public async Task<Collateral> MyReturnedCase(Guid CollateralId)
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var cases = _cbeContext.Collaterals.Where(res => res.Status == "correction" && res.Id == CollateralId).FirstOrDefault();
        //    return cases;
        //}
        //public async Task<Collateral> MyResubmitedCase(Guid CollateralId)
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var cases = _cbeContext.Collaterals.Where(res => res.Status == "Resubmited" && res.Id == CollateralId).FirstOrDefault();
        //    return cases;
        //}
        //#endregion

        public async Task<CaseTerminate> ApproveCaseTermination(Guid id)
        {
            var caseTerminate = await _cbeContext.CaseTerminates.FindAsync(id);
            if (caseTerminate == null)
            {
                throw new Exception("case Schedule not Found");
            }
            caseTerminate.Status = "Approved";
            _cbeContext.Update(caseTerminate);

            var cases = await _cbeContext.Cases.FindAsync(caseTerminate.CaseId);
            cases.Status = "Terminate";
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == caseTerminate.CaseId).ToListAsync();

            foreach (var collateral in collaterals)
            {
                collateral.CurrentStage = "Relation Manager";
                collateral.CurrentStatus = "Terminate";
                var caseAssignments = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == collateral.Id).ToListAsync();
                foreach (var caseAssignment in caseAssignments)
                {
                    caseAssignment.Status = "Terminate";

                }
                _cbeContext.CaseAssignments.UpdateRange(caseAssignments);
            }

            _cbeContext.Collaterals.UpdateRange(collaterals);

            await _cbeContext.SaveChangesAsync();
            return caseTerminate;
        }
    }
}
