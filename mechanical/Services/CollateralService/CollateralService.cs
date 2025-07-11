using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using mechanical.Models;
using mechanical.Models.Dto.Correction;
using Microsoft.VisualBasic;
using Azure;
using mechanical.Models.Enum;

namespace mechanical.Services.CollateralService
{
    public class CollateralService : ICollateralService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IUploadFileService _uploadFileService;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CollateralService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _uploadFileService = uploadFileService;
            _caseTimeLineService = caseTimeLineService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Collateral> CreateCollateral(Guid userId, Guid caseId, CollateralPostDto createCollateralDto)
        {
            var collateral = _mapper.Map<Collateral>(createCollateralDto);
            if (collateral.Category == MechanicalCollateralCategory.CMAMachinery)
            {
                collateral.PlateNo = createCollateralDto.CPlateNo;
            }

            collateral.Id = Guid.NewGuid();
            collateral.CaseId = caseId;
            try
            {
                if (collateral.Category == MechanicalCollateralCategory.MOV || collateral.Category == MechanicalCollateralCategory.CMAMachinery)
                {
                    await this.UploadFile(userId, "Proforma inovice", collateral, createCollateralDto.PackingList);
                    await this.UploadFile(userId, "Title Deed Certificate", collateral, createCollateralDto.TitleDeed);
                }
                else if (collateral.Category == MechanicalCollateralCategory.IBFEqupment)
                {
                    await this.UploadFile(userId, "Packing List", collateral, createCollateralDto.PackingList);
                    await this.UploadFile(userId, "LHC", collateral, createCollateralDto.TitleDeed);
                }

                await this.UploadFile(userId, "Commercial Invoice", collateral, createCollateralDto.CommercialInvoice);
                await this.UploadFile(userId, "Custom Declaration", collateral, createCollateralDto.CustomDeclaration);

                await this.UploadFile(userId, "Sales Document", collateral, createCollateralDto.SalesDocument);
                if (createCollateralDto.OtherDocument != null)
                {
                    foreach (var otherDocument in createCollateralDto.OtherDocument)
                    {
                        await this.UploadFile(userId, "Other Supportive Document", collateral, otherDocument);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("unable to upload file");
            }
            collateral.CreationDate = DateTime.UtcNow;
            collateral.CreatedById = userId;
            collateral.CurrentStage = "Relation Manager";
            collateral.CurrentStatus = "New";
            collateral.CollateralType = "Mechanical";

            await _cbeContext.Collaterals.AddAsync(collateral);
            await _cbeContext.SaveChangesAsync();

            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong>A new collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Relation Manager"
            });

            return collateral;
        }
        public async Task<bool> CreateMOFile(Guid userId, Guid caseId, string DocumentType, IEnumerable<IFormFile>? Document)
        {
            try
            {
                if (Document != null)
                {
                    foreach (var otherDocument in Document)
                    {
                        var moDocuments = new CreateFileDto()
                        {
                            File = otherDocument ?? throw new ArgumentNullException(nameof(otherDocument)),
                            CaseId = caseId,
                            Category = DocumentType
                        };
                        await _uploadFileService.CreateUploadFile(userId, moDocuments);
                    }
                    return true;
                }
                return true;
            }
            catch (Exception)
            {
                throw new Exception("unable to upload file");
            }

        }

        public async Task<Collateral> CreateCivilCollateral(Guid userId, Guid caseId, CivilCollateralPostDto createCivilCollateralDto)
        {
            var collateral = _mapper.Map<Collateral>(createCivilCollateralDto);
            collateral.Id = Guid.NewGuid();
            collateral.CaseId = caseId;
            try
            {
                await this.UploadFile(userId, "Title Deed Certificate", collateral, createCivilCollateralDto.UploadLHC);
                await this.UploadFile(userId, "Commercial Invoice", collateral, createCivilCollateralDto.UploadSitePlan);
                await this.UploadFile(userId, "Custom Declaration", collateral, createCivilCollateralDto.LeaseAgreement);
                await this.UploadFile(userId, "Packing List", collateral, createCivilCollateralDto.CurrentLeasePaymentReceipt);
                await this.UploadFile(userId, "Sales Document", collateral, createCivilCollateralDto.CurrentLandRevenueTaxPaymentReceipt);
                await this.UploadFile(userId, "Applicants Exact Holding Confirmed By Association", collateral, createCivilCollateralDto.ApplicantsExactHoldingConfirmedByAssociation);
                await this.UploadFile(userId, "Letter From The Cooperative Association With Minutes", collateral, createCivilCollateralDto.LetterFromTheCooperativeAssociationWithMinutes);
                await this.UploadFile(userId, "Valid Construction Permit", collateral, createCivilCollateralDto.ValidConstructionPermit);
                await this.UploadFile(userId, "Bill Of Quantity", collateral, createCivilCollateralDto.BillOfQuantity);
                await this.UploadFile(userId, "Renewed Consultant Professional License", collateral, createCivilCollateralDto.RenewedConsultantProfessionalLicense);
                if (createCivilCollateralDto.OtherDocument != null)
                {
                    foreach (var otherDocument in createCivilCollateralDto.OtherDocument)
                    {
                        await this.UploadFile(userId, "Other Supportive Document", collateral, otherDocument);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("unable to upload file");
            }
            collateral.CreationDate = DateTime.UtcNow;
            collateral.CreatedById = userId;
            collateral.CurrentStage = "Relation Manager";
            collateral.CurrentStatus = "New";
            collateral.CollateralType = "Civil";

            await _cbeContext.Collaterals.AddAsync(collateral);
            await _cbeContext.SaveChangesAsync();

            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong>A new collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Relation Manager"
            });

            return collateral;
        }
        public async Task<Collateral> CreateAgricultureCollateral(Guid userId, Guid caseId, AgricultureCollateralPostDto createAgricultureCollateralDto)
        {
            var collateral = _mapper.Map<Collateral>(createAgricultureCollateralDto);
            collateral.Id = Guid.NewGuid();
            collateral.CaseId = caseId;
            try
            {
                await this.UploadFile(userId, "Title Deed Certificate", collateral, createAgricultureCollateralDto.UploadLHC);
                await this.UploadFile(userId, "Commercial Invoice", collateral, createAgricultureCollateralDto.UploadSitePlan);
                await this.UploadFile(userId, "Custom Declaration", collateral, createAgricultureCollateralDto.LeaseAgreement);
                await this.UploadFile(userId, "Packing List", collateral, createAgricultureCollateralDto.CurrentLeasePaymentReceipt);
                if (createAgricultureCollateralDto.OtherDocument != null)
                {
                    foreach (var otherDocument in createAgricultureCollateralDto.OtherDocument)
                    {
                        await this.UploadFile(userId, "Other Supportive Document", collateral, otherDocument);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("unable to upload file");
            }
            collateral.CreationDate = DateTime.UtcNow;
            collateral.CreatedById = userId;
            collateral.CurrentStage = "Relation Manager";
            collateral.CurrentStatus = "New";
            collateral.CollateralType = "Agriculture";

            await _cbeContext.Collaterals.AddAsync(collateral);
            await _cbeContext.SaveChangesAsync();

            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong>A new collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Relation Manager"
            });

            return collateral;
        }

        public async Task<IEnumerable<CollateralAssignmentDto>> GetMyAssignmentCollateral(Guid UserId, Guid CaseId)
        {
            var userSupervised = await _cbeContext.Users.Where(res => res.SupervisorId == UserId).ToListAsync();
            var collateralAssigmentDtos = new List<CollateralAssignmentDto>();

            foreach (var item in userSupervised)
            {
                var caseAssignment = await _cbeContext.CaseAssignments.Include(x => x.User).Include(x => x.Collateral).Where(res => res.UserId == item.Id && res.Collateral.CaseId == CaseId).ToListAsync();
                caseAssignment = caseAssignment.DistinctBy(res => res.CollateralId).ToList();
                foreach (var items in caseAssignment)
                {
                    var collateralAssigmentDto = new CollateralAssignmentDto
                    {
                        CollateralId = items.CollateralId,
                        CaseId = CaseId,
                        PropertyOwner = items.Collateral.PropertyOwner,
                        CaseAssigmentId = items.Id,
                        Role = items.Collateral.Role,
                        Type = EnumToDisplayName(items.Collateral.Type),
                        Category = EnumToDisplayName(items.Collateral.Category),
                        User = items.User.Name,
                        AssignmentDate = items.AssignmentDate,
                        Status = items.Status,
                    };
                    collateralAssigmentDtos.Add(collateralAssigmentDto);
                }
            }
            return collateralAssigmentDtos;
        }
        string EnumToDisplayName<TEnum>(TEnum enumValue)
        {
            return (typeof(TEnum).GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute)?.Name ?? enumValue.ToString();
        }

        public async Task<Collateral> EditCollateral(Guid userId, Guid CollaterlId, CollateralPostDto createCollateralDto)
        {
            if (createCollateralDto.Category == MechanicalCollateralCategory.CMAMachinery)
            {
                createCollateralDto.PlateNo = createCollateralDto.CPlateNo;
            }

            var collateral = await _cbeContext.Collaterals.FindAsync(CollaterlId);
            if (collateral == null)
            {
                throw new Exception("collateral not Found");
            }
            if (collateral.CreatedById != userId)
            {
                throw new Exception("you don't have permission");
            }
            if (collateral.CurrentStage == "Relation Manager")
            {
                createCollateralDto.CaseId = collateral.CaseId;
                createCollateralDto.CollateralType = collateral.CollateralType;
                _mapper.Map(createCollateralDto, collateral);
                _cbeContext.Collaterals.Update(collateral);
                await _cbeContext.SaveChangesAsync();
                return collateral;
            }
            throw new Exception("unable to Edit collateral");
        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetCollaterals(Guid CaseId)
        {
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && (res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetRejectedCollaterals(Guid CaseId)
        {
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && (res.CurrentStatus == "Reject" && res.CurrentStage == "Relation Manager")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetPendCollaterals(Guid CaseId)
        {
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && (res.CurrentStage != "Relation Manager" && res.CurrentStatus != "Complete")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetRmComCollaterals(Guid CaseId)
        {
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.CurrentStage == "Checker officer" && res.CurrentStatus == "Complete").ToListAsync();
            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        public async Task<ReturnCollateralDto> GetCollateral(Guid userId, Guid id)
        {
            var collateral = await _cbeContext.Collaterals
                           .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<ReturnCollateralDto>(collateral);
        }

        private async Task UploadFile(Guid userId, string Category, Collateral collateral, IFormFile? file)
        {
            if (file != null)
            {
                await _uploadFileService.CreateUploadFile(userId, new CreateFileDto()
                {
                    File = file,
                    CaseId = collateral.CaseId,
                    CollateralId = collateral.Id,
                    Category = Category
                });
            }
        }
        // public async Task<IEnumerable<ReturnCollateralDto>> GetCompleteCollaterals(Guid CaseId)
        // {
        //     var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Status == "Complete").ToListAsync();
        //     return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        // }

        // public async Task<IEnumerable<ReturnCollateralDto>> GetMOCollaterals(Guid CaseId)
        // {
        //     var httpContext = _httpContextAccessor.HttpContext;
        //     List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Maker Officer" && ca.CaseId == CaseId).ToListAsync();

        //     List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
        //     if (caseAssignments != null)
        //     {
        //         foreach (var caseAssignment in caseAssignments)
        //         {
        //             var collatearal = await _cbeContext.Collaterals.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.CollateralId && ca.Status !="Complete");
        //             if (collatearal != null)
        //             {
        //                 mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
        //             }
        //         }
        //     }
        //     return mTLreturnCollateralDtos;
        // }
        // public async Task<IEnumerable<ReturnCollateralDto>> GetCOCollaterals(Guid CaseId)
        // {
        //     var httpContext = _httpContextAccessor.HttpContext;
        //     List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Checker Officer" && ca.CaseId == CaseId).ToListAsync();

        //     List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
        //     if (caseAssignments != null)
        //     {
        //         foreach (var caseAssignment in caseAssignments)
        //         {
        //             var collatearal = await _cbeContext.Collaterals.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.CollateralId && ca.Status != "Complete");
        //             if (collatearal != null)
        //             {
        //                 mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
        //             }
        //         }
        //     }
        //     return mTLreturnCollateralDtos;
        // }
        public async Task<IEnumerable<ReturnCollateralDto>> GetMMCollaterals(Guid userId, Guid CaseId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Collateral.CaseId == CaseId && res.Status == "New").ToListAsync();
            var collaterals = caseAssignments.Select(res => res.Collateral);
            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetMMCompleteCollaterals(Guid userId, Guid CaseId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Collateral.CaseId == CaseId && res.Status == "Complete").ToListAsync();
            var collaterals = caseAssignments.Select(res => res.Collateral);
            var returnCollateralDtos = _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
            foreach (var collateral in returnCollateralDtos)
            {
                if (collateral.Category == "MOTOR VEHICLE")
                {
                    var mov = await _cbeContext.MotorVehicles.Where(res => res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                    if (mov != null)
                    {
                        collateral.MechanicalEqpmntName = mov.MechanicalEqpmntName;
                    }
                }
                else if (collateral.Category == "CONST, MNG & AGR MACHINERY")
                {
                    var mov = await _cbeContext.ConstMngAgrMachineries.Where(res => res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                    if (mov != null)
                    {
                        collateral.MechanicalEqpmntName = mov.MechanicalEqpmntName;
                    }
                }
                else if (collateral.Category == "IND (Mfg) & BLDG FACILITY EQUIPMENT")
                {
                    var mov = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                    if (mov != null)
                    {
                        collateral.MechanicalEqpmntName = mov.MechanicalEqpmntName;
                    }
                }
            }
            return returnCollateralDtos;
        }

        public async Task<IEnumerable<ReturnCollateralDto>> GetCMCollaterals(Guid userId, Guid CaseId)
        {
            var user = await _cbeContext.Users.Include(res => res.District).FirstOrDefaultAsync(res => res.Id == userId);
            if (user.District.Name == "Head Office")
            {
                var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Collateral.CaseId == CaseId && res.Status == "New").ToListAsync();
                var collaterals = caseAssignments.Select(res => res.Collateral);
                var returnCollateralDtos = _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
                foreach (var collateral in returnCollateralDtos)
                {
                    if (collateral.Category == "MOTOR VEHICLE")
                    {
                        var mov = await _cbeContext.MotorVehicles.Where(res => res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                        if (mov != null)
                        {
                            collateral.MechanicalEqpmntName = mov.MechanicalEqpmntName;
                        }
                    }
                    else if (collateral.Category == "CONST, MNG & AGR MACHINERY")
                    {
                        var mov = await _cbeContext.ConstMngAgrMachineries.Where(res => res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                        if (mov != null)
                        {
                            collateral.MechanicalEqpmntName = mov.MechanicalEqpmntName;
                        }
                    }
                    else if (collateral.Category == "IND (Mfg) & BLDG FACILITY EQUIPMENT")
                    {
                        var mov = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.CollateralId == collateral.Id).FirstOrDefaultAsync();
                        if (mov != null)
                        {
                            collateral.MechanicalEqpmntName = mov.MechanicalEqpmntName;
                        }
                    }
                }
                return returnCollateralDtos;

            }
            else
            {
                var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Collateral.CaseId == CaseId && res.Status == "Checker New").ToListAsync();
                var collaterals = caseAssignments.Select(res => res.Collateral);
                return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
            }

        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetRemarkCollaterals(Guid userId, Guid CaseId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Collateral.CaseId == CaseId && res.Status.Contains("Remark")).ToListAsync();
            var collaterals = caseAssignments.Select(res => res.Collateral);
            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetRmRemarkCollaterals(Guid userId, Guid CaseId)
        {
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.CurrentStatus.Contains("Remark")).ToListAsync();

            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        public async Task<IEnumerable<ReturnCollateralDto>> GetMMPendCollaterals(Guid userId, Guid CaseId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Collateral.CaseId == CaseId && res.Status == "Pending").ToListAsync();
            var collaterals = caseAssignments.Select(res => res.Collateral);
            return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collaterals);
        }
        //public async Task<IEnumerable<ReturnCollateralDto>> GetMTLCollaterals(Guid CaseId)
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Maker TeamLeader" && ca.CaseId == CaseId).ToListAsync();

        //    List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
        //    if (caseAssignments != null)
        //    {
        //        foreach (var caseAssignment in caseAssignments)
        //        {
        //            var collatearal = await _cbeContext.Collaterals.FindAsync(caseAssignment.CollateralId);
        //            if (collatearal != null)
        //            {
        //                mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
        //            }
        //        }
        //    }
        //    return mTLreturnCollateralDtos;
        //}
        // public async Task<IEnumerable<ReturnCollateralDto>> GetCTLCollaterals(Guid CaseId)
        // {
        //     var httpContext = _httpContextAccessor.HttpContext;
        //     List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && ca.status == "Checker TeamLeader" && ca.CaseId == CaseId).ToListAsync();

        //     List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
        //     if (caseAssignments != null)
        //     {
        //         foreach (var caseAssignment in caseAssignments)
        //         {
        //             var collatearal = await _cbeContext.Collaterals.FindAsync(caseAssignment.CollateralId);
        //             if (collatearal != null)
        //             {
        //                 mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
        //             }
        //         }
        //     }
        //     return mTLreturnCollateralDtos;
        // }
        // public async Task<IEnumerable<ReturnCollateralDto>> GetCMCollaterals(Guid CaseId)
        // {
        //     var collateral = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Status == "Checker").ToListAsync();
        //     return _mapper.Map<IEnumerable<ReturnCollateralDto>>(collateral);
        // }
        // public async Task<ReturnCollateralDto> GetCollateral(Guid Id)
        // {
        //     var collateral = await _cbeContext.Collaterals.FirstOrDefaultAsync(res => res.Id == Id);
        //     return _mapper.Map<ReturnCollateralDto>(collateral);
        // }
        // public async Task<ReturnCollateralDto> GetMORetunedCollaterals(Guid Id)
        // {
        //     var collateral = await _cbeContext.Collaterals.FirstOrDefaultAsync(res => res.Id == Id&& res.Status=="correction");
        //     return _mapper.Map<ReturnCollateralDto>(collateral);
        // }

        // public async Task<IEnumerable<ReturnFileDto>> GetCollateralFile(Guid CollateralId)
        // {
        //     List<ReturnFileDto> returnFileDtos = new List<ReturnFileDto>();
        //     var collateral = await _cbeContext.Collaterals.FindAsync(CollateralId);
        //     if (collateral != null)
        //     {
        //         if (collateral.TitleDeedId != null) returnFileDtos.Add(await _uploadFileService.GetUploadFile(collateral.TitleDeedId));
        //         if (collateral.CommercialInvoiceId != null) returnFileDtos.Add(await _uploadFileService.GetUploadFile(collateral.CommercialInvoiceId));
        //         if (collateral.CustomDeclarationId != null) returnFileDtos.Add(await _uploadFileService.GetUploadFile(collateral.CustomDeclarationId));
        //         if (collateral.PackingListId != null) returnFileDtos.Add(await _uploadFileService.GetUploadFile(collateral.PackingListId));
        //     }
        //     return returnFileDtos;
        // }
        public async Task<bool> ChangeStatus(Guid useId, Guid Id, string Status)
        {
            try
            {
                var collateral = await _cbeContext.Collaterals.FirstOrDefaultAsync(res => res.Id == Id);
                var caseassignment = await _cbeContext.CaseAssignments.FirstOrDefaultAsync(res => res.UserId == useId && res.CollateralId == Id);

                if (collateral != null)
                {
                    collateral.CurrentStatus = Status;
                    if (Status == "correction")
                    {
                        var correction = await _cbeContext.Corrections.Where(res => res.CollateralID == Id && res.CommentedByUserId == useId).ToListAsync();
                        if (correction.Count == 0)
                        {
                            throw new Exception("correction");
                        }
                        caseassignment.Status = "Correction";
                        collateral.CurrentStage = "Maker Officer";
                        collateral.CurrentStatus = "Correction";
                        collateral.NumberOfReturns = collateral.NumberOfReturns + 1;
                        if (collateral.Category == MechanicalCollateralCategory.MOV)
                        {
                            //this is to set the user who made it 
                            var evaluatedBy = await _cbeContext.MotorVehicles.Where(res => res.CollateralId == Id).FirstOrDefaultAsync();
                            evaluatedBy.CheckerUserID = useId;
                            _cbeContext.Update(evaluatedBy);
                            await _cbeContext.SaveChangesAsync();
                        }
                        else if (collateral.Category == MechanicalCollateralCategory.CMAMachinery)
                        {
                            //this is to set the user who made it 
                            var evaluatedBy = await _cbeContext.ConstMngAgrMachineries.Where(res => res.CollateralId == Id).FirstOrDefaultAsync();
                            evaluatedBy.CheckerUserID = useId;
                            _cbeContext.Update(evaluatedBy);
                            await _cbeContext.SaveChangesAsync();
                        }
                        else if (collateral.Category == MechanicalCollateralCategory.IBFEqupment)
                        {//this is to set the user who made it 
                            var evaluatedBy = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.CollateralId == Id).FirstOrDefaultAsync();
                            evaluatedBy.CheckerUserID = useId;
                            _cbeContext.Update(evaluatedBy);
                            await _cbeContext.SaveChangesAsync();
                        }
                    }
                    else if (Status == "Complete")
                    {
                        var correction = await _cbeContext.Corrections.Where(res => res.CollateralID == Id && res.CommentedByUserId == useId).ToListAsync();
                        if (correction.Count != 0)
                        {
                            throw new Exception("Complete");
                        }
                        if (collateral.Category == MechanicalCollateralCategory.MOV)
                        {
                            //this is to set the user who made it 
                            var evaluatedBy = await _cbeContext.MotorVehicles.Where(res => res.CollateralId == Id).FirstOrDefaultAsync();
                            evaluatedBy.CheckerUserID = useId;
                            _cbeContext.Update(evaluatedBy);
                            await _cbeContext.SaveChangesAsync();

                            var collAssginmet = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == Id).ToListAsync();
                            foreach (var assignment in collAssginmet)
                            {
                                assignment.Status = "Complete";
                                assignment.CompletionDate = DateTime.UtcNow;
                                _cbeContext.CaseAssignments.Update(assignment);

                            }
                            await _cbeContext.SaveChangesAsync();
                            collateral.CurrentStage = "Checker Officer";
                            collateral.CurrentStatus = "Complete";
                        }
                        else if (collateral.Category == MechanicalCollateralCategory.CMAMachinery)
                        {
                            //this is to set the user who made it 
                            var evaluatedBy = await _cbeContext.ConstMngAgrMachineries.Where(res => res.CollateralId == Id).FirstOrDefaultAsync();
                            evaluatedBy.CheckerUserID = useId;
                            _cbeContext.Update(evaluatedBy);
                            await _cbeContext.SaveChangesAsync();

                            var collAssginmet = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == Id).ToListAsync();
                            foreach (var assignment in collAssginmet)
                            {
                                assignment.Status = "Complete";
                                assignment.CompletionDate = DateTime.UtcNow;
                                _cbeContext.CaseAssignments.Update(assignment);

                            }
                            await _cbeContext.SaveChangesAsync();
                            collateral.CurrentStage = "Checker Officer";
                            collateral.CurrentStatus = "Complete";
                        }
                        else if (collateral.Category == MechanicalCollateralCategory.IBFEqupment)
                        {//this is to set the user who made it 
                            var evaluatedBy = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.CollateralId == Id).FirstOrDefaultAsync();
                            evaluatedBy.CheckerUserID = useId;
                            _cbeContext.Update(evaluatedBy);
                            await _cbeContext.SaveChangesAsync();

                            var collAssginmet = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == Id).ToListAsync();
                            foreach (var assignment in collAssginmet)
                            {
                                assignment.Status = "Complete";
                                assignment.CompletionDate = DateTime.UtcNow;
                                _cbeContext.CaseAssignments.Update(assignment);

                            }
                            await _cbeContext.SaveChangesAsync();
                            collateral.CurrentStage = "Checker Officer";
                            collateral.CurrentStatus = "Complete";
                        }

                    }
                    else
                    {
                        caseassignment.Status = "Pending";
                        collateral.CurrentStage = "Checker Officer";
                        collateral.CurrentStatus = "Pending";
                    }
                    _cbeContext.Update(caseassignment);
                    await _cbeContext.SaveChangesAsync();
                }
                return (true);

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<IEnumerable<ReturnCollateralDto>> MyReturnedCollaterals(Guid userId)
        {
            List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(ca => ca.UserId == userId && ca.Status == "Reject").ToListAsync();
            List<Collateral> collaterals = await _cbeContext.Collaterals.Where(ca => ca.CurrentStage == "Maker Officer" && ca.CurrentStatus == "Correction").ToListAsync();
            //List<Collateral> collaterals = new List<Collateral>();
            if (caseAssignments.Count >0)
            {
               collaterals = caseAssignments.Select(res => res.Collateral).ToList();
            }
         
            List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
            if (collaterals != null)
            {
                foreach (var caseAssignment in caseAssignments)
                {
                    var collatearal = await _cbeContext.Collaterals.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.CollateralId && ca.CurrentStatus == "Reject");
                    if (collatearal != null)
                    {
                        mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
                    }
                }
            }
            return _mapper.Map<List<ReturnCollateralDto>>(collaterals);
        }
        public async Task<IEnumerable<ReturnCollateralDto>> CorrectionCollaterals(Guid userId)
        {
            List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(ca => ca.UserId == userId && ca.Status == "Correction").ToListAsync();
            List<Collateral> collaterals = await _cbeContext.Collaterals.Where(ca => ca.CurrentStage == "Maker Officer" && ca.CurrentStatus == "Correction").ToListAsync();
            //List<Collateral> collaterals = new List<Collateral>();
            if (caseAssignments.Count > 0)
            {
                collaterals = caseAssignments.Select(res => res.Collateral).ToList();
            }

            List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
            if (collaterals != null)
            {
                foreach (var caseAssignment in caseAssignments)
                {
                    var collatearal = await _cbeContext.Collaterals.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.CollateralId && ca.CurrentStatus == "Reject");
                    if (collatearal != null)
                    {
                        mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
                    }
                }
            }
            return _mapper.Map<List<ReturnCollateralDto>>(collaterals);
        }
        public async Task<ReturnCollateralDto> MyReturnedCollateral(Guid userId, Guid id)
        {
            List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == userId && ca.CollateralId == id && ca.Status == "Correction").ToListAsync();

            List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
            if (caseAssignments != null)
            {
                foreach (var caseAssignment in caseAssignments)
                {
                    var collatearal = await _cbeContext.Collaterals.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.CollateralId && ca.CurrentStatus == "Correction");
                    if (collatearal != null)
                    {
                        mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
                    }
                }
            }
            return mTLreturnCollateralDtos[0];
        }
        public async Task<IEnumerable<ReturnCollateralDto>> MyResubmitedCollaterals(Guid userId)
        {
            List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == userId && ca.Status == "Resubmited").ToListAsync();

            List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
            if (caseAssignments != null)
            {
                foreach (var caseAssignment in caseAssignments)
                {
                    var collatearal = await _cbeContext.Collaterals.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.CollateralId && ca.CurrentStatus == "Resubmited");
                    if (collatearal != null)
                    {
                        mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
                    }
                }
            }
            return mTLreturnCollateralDtos;
        }
        public async Task<ReturnCollateralDto> MyResubmitedCollateral(Guid userId, Guid id)
        {
            List<CaseAssignment> caseAssignments = await _cbeContext.CaseAssignments.Where(ca => ca.UserId == userId && ca.CollateralId == id && ca.Status == "Resubmited").ToListAsync();

            List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
            if (caseAssignments != null)
            {
                foreach (var caseAssignment in caseAssignments)
                {
                    var collatearal = await _cbeContext.Collaterals.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.CollateralId && ca.Id == id && ca.CurrentStatus == "Resubmited");
                    if (collatearal != null)
                    {
                        mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
                    }
                }
            }
            return mTLreturnCollateralDtos[0];

        }

        public async Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid CollateralId)
        {
            var comments = await _cbeContext.Corrections.Where(ca => ca.CollateralID == CollateralId).ToListAsync();
            if (comments != null)
            {
                return _mapper.Map<IEnumerable<CorrectionRetunDto>>(comments);
            }


            return null;
        }

        public async Task<bool> DeleteCocllateral(Guid userId, Guid id)
        {
            var collateral = await _cbeContext.Collaterals.Where(c => c.Id == id && c.CreatedById == userId && c.CurrentStage == "Relation Manager").FirstOrDefaultAsync();
            if (collateral != null)
            {
                _cbeContext.Remove(collateral);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteCollateralFile(Guid userId, Guid Id)
        {
            var file = await _cbeContext.UploadFiles.FindAsync(Id);
            if (file == null)
            {
                return false;
            }
            if (file.userId != userId)
            {
                return false;
            }
            if (await _uploadFileService.DeleteFile(file.Id))
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UploadCollateralFile(Guid userId, IFormFile file, Guid caseId, string DocumentCategory)
        {
            var collateral = await _cbeContext.Collaterals.FindAsync(caseId);
            if (collateral == null)
            {
                return false;
            }

            var CollateralFile = new CreateFileDto()
            {
                File = file ?? throw new ArgumentNullException(nameof(file)),
                CollateralId = caseId,
                Category = DocumentCategory,

            };
            if (await _uploadFileService.CreateUploadFile(userId, CollateralFile) != Guid.Empty)
            {
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<CaseCorrectionHistoryRetunDto>> GetGetCollateralCorrectionHistorys(Guid caseId)
        {
            var caseComment = await _cbeContext.CommentHistorys.Include(res => res.CommentBy).Where(res => res.CollateralId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<CaseCorrectionHistoryRetunDto>>(caseComment);
        }
   
   
    }
}
