using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto;
using mechanical.Models.Dto.NotificationDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.NotificationService;
using mechanical.Services.UploadFileService;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using OpenCvSharp.CPlusPlus;

namespace mechanical.Services.MOCaseService
{
    public class MOCaseService : ICOCaseService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IUploadFileService _uploadFileService;
        private readonly INotificationService _notificationService;
        public MOCaseService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, ICaseTimeLineService caseTimeLineService, INotificationService notificationService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
            _uploadFileService = uploadFileService;
            _notificationService = notificationService;
        }
        //public async Task<IEnumerable<MMNewCaseDto>> GetMONewCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Maker Officer").ToListAsync();

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
        //                        NoOfCollateral = await _cbeContext.CaseAssignments.CountAsync(ca => ca.CaseId == caseDetail.Id && ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Maker Officer")
        //                    };
        //                    mTlNewCaseDtos.Add(caseDto);
        //                }
        //            }
        //        }
        //    }
        //    return mTlNewCaseDtos;
        //}

        public async Task<bool> SendCheking(Guid userId, Guid CollateralId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var collateral = await _cbeContext.Collaterals.FindAsync(CollateralId);

            if (collateral == null)
            {
                return false;
            }
            if (collateral.Category == Models.Enum.MechanicalCollateralCategory.IBFEqupment)
            {
                var indBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.CollateralId == CollateralId).FirstOrDefaultAsync();
                if (indBldgFacilityEquipment != null)
                {
                    var indBldgFacilityEquipmentCosts = await _cbeContext.IndBldgFacilityEquipmentCosts.Where(res => res.Id == indBldgFacilityEquipment.IndBldgFacilityEquipmentCostsId).FirstOrDefaultAsync();
                    var indBldgFacilityEquipments = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.IndBldgFacilityEquipmentCostsId == indBldgFacilityEquipment.IndBldgFacilityEquipmentCostsId).ToListAsync();
                    if (indBldgFacilityEquipmentCosts != null)
                    {
                        var remaining = indBldgFacilityEquipmentCosts.CollateralCount - indBldgFacilityEquipments.Count();
                        if (remaining != 0)
                        {
                            throw new InvalidOperationException("The system cannot send to the checker unit until all collateral sharing the same INDBLDG Facility Equipment Costs has completed evaluation.");
                        }
                    }
                }

            }
            collateral.CurrentStage = "Checker Manager";
            collateral.CurrentStatus = "New";
            _cbeContext.Collaterals.Update(collateral);
            await _cbeContext.SaveChangesAsync();

            var cases = await _cbeContext.Cases.Include(res => res.District).FirstOrDefaultAsync(res => res.Id == collateral.CaseId);
            var user = await _cbeContext.Users.Include(res => res.District).FirstOrDefaultAsync(res => res.Id == userId);
            if (user == null)
            {
                throw new InvalidOperationException("Checker unit in you department is not ready.");
            }
            //notification 
            var notificationContent = "New case evalaution sent for checking";
            var notificationType = "Case check";
            var link = $"/Collateral/Detail?Id={collateral.Id}";
            NotificationReturnDto notification=null;

            if (user?.District?.Name == "Head Office")
            {
                var checker = await _cbeContext.Users.FirstOrDefaultAsync(res => res.District.Name == "Head Office" && res.Role.Name == "Checker Manager");
                if (checker == null) return false;
                var caseAssignment = new CaseAssignment()
                {
                    CollateralId = CollateralId,
                    UserId = checker.Id,
                    Status = "New",
                    AssignmentDate = DateTime.UtcNow
                };
                await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
                await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                {
                    CaseId = collateral.CaseId,
                    Activity = $"<strong>Case send for Checkeing to Checkr Unit.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {cases.District.Name}.",
                    CurrentStage = "Maker Manager"
                });
                await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                {
                    CaseId = collateral.CaseId,
                    Activity = $"<strong>New Case assigned for evaluation.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {cases.District.Name}.",
                    CurrentStage = "Checker Manager",
                    UserId = checker.Id
                });
                // Add Notification
                notification = await _notificationService.AddNotification(checker.Id, notificationContent, notificationType, link);
            }
            else
            {
                var checker = await _cbeContext.Users.FirstOrDefaultAsync(res => res.DistrictId == user.DistrictId && res.Role.Name == "District Valuation Manager");
                var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == CollateralId && res.UserId == checker.Id).FirstOrDefaultAsync();
                if (caseAssignment == null) return false;
                caseAssignment.Status = "Checker New";
                _cbeContext.CaseAssignments.Update(caseAssignment);
                await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                {
                    CaseId = collateral.CaseId,
                    Activity = $"<strong>Case send for Checkeing to Checkr Unit.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {cases.District.Name}.",
                    CurrentStage = "Maker Manager"
                });
                await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                {
                    CaseId = collateral.CaseId,
                    Activity = $"<strong>New Case assigned for evaluation.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {cases.District.Name}.",
                    CurrentStage = "Checker Manager",
                    UserId = checker.Id
                });
            }

            _cbeContext.Collaterals.Update(collateral);
            await _cbeContext.SaveChangesAsync();

            // Realtime Nofication
            if (notification != null) await _notificationService.SendNotification(notification);
            
            return true;
        }
    }
}
