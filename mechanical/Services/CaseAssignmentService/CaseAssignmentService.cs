using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseAssignmentDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using mechanical.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Microsoft.VisualBasic;
using OpenCvSharp.CPlusPlus;

namespace mechanical.Services.CaseAssignmentService
{
    public class CaseAssignmentService : ICaseAssignmentService

    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;

        public CaseAssignmentService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
        }

        public async Task<List<CaseAssignmentDto>> SendForReestimation(string ReestimationReason, string selectedCollateralIds, string CenterId)
        {
            var centerId = Guid.Parse(CenterId);
            var user = await _cbeContext.Users.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Role.Name == "Maker Manager");
            if (user == null)
            {
                throw new Exception("sorry the center is not ready.");
            }
            List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();
            List<Guid> collateralIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            CaseTimeLinePostDto caseTimeLinePostDto = null;

            foreach (Guid collateralId in collateralIdList)
            {
                var collateral = await _cbeContext.Collaterals.FindAsync(collateralId);

                if (collateral != null)
                {
                    collateral.CurrentStage = "Maker Manager";
                    collateral.CurrentStatus = "New";
                    var previousCaseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == collateralId && res.UserId == user.Id).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.CaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new CaseAssignment()
                        {
                            CollateralId = collateralId,
                            UserId = user.Id,
                            Status = "New",
                            AssignmentDate = DateTime.UtcNow
                        };
                        await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
                        _cbeContext.Collaterals.Update(collateral);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
                    }


                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new CaseTimeLinePostDto()
                        {
                            CaseId = collateral.CaseId,
                            Activity = $"<strong>Collateral assigned for Re-evaluation for <a href='/UserManagment/Profile?id={user.Id}'>{user.Name}</a> Maker Manager.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {user.District.Name}.",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";

                }
                var caseReEvaluation = new CollateralReestimation
                {
                    CollateralId = collateralId,
                    Reason = ReestimationReason,
                    CreatedAt = DateTime.UtcNow,
                };
                await _cbeContext.CollateralReestimations.AddAsync(caseReEvaluation);
                await _cbeContext.SaveChangesAsync();
            }
            if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);
            return caseAssignments;
        }
        public async Task<List<CaseAssignmentDto>> SendForValuation(string selectedCollateralIds, string CenterId)
        {
            var centerId = Guid.Parse(CenterId);
            var districtName = await _cbeContext.Districts.Where(c => c.Id == centerId).Select(c => c.Name).FirstOrDefaultAsync();
            var CivilUser = await _cbeContext.Users.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Civil" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));
            var MechanicalUser = await _cbeContext.Users.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Mechanical" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));
            var AgricultureUser = await _cbeContext.Users.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Agriculture" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));

            List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();
            List<Guid> collateralIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            CaseTimeLinePostDto caseTimeLinePostDto = null;

            foreach (Guid collateralId in collateralIdList)
            {

                var collateral = await _cbeContext.Collaterals.FindAsync(collateralId);
                if (collateral != null)
                {

                    if (districtName != null && districtName == "Head Office")
                    {
                        collateral.CurrentStage = "Maker Manager";
                    }
                    else
                    {
                        collateral.CurrentStage = "District Valuation Manager";
                    }
                    collateral.CurrentStatus = "New";
                    var UserID = Guid.Empty;
                    string UserName = "";
                    string District = "";
                    if (collateral.CollateralType == "Civil")
                    {
                        if (CivilUser == null)
                        {
                            throw new Exception("sorry the Civil Evaluation center is not ready.");
                        }
                        else
                        {
                            UserID = CivilUser.Id;
                            UserName = CivilUser.Name;
                            District = CivilUser.District.Name;
                        }
                    }
                    else if (collateral.CollateralType == "Mechanical")
                    {
                        if (MechanicalUser == null)
                        {
                            throw new Exception("sorry the Mechanical Evaluation center is not ready.");
                        }
                        else
                        {
                            UserID = MechanicalUser.Id;
                            UserName = MechanicalUser.Name;
                        }
                    }
                    if (collateral.CollateralType == "Agriculture")
                    {
                        if (AgricultureUser == null)
                        {
                            throw new Exception("sorry the Agriculture Evaluation center is not ready.");
                        }
                        else
                        {
                            UserID = AgricultureUser.Id;
                            UserName = AgricultureUser.Name;
                        }
                    }
                    var previousCaseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == collateralId && res.UserId == UserID).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.CaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new CaseAssignment()
                        {
                            CollateralId = collateralId,
                            UserId = UserID,
                            Status = "New",
                            AssignmentDate = DateTime.UtcNow
                        };
                        await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
                        _cbeContext.Collaterals.Update(collateral);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
                    }


                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new CaseTimeLinePostDto()
                        {
                            CaseId = collateral.CaseId,
                            Activity = $"<strong>Collateral assigned for evaluation for <a href='/UserManagment/Profile?id={UserID}'>{UserName}</a> Maker Manager.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {districtName}.",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";

                }
            }
            if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);
            return caseAssignments;
        }

        public async Task<List<CaseAssignmentDto>> AssignMakerTeamleader(Guid userId, string selectedCollateralIds, string employeeId)
        {

            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.Users.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();

            List<Guid> collateralIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            CaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid collateralId in collateralIdList)
            {
                var collateral = await _cbeContext.Collaterals.FindAsync(collateralId);
                if (collateral != null)
                {
                    collateral.CurrentStage = user.Role.Name;
                    collateral.CurrentStatus = "New";
                    var previousCaseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == collateralId && res.UserId == user.Id).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.CaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new CaseAssignment()
                        {
                            CollateralId = collateralId,
                            UserId = UserId,
                            Status = "New",
                            AssignmentDate = DateTime.UtcNow
                        };
                        await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
                    }


                    _cbeContext.Collaterals.Update(collateral);
                    await _cbeContext.SaveChangesAsync();
                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new CaseTimeLinePostDto()
                        {
                            CaseId = collateral.CaseId,
                            Activity = $" <strong>A collateral has been assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";

                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.CaseId;
                    }
                }
                var caseassig = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                _cbeContext.Update(caseassig);
                await _cbeContext.SaveChangesAsync();

            }
            if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);

            return caseAssignments;
        }
        public async Task<List<CaseAssignmentDto>> ReAssignMakerTeamleader(Guid userId, string selectedCollateralIds, string employeeId)
        {
            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.Users.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();

            List<Guid> caseAssigmentIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            CaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid cassAssigmentId in caseAssigmentIdList)
            {
                var caseAssignment = await _cbeContext.CaseAssignments.FindAsync(cassAssigmentId);
                var collateral = await _cbeContext.Collaterals.FindAsync(caseAssignment.CollateralId);
                if (caseAssignment != null)
                {
                    if (caseAssignment.Status == "New")
                    {
                        caseAssignment.UserId = UserId;
                        caseAssignment.AssignmentDate = DateTime.UtcNow;
                    }
                    _cbeContext.CaseAssignments.Update(caseAssignment);
                    await _cbeContext.SaveChangesAsync();

                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new CaseTimeLinePostDto()
                        {
                            CaseId = collateral.CaseId,
                            Activity = $" <strong>A collateral has been Re-assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";
                    caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.CaseId;
                    }
                }
                var caseassig = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                _cbeContext.Update(caseassig);

            }
            if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);

            return caseAssignments;
        }
        public async Task<List<CaseAssignmentDto>> AssignCheckerTeamleader(Guid userId, string selectedCollateralIds, string employeeId)
        {

            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.Users.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();

            List<Guid> collateralIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            CaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid collateralId in collateralIdList)
            {
                var collateral = await _cbeContext.Collaterals.FindAsync(collateralId);
                if (collateral != null)
                {
                    collateral.CurrentStage = user.Role.Name;
                    collateral.CurrentStatus = "New";
                    var previousCaseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == collateralId && res.UserId == user.Id).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.CaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new CaseAssignment()
                        {
                            CollateralId = collateralId,
                            UserId = user.Id,
                            Status = "New",
                            AssignmentDate = DateTime.UtcNow
                        };
                        await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
                    }


                    _cbeContext.Collaterals.Update(collateral);
                    await _cbeContext.SaveChangesAsync();
                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new CaseTimeLinePostDto()
                        {
                            CaseId = collateral.CaseId,
                            Activity = $" <strong>A collateral has been assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Checker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";

                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.CaseId;
                    }
                }
                var caseassig = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                 _cbeContext.Update(caseassig);
                await _cbeContext.SaveChangesAsync();

            }
            if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);

            return caseAssignments;
        }
        public async Task<List<CaseAssignmentDto>> ReAssignCheckerTeamleader(Guid userId, string selectedCollateralIds, string employeeId)
        {

            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.Users.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();

            List<Guid> caseAssigmentIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            CaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid cassAssigmentId in caseAssigmentIdList)
            {
                var caseAssignment = await _cbeContext.CaseAssignments.FindAsync(cassAssigmentId);
                var collateral = await _cbeContext.Collaterals.FindAsync(caseAssignment.CollateralId);
                if (caseAssignment != null)
                {
                    if (caseAssignment.Status == "New")
                    {
                        caseAssignment.UserId = UserId;
                        caseAssignment.AssignmentDate = DateTime.UtcNow;
                    }
                    _cbeContext.CaseAssignments.Update(caseAssignment);
                    await _cbeContext.SaveChangesAsync();

                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new CaseTimeLinePostDto()
                        {
                            CaseId = collateral.CaseId,
                            Activity = $" <strong>A collateral has been Re-assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Checker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";
                    caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.CaseId;
                    }
                }
                var caseassig = await _cbeContext.CaseAssignments.Where(res => res.UserId == userId && res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                _cbeContext.Update(caseassig);

            }
            if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);

            return caseAssignments;
        }

        //public async Task<CaseAssignmentDto> CreateCaseAssignment(CaseAssignmentDto caseAssignmentDto)
        //{
        //    var caseAssignment = _mapper.Map<CaseAssignment>(caseAssignmentDto);
        //    caseAssignment.status = "New";
        //    caseAssignment.AssignmentDate = DateTime.UtcNow;
        //    await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
        //    await _cbeContext.SaveChangesAsync();
        //    return caseAssignmentDto;
        //}

        //    var casse = await _cbeContext.Cases.FindAsync(collateralCaseId);
        //    //if (casse != null && casse.CurrentStatus == "New")
        //    //{
        //    //    casse.CurrentStage = "Maker";
        //    //    casse.CurrentStatus = "Pending";
        //    //    _cbeContext.Cases.Update(casse);
        //    //    await _cbeContext.SaveChangesAsync();
        //    //}

        //    if(caseTimeLinePostDto != null)  await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);
        //    return caseAssignments;
        //}
        //public async Task<List<CaseAssignmentDto>> AssignCheckerTeamleader(string selectedCollateralIds, string employeeId)
        //{
        //    Guid collateralCaseId = Guid.Empty;
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var UserId = Guid.Parse(employeeId);
        //    var user = await _cbeContext.Users.FindAsync(UserId);
        //    List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();

        //    List<Guid> collateralIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
        //    CaseTimeLinePostDto caseTimeLinePostDto = null;
        //    foreach (Guid collateralId in collateralIdList)
        //    {
        //        var collateral = await _cbeContext.Collaterals.FindAsync(collateralId);
        //        if (collateral != null)
        //        {
        //            collateral.Status = "Checker Teamleader";
        //            var caseAssignment = new CaseAssignment()
        //            {
        //                CaseId = collateral.CaseId,
        //                CollateralId = collateralId,
        //                UserId = UserId,
        //                status = "Checker TeamLeader",
        //                AssignmentDate = DateTime.UtcNow
        //            };
        //            await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
        //            await _cbeContext.SaveChangesAsync();

        //            _cbeContext.Collaterals.Update(collateral);
        //            await _cbeContext.SaveChangesAsync();
        //            if (caseTimeLinePostDto == null)
        //            {
        //                caseTimeLinePostDto = new CaseTimeLinePostDto()
        //                {
        //                    CaseId = collateral.CaseId,
        //                    Activity = $" <strong>A collateral has been assigned for {user.Name} Team Leader. </strong> <br>",
        //                    CurrentStage = "Checker Manager"
        //                };
        //            }
        //            caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";
        //            caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
        //            if (collateralCaseId == Guid.Empty)
        //            {
        //                collateralCaseId = collateral.CaseId;
        //            }
        //        }

        //    }

        //    if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);
        //    return caseAssignments;
        //}
        //public async Task<List<CaseAssignmentDto>> AssignCheckerOfficer(string selectedCollateralIds, string employeeId)
        //{
        //    Guid collateralCaseId = Guid.Empty;
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var UserId = Guid.Parse(employeeId);
        //    var user = await _cbeContext.Users.FindAsync(UserId);
        //    List<CaseAssignmentDto> caseAssignments = new List<CaseAssignmentDto>();

        //    List<Guid> collateralIdList = selectedCollateralIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
        //    CaseTimeLinePostDto caseTimeLinePostDto = null;
        //    foreach (Guid collateralId in collateralIdList)
        //    {
        //        var caseAssig = await _cbeContext.CaseAssignments.FirstOrDefaultAsync(ca => ca.CollateralId == collateralId && ca.status == "Checker Teamleader" && ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")));
        //        if (caseAssig != null)
        //        {
        //            caseAssig.status = "Pending";
        //            _cbeContext.CaseAssignments.Update(caseAssig);
        //        }
        //        var collateral = await _cbeContext.Collaterals.FindAsync(collateralId);
        //        if (collateral != null)
        //        {
        //            collateral.Status = "Checker Officer";
        //            var caseAssignment = new CaseAssignment()
        //            {
        //                CaseId = collateral.CaseId,
        //                CollateralId = collateralId,
        //                UserId = UserId,
        //                status = "Checker Officer",
        //                AssignmentDate = DateTime.UtcNow
        //            };
        //            await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
        //            await _cbeContext.SaveChangesAsync();

        //            _cbeContext.Collaterals.Update(collateral);
        //            await _cbeContext.SaveChangesAsync();
        //            if (caseTimeLinePostDto == null)
        //            {
        //                caseTimeLinePostDto = new CaseTimeLinePostDto()
        //                {
        //                    CaseId = collateral.CaseId,
        //                    Activity = $" <strong>A collateral has been assigned for {user.Name} Checker Officer. </strong> <br>",
        //                    CurrentStage = "Checker Manager"
        //                };
        //            }
        //            caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}. <br>";
        //            caseAssignments.Add(_mapper.Map<CaseAssignmentDto>(caseAssignment));
        //            if (collateralCaseId == Guid.Empty)
        //            {
        //                collateralCaseId = collateral.CaseId;
        //            }
        //        }

        //    }
        //    if (caseTimeLinePostDto != null) await _caseTimeLineService.CreateCaseTimeLine(caseTimeLinePostDto);
        //    return caseAssignments;
        //}
    }

}
