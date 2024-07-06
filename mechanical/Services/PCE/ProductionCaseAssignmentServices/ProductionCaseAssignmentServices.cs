using AutoMapper;
using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.CaseAssignmentDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.PCE.Dto.ProductionCaseAssignmentDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.PCE.PCECaseTimeLineService;
using Microsoft.EntityFrameworkCore;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.ProductionCaseAssignmentServices
{
    public class ProductionCaseAssignmentServices : IProductionCaseAssignmentServices
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IProductionCaseTimeLineService _productionCaseTimeLineService;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;


        public ProductionCaseAssignmentServices(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            //_productionCaseTimeLineService = productionCaseTimeLineService;
            _IPCECaseTimeLineService = IPCECaseTimeLineService;

        }
        public async Task<List<ProductionCaseAssignmentDto>> AssignProductionCheckerTeamleader(Guid userId, string selectedProductionIds, string employeeId)
        {
            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<ProductionCaseAssignmentDto> caseAssignments = new List<ProductionCaseAssignmentDto>();

            List<Guid> collateralIdList = selectedProductionIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            PCECaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid collateralId in collateralIdList)
            {
                var collateral = await _cbeContext.ProductionCapacities.FindAsync(collateralId);
                if (collateral != null)
                {
                    collateral.CurrentStage = user.Role.Name;
                    collateral.CurrentStatus = "New";
                    var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.PCECaseId == collateralId && res.UserId == user.Id).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new ProductionCaseAssignment()
                        {
                            PCECaseId = collateralId,
                            UserId = user.Id,
                            Status = "New",
                            AssignmentDate = DateTime.Now
                        };
                        await _cbeContext.ProductionCaseAssignments.AddAsync(caseAssignment);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(caseAssignment));
                    }


                    _cbeContext.ProductionCapacities.Update(collateral);
                    await _cbeContext.SaveChangesAsync();
                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new PCECaseTimeLinePostDto()
                        {
                            CaseId = collateral.PCECaseId,
                            Activity = $" <strong>A collateral has been assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Checker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {collateral.Type}. <br>";

                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.PCECaseId;
                    }
                }
                var caseassig = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.PCECaseId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                _cbeContext.Update(caseassig);

            }
            if (caseTimeLinePostDto != null) await _IPCECaseTimeLineService.PCECaseTimeLine(caseTimeLinePostDto);

            return caseAssignments;
        }


        public async Task<List<ProductionCaseAssignmentDto>> AssignProductMakerTeamleader(Guid userId, string selectedProductionIds, string employeeId)
        {
            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<ProductionCaseAssignmentDto> caseAssignments = new List<ProductionCaseAssignmentDto>();

            List<Guid> collateralIdList = selectedProductionIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            PCECaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid collateralId in collateralIdList)
            {
                var collateral = await _cbeContext.ProductionCapacities.FindAsync(collateralId);
                if (collateral != null)
                {
                    collateral.CurrentStage = user.Role.Name;
                    collateral.CurrentStatus = "New";
                    var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.PCECaseId == collateralId && res.UserId == user.Id).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new ProductionCaseAssignment()
                        {
                            PCECaseId = collateralId,
                            UserId = UserId,
                            Status = "New",
                            AssignmentDate = DateTime.Now
                        };
                        await _cbeContext.ProductionCaseAssignments.AddAsync(caseAssignment);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(caseAssignment));
                    }


                    _cbeContext.ProductionCapacities.Update(collateral);
                    await _cbeContext.SaveChangesAsync();
                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new PCECaseTimeLinePostDto()
                        {
                            CaseId = collateral.PCECaseId,
                            Activity = $" <strong>A collateral has been assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {collateral.Type}. <br>";

                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.PCECaseId;
                    }
                }
                var caseassig = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.PCECaseId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                _cbeContext.Update(caseassig);
                await _cbeContext.SaveChangesAsync();

            }
            if (caseTimeLinePostDto != null) await _IPCECaseTimeLineService.PCECaseTimeLine(caseTimeLinePostDto);

            return caseAssignments;
        }


        public async Task<List<ProductionCaseAssignmentDto>> ReAssignProductionCheckerTeamleader(Guid userId, string selectedProductionIds, string employeeId)
        {
            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<ProductionCaseAssignmentDto> caseAssignments = new List<ProductionCaseAssignmentDto>();

            List<Guid> caseAssigmentIdList = selectedProductionIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            PCECaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid cassAssigmentId in caseAssigmentIdList)
            {
                var caseAssignment = await _cbeContext.ProductionCaseAssignments.FindAsync(cassAssigmentId);
                var collateral = await _cbeContext.ProductionCapacities.FindAsync(caseAssignment.PCECaseId);
                if (caseAssignment != null)
                {
                    if (caseAssignment.Status == "New")
                    {
                        caseAssignment.UserId = UserId;
                        caseAssignment.AssignmentDate = DateTime.Now;
                    }
                    _cbeContext.ProductionCaseAssignments.Update(caseAssignment);
                    await _cbeContext.SaveChangesAsync();

                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new PCECaseTimeLinePostDto()
                        {
                            CaseId = collateral.PCECaseId,
                            Activity = $" <strong>A collateral has been Re-assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Checker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {collateral.Type}. <br>";
                    caseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(caseAssignment));
                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.PCECaseId;
                    }
                }
                var caseassig = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.PCECaseId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                _cbeContext.Update(caseassig);

            }
            if (caseTimeLinePostDto != null) await _IPCECaseTimeLineService.PCECaseTimeLine(caseTimeLinePostDto);

            return caseAssignments;
        }

        public async Task<List<ProductionCaseAssignmentDto>> ReAssignProductionMakerTeamleader(Guid userId, string selectedProductionIds, string employeeId)
        {
            Guid collateralCaseId = Guid.Empty;
            var UserId = Guid.Parse(employeeId);
            var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);
            List<ProductionCaseAssignmentDto> caseAssignments = new List<ProductionCaseAssignmentDto>();

            List<Guid> caseAssigmentIdList = selectedProductionIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            PCECaseTimeLinePostDto caseTimeLinePostDto = null;
            foreach (Guid cassAssigmentId in caseAssigmentIdList)
            {
                var caseAssignment = await _cbeContext.ProductionCaseAssignments.FindAsync(cassAssigmentId);
                var collateral = await _cbeContext.ProductionCapacities.FindAsync(caseAssignment.PCECaseId);
                if (caseAssignment != null)
                {
                    if (caseAssignment.Status == "New")
                    {
                        caseAssignment.UserId = UserId;
                        caseAssignment.AssignmentDate = DateTime.Now;
                    }
                    _cbeContext.ProductionCaseAssignments.Update(caseAssignment);
                    await _cbeContext.SaveChangesAsync();

                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new PCECaseTimeLinePostDto()
                        {
                            CaseId = collateral.PCECaseId,
                            Activity = $" <strong>A collateral has been Re-assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {collateral.Type}. <br>";
                    caseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(caseAssignment));
                    if (collateralCaseId == Guid.Empty)
                    {
                        collateralCaseId = collateral.PCECaseId;
                    }
                }
                var caseassig = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.PCECaseId == collateral.Id).FirstOrDefaultAsync();
                caseassig.Status = "Pending";
                _cbeContext.Update(caseassig);

            }
            if (caseTimeLinePostDto != null) await _IPCECaseTimeLineService.PCECaseTimeLine(caseTimeLinePostDto);
            return caseAssignments;
        }



        public async Task<List<ProductionCaseAssignmentDto>> SendProductionForReestimation(string ReestimationReason, string selectedProductionIds, string CenterId)
        {
            var centerId = Guid.Parse(CenterId);
            var user = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Role.Name == "Maker Manager");
            if (user == null)
            {
                throw new Exception("sorry the center is not ready.");
            }
            List<ProductionCaseAssignmentDto> caseAssignments = new List<ProductionCaseAssignmentDto>();
            List<Guid> collateralIdList = selectedProductionIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            PCECaseTimeLinePostDto caseTimeLinePostDto = null;

            foreach (Guid collateralId in collateralIdList)
            {
                var collateral = await _cbeContext.ProductionCapacities.FindAsync(collateralId);

                if (collateral != null)
                {
                    collateral.CurrentStage = "Maker Manager";
                    collateral.CurrentStatus = "New";
                    var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.PCECaseId == collateralId && res.UserId == user.Id).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new ProductionCaseAssignment()
                        {
                            PCECaseId = collateralId,
                            UserId = user.Id,
                            Status = "New",
                            AssignmentDate = DateTime.Now
                        };
                        await _cbeContext.ProductionCaseAssignments.AddAsync(caseAssignment);
                        _cbeContext.ProductionCapacities.Update(collateral);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(caseAssignment));
                    }


                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new PCECaseTimeLinePostDto()
                        {
                            CaseId = collateral.PCECaseId,
                            Activity = $"<strong>Collateral assigned for Re-evaluation for <a href='/UserManagment/Profile?id={user.Id}'>{user.Name}</a> Maker Manager.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {user.District.Name}.",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {collateral.Type}. <br>";

                }
                var caseReEvaluation = new CollateralReestimation
                {
                    CollateralId = collateralId,
                    Reason = ReestimationReason,
                    CreatedAt = DateTime.Now,
                };
                await _cbeContext.CollateralReestimations.AddAsync(caseReEvaluation);
                await _cbeContext.SaveChangesAsync();
            }
            if (caseTimeLinePostDto != null) await _IPCECaseTimeLineService.PCECaseTimeLine(caseTimeLinePostDto);
            return caseAssignments;
        }
    

        public async Task<List<ProductionCaseAssignmentDto>> SendProductionForValuation(string selectedProductionIds, string CenterId)
        {
            var centerId = Guid.Parse(CenterId);
            var districtName = await _cbeContext.Districts.Where(c => c.Id == centerId).Select(c => c.Name).FirstOrDefaultAsync();
            var CivilUser = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Civil" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));
            var MechanicalUser = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Mechanical" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));
            var AgricultureUser = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Agriculture" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));

            List<ProductionCaseAssignmentDto> caseAssignments = new List<ProductionCaseAssignmentDto>();
            List<Guid> collateralIdList = selectedProductionIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
            PCECaseTimeLinePostDto caseTimeLinePostDto = null;

            foreach (Guid collateralId in collateralIdList)
            {

                var collateral = await _cbeContext.ProductionCapacities.FindAsync(collateralId);
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
                    if (collateral.ProductionType == "Manufacturing")
                    {
                        if (CivilUser == null)
                        {
                            throw new Exception("sorry the Manufacturing Evaluation center is not ready.");
                        }
                        else
                        {
                            UserID = CivilUser.Id;
                            UserName = CivilUser.Name;
                            District = CivilUser.District.Name;
                        }
                    }
                    else if (collateral.ProductionType == "Plant")
                    {
                        if (MechanicalUser == null)
                        {
                            throw new Exception("sorry the Plant Evaluation center is not ready.");
                        }
                        else
                        {
                            UserID = MechanicalUser.Id;
                            UserName = MechanicalUser.Name;
                        }
                    }
                    //if (collateral.CollateralType == "Agriculture")
                    //{
                    //    if (AgricultureUser == null)
                    //    {
                    //        throw new Exception("sorry the Agriculture Evaluation center is not ready.");
                    //    }
                    //    else
                    //    {
                    //        UserID = AgricultureUser.Id;
                    //        UserName = AgricultureUser.Name;
                    //    }
                    //}
                    var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.PCECaseId == collateralId && res.UserId == UserID).FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        await _cbeContext.SaveChangesAsync();
                    }
                    else
                    {
                        var caseAssignment = new CaseAssignment()
                        {
                            CollateralId = collateralId,
                            UserId = UserID,
                            Status = "New",
                            AssignmentDate = DateTime.Now
                        };
                        await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
                        _cbeContext.ProductionCapacities.Update(collateral);
                        await _cbeContext.SaveChangesAsync();
                        caseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(caseAssignment));
                    }


                    if (caseTimeLinePostDto == null)
                    {
                        caseTimeLinePostDto = new PCECaseTimeLinePostDto()
                        {
                            CaseId = collateral.PCECaseId,
                            Activity = $"<strong>Collateral assigned for evaluation for <a href='/UserManagment/Profile?id={UserID}'>{UserName}</a> Maker Manager.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {districtName}.",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    caseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {collateral.Type}. <br>";

                }
            }
            if (caseTimeLinePostDto != null) await _IPCECaseTimeLineService.PCECaseTimeLine(caseTimeLinePostDto);
            return caseAssignments;
        
    }
    } 
}
