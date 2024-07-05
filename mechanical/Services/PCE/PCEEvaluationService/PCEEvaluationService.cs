using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using Microsoft.CodeAnalysis.Operations;
using OpenCvSharp.CPlusPlus;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using mechanical.Data;
using mechanical.Models.Dto.Correction;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.FileUploadDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;

using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.PCE.Enum.File;
using mechanical.Services.MailService;
using mechanical.Services.PCE.FileUploadService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.PCECaseTimeLineService;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public class PCEEvaluationService : IPCEEvaluationService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCEEvaluationService> _logger;
        private readonly IFileUploadService _fileUploadService;
        private readonly IPCECaseTimeLineService _PCECaseTimeLineService;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PCEEvaluationService(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IMailService mailService, CbeContext context, IMapper mapper, IPCECaseTimeLineService PCECaseTimeLineService, ILogger<PCEEvaluationService> logger, IFileUploadService fileUploadService)
        {
            _cbeContext = context;
            _mapper = mapper;
            _logger = logger;
            _fileUploadService = fileUploadService;
            _PCECaseTimeLineService = PCECaseTimeLineService;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<PCEEvaluationPostDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceentity = _mapper.Map<PCEEvaluation>(Dto);
                pceentity.Id = Guid.NewGuid();
                pceentity.EvaluatorID = UserId;             
                // pceentity.CheckerID = UserId;              
                // pceentity.PCEId = Dto.PCEId;
                pceentity.CreatedAt = DateTime.Now;
                pceentity.Status = Status.New;
                pceentity.RejectionReason = null;   

                await _cbeContext.PCEEvaluations.AddAsync(pceentity);
                await _cbeContext.SaveChangesAsync();       
             
                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);

                // pceentity.InvoiceValue = PCEEvaluation.InvoiceValue * PCEEvaluation.ExchangeRate;

                // pceentity.MarketShareFactor = await _motorVehicleAnnexService.GetCAMIBFMarketShareFactor(PCEEvaluation.TechnologyStandard);
                // pceentity.DepreciationRate = await _motorVehicleAnnexService.GetIBMDepreciationRate(DateTime.Now.Year - PCEEvaluation.YearOfManufacture, PCEEvaluation.IndustrialBuildingMachineryType);
                // pceentity.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(PCEEvaluation.CurrentEqpmntCondition, PCEEvaluation.AllocatedPointsRange);
                // pceentity.ReplacementCost = PCEEvaluation.InvoiceValue;
                // pceentity.NetEstimationValue = PCEEvaluation.MarketShareFactor * PCEEvaluation.DepreciationRate * PCEEvaluation.EqpmntConditionFactor * PCEEvaluation.ReplacementCost;
         

                await _fileUploadService.CreateFiles(UserId, pceentity.Id, Dto.SupportingEvidences, Category.SupportingEvidence);
                await _fileUploadService.CreateFiles(UserId, pceentity.Id, Dto.ProductionProcessFlowDiagrams, Category.ProductionProcessFlowDiagram);
                await _cbeContext.SaveChangesAsync();

                // var PCECase = await _cbeContext.PCECases.FindAsync(pceentity.PCEId);
                // PCECase.CurrentStatus = "Pending";
                // _cbeContext.Update(PCE);
                // await _cbeContext.SaveChangesAsync();

                // var PCECaseAssignment = await _cbeContext.PCECaseAssignments.FirstOrDefaultAsync(a => a.PCEId == PCE.Id && a.UserId == Userid);
                // PCECaseAssignment.Status = "Pending";
                // _cbeContext.Update(PCECaseAssignment);
                // await _cbeContext.SaveChangesAsync();

                // await _PCECaseTimeLineService.CreatePCECaseTimeLine(new PCECaseTimeLinePostDto
                // {
                //     PCEId = PCE.PCEId,
                //     Activity = $" <strong class=\"text-sucess\">PCE maker Evaluation has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {PCE.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {PCE.Role}.&nbsp; <i class='text-purple'>PCE Catagory:</i> {EnumHelper.GetEnumDisplayName(PCE.Category)}. &nbsp; <i class='text-purple'>PCE Type:</i> {EnumHelper.GetEnumDisplayName(PCE.Type)}.",
                //     CurrentStage = "Maker Manager"
                // });

                return _mapper.Map<PCEEvaluationPostDto>(pceentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PCEEvaluation");
                throw new ApplicationException("An error occurred while creating the PCEEvaluation.");
            }
        }
        

        public async Task<PCEEvaluationPostDto> EditPCEEvaluation(Guid UserId, Guid id, PCEEvaluationPostDto Dto)
        {
            try
            {

                var pceentity = await _cbeContext.PCEEvaluations.FindAsync(id);
                if (pceentity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                var PCE = await _cbeContext.PCECases.FindAsync(pceentity.PCEId);
                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);

                _mapper.Map(Dto, pceentity);
                _cbeContext.Update(pceentity);
                await _cbeContext.SaveChangesAsync();

                await _fileUploadService.CreateFiles(UserId, pceentity.Id, Dto.SupportingEvidences, Category.SupportingEvidence);
                await _fileUploadService.CreateFiles(UserId, pceentity.Id, Dto.ProductionProcessFlowDiagrams, Category.ProductionProcessFlowDiagram);
                await _cbeContext.SaveChangesAsync();
                          
                // pceentity.InvoiceValue = pceentity.InvoiceValue * pceentity.ExchangeRate;
                // pceentity.MarketShareFactor = await _motorVehicleAnnexService.GetCAMIBFMarketShareFactor(pceentity.TechnologyStandard);
                // pceentity.DepreciationRate = await _motorVehicleAnnexService.GetIBMDepreciationRate(DateTime.Now.Year - pceentity.YearOfManufacture, pceentity.IndustrialBuildingMachineryType);
                // pceentity.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(pceentity.CurrentEqpmntCondition, pceentity.AllocatedPointsRange);
                // pceentity.ReplacementCost = pceentity.InvoiceValue;
                // pceentity.NetEstimationValue = pceentity.MarketShareFactor * pceentity.DepreciationRate * pceentity.EqpmntConditionFactor * pceentity.ReplacementCost;

                // await _PCECaseTimeLineService.CreatePCECaseTimeLine(new PCECaseTimeLinePostDto
                // {
                //     PCEId = PCE.PCEId,
                //     Activity = $" <strong class=\"text-sucess\">PCE maker valuation Correction has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {PCE.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {PCE.Role}.&nbsp; <i class='text-purple'>PCE Catagory:</i> {EnumHelper.GetEnumDisplayName(PCE.Category)}. &nbsp; <i class='text-purple'>PCE Type:</i> {EnumHelper.GetEnumDisplayName(PCE.Type)}.",
                //     CurrentStage = "Maker Manager"
                // });

                return _mapper.Map<PCEEvaluationPostDto>(pceentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing PCEEvaluation");
                throw new ApplicationException("An error occurred while editing the PCEEvaluation.");
            }
        }


        public async Task<PCEEvaluationReturnDto> CheckPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            var pceentity = _mapper.Map<PCEEvaluation>(Dto);
            var PCE = await _cbeContext.PCECases.FindAsync(pceentity.PCEId);
            // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);

            // pceentity.InvoiceValue = pceentity.InvoiceValue * pceentity.ExchangeRate;
            // pceentity.DepreciationRate = await _motorVehicleAnnexService.GetIBMDepreciationRate(DateTime.Now.Year - pceentity.YearOfManufacture, pceentity.IndustrialBuildingMachineryType);
            // pceentity.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(pceentity.CurrentEqpmntCondition, pceentity.AllocatedPointsRange);
            // pceentity.ReplacementCost = pceentity.InvoiceValue;
            // pceentity.NetEstimationValue = pceentity.MarketShareFactor * pceentity.DepreciationRate * pceentity.EqpmntConditionFactor * pceentity.ReplacementCost;

            return _mapper.Map<PCEEvaluationReturnDto>(pceentity);
        }
        
        public async Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid id)
        {
            try
            {
                var pceentity = await _cbeContext.PCEEvaluations
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .FirstOrDefaultAsync(e => e.Id == id);
                if (pceentity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }
                return _mapper.Map<PCEEvaluationReturnDto>(pceentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> GetPCEEvaluationByPCEId(Guid UserId, Guid PCEId)
        {
            try
            {
                var pceentity = await _cbeContext.PCEEvaluations
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .Include(res => res.PCE)
                    .FirstOrDefaultAsync(res => res.PCEId == PCEId);
                if (pceentity == null)
                {
                    _logger.LogWarning("PCEEvaluation with PCE Id {PCEId} not found", PCEId);
                    throw new KeyNotFoundException("PCEEvaluation with PCEID {PCEID} not found");
                }
                return _mapper.Map<PCEEvaluationReturnDto>(pceentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation with PCEID {PCEID}");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation with PCEID {PCEID}.");
            }
        }
        public async Task<PCEEvaluationReturnDto> GetEvaluatedPCEEvaluation(Guid UserId, Guid PCEId)
        {
            // PCECaseCommenAttributeDto PCECaseCommenAttributeDto = new PCECaseCommenAttributeDto();
            // ReturnEvaluatedPCECaseDto returnEvaluatedPCECaseDto = new ReturnEvaluatedPCECaseDto();
        
            try
            {
                var pceentity = await _cbeContext.PCEEvaluations
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .Include(res => res.PCE)
                    .FirstOrDefaultAsync(res => res.PCEId == PCEId);
                if (pceentity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", PCEId);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }
                return _mapper.Map<PCEEvaluationReturnDto>(pceentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation.");
            }
        }
        public async Task<PCEEvaluationPostDto> GetReturnedPCEEvaluation(Guid UserId, Guid PCEId)
        { 
            try
            {
                var pceentity = await _cbeContext.PCEEvaluations
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .Include(res => res.PCE)
                    .FirstOrDefaultAsync(res => res.PCEId == PCEId);
                if (pceentity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", PCEId);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }
                return _mapper.Map<PCEEvaluationPostDto>(pceentity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation.");
            }
        }

        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetNewPCEEvaluations(Guid UserId)
        {
            try
            {
                var pceentities = await _cbeContext.PCEEvaluations.Where(e => e.Status == Status.New).ToListAsync();
                return _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceentities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new PCEEvaluations");
                throw new ApplicationException("An error occurred while fetching new PCEEvaluations.");
            }
        }

        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetRejectedPCEEvaluations(Guid UserId)
        {
            try
            {
                var pceentities = await _cbeContext.PCEEvaluations.Where(e => e.Status == Status.Rejected).ToListAsync();
                return _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceentities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected PCEEvaluations");
                throw new ApplicationException("An error occurred while fetching rejected PCEEvaluations.");
            }
        }

        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetTerminatedPCEEvaluations(Guid UserId)
        {
            try
            {
                var pceentities = await _cbeContext.PCEEvaluations.Where(e => e.Status == Status.Terminated).ToListAsync();
                return _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceentities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching terminated PCEEvaluations");
                throw new ApplicationException("An error occurred while fetching terminated PCEEvaluations.");
            }
        }

        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetPendingPCEEvaluations(Guid UserId)
        {
            try
            {
                var pceentities = await _cbeContext.PCEEvaluations.Where(e => e.Status == Status.Pending).ToListAsync();
                return _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceentities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending PCEEvaluations");
                throw new ApplicationException("An error occurred while fetching pending PCEEvaluations.");
            }
        }

        public async Task SendForApproval(Guid UserId, IEnumerable<Guid> SelectedPCEIds, Guid CenterId)
        
        {
            try
            {
                var PCEIds = SelectedPCEIds; //.Split(',').Select(id => Guid.Parse(id)).ToList();
                var PCEEvaluations = await _cbeContext.PCEEvaluations.Where(e => PCEIds.Contains(e.Id)).ToListAsync();
                foreach (var PCEEvaluation in PCEEvaluations)
                {
                    PCEEvaluation.Status = Status.Approved;
                }
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PCEEvaluations for approval");
                throw new ApplicationException("An error occurred while sending PCEEvaluations for approval.");
            }
        }

        public async Task RejectPCEEvaluation(Guid UserId, Guid Id, string rejectionReason)
        {
            try
            {
                var PCEEvaluation = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (PCEEvaluation == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                PCEEvaluation.Status = Status.Rejected;
                PCEEvaluation.RejectionReason = rejectionReason;
                _cbeContext.PCEEvaluations.Update(PCEEvaluation);
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting PCEEvaluation");
                throw new ApplicationException("An error occurred while rejecting the PCEEvaluation.");
            }
        }

       
        public async Task<int> GetDashboardPCEEvaluationCount(Guid UserId)
        {
            try
            {
                return await _cbeContext.PCEEvaluations.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard PCEEvaluation count");
                throw new ApplicationException("An error occurred while fetching the dashboard PCEEvaluation count.");
            }
        }

        public async Task<int> GetMyDashboardPCEEvaluationCount(Guid UserId)
        {
            try
            {
                return await _cbeContext.PCEEvaluations.CountAsync(e => e.CreatedBy == UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my dashboard PCEEvaluation count");
                throw new ApplicationException("An error occurred while fetching your dashboard PCEEvaluation count.");
            }
        }

        public async Task<bool> DeleteSupportingEvidence(Guid UserId, Guid Id)
        {
            try
            {
                var evidence = await _cbeContext.UploadFiles.FindAsync(Id);
                if (evidence == null) return false;

                _cbeContext.UploadFiles.Remove(evidence);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supporting evidence");
                throw new ApplicationException("An error occurred while deleting the supporting evidence.");
            }
        }

        public async Task<bool> DeleteProcessFlowDiagram(Guid UserId, Guid Id)
        {
            try
            {
                var diagram = await _cbeContext.UploadFiles.FindAsync(Id);
                if (diagram == null) return false;

                _cbeContext.UploadFiles.Remove(diagram);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting process flow diagram");
                throw new ApplicationException("An error occurred while deleting the process flow diagram.");
            }
        }

        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetAllPCEEvaluations(Guid UserId)
        {
            try
            {
                var pceentities = await _cbeContext.PCEEvaluations
                    .Include(p => p.ShiftHours)
                    .Include(p => p.TimeConsumedToCheck)
                    .Include(p => p.SupportingDocuments)
                    // .Include(e => e.SupportingEvidences)
                    // .Include(e => e.ProductionProcessFlowDiagrams)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceentities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all PCEEvaluations");
                throw new ApplicationException("An error occurred while fetching all PCEEvaluations.");
            }
        }

        public async Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id)
        {
            try
            {
                var pceentity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceentity == null)
                {
                    return false;
                }

                _cbeContext.PCEEvaluations.Remove(pceentity);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PCEEvaluation");
                throw new ApplicationException("An error occurred while deleting the PCEEvaluation.");
            }
        }
     
     
        [HttpPost]
        public async Task RemarkRelease(Guid UserId, Guid Id, Guid PCEId, String Remark, Guid EvaluatorID)
        {
            var entity = await _cbeContext.PCEEvaluations.FindAsync(Id);
            entity.Remark = Remark;
            _cbeContext.Update(entity);
            _cbeContext.SaveChanges();

            // var PCE = await _cbeContext.PCEs.FindAsync(PCEId);
            // PCE.CurrentStage = "Checker Officer";
            // PCE.CurrentStatus = "Complete";
            // _cbeContext.Update(PCE);
            // _cbeContext.SaveChanges();

            // var PCECaseAssignment = await _cbeContext.PCECaseAssignments.Where(res => res.UserId == EvaluatorID && res.PCEId == PCEId).FirstOrDefaultAsync();
            // PCECaseAssignment.Status = "Complete";
            // _cbeContext.Update(PCECaseAssignment);
            // _cbeContext.SaveChanges();
            // await _mailService.SendEmail(new MailPostDto
            // {
            //     SenderEmail = " getnetadane1@cbe.com.et",
            //     SenderPassword = "Gechlove@1234",
            //     RecipantEmail = "yohannessintayhu@cbe.com.et",
            //     Subject = "Remark Release Update ",
            //     Body = "Dear! </br> Remark release Update  For Applicant:-" + PCE.PropertyOwner + "</br></br> For further Detail please check PCE Valuation System",
            // });
        }

        public async Task<string> GetPCEEvaluationSummary(Guid UserId, Guid PCEId)
        {
            var PCEEvaluations = await _cbeContext.PCEEvaluations.Where(res => res.PCEId == PCEId).ToListAsync();
            // var PCEEvaluations = await _cbeContext.PCEEvaluations.Where(res => res.PCE.PCEId == PCEId).ToListAsync();
            return JsonConvert.SerializeObject(PCEEvaluations);
        }


        public async Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid UserId, Guid Id)
        {
            var comments = await _cbeContext.Corrections.Where(ca => ca.Id == Id).ToListAsync();
            if (comments != null)
            {
              return _mapper.Map<IEnumerable<CorrectionRetunDto>>(comments);
             }
                
            return null;
        }


        public async Task<bool> SendToRM(Guid UserId, Guid PCEId)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // var PCECase = await _cbeContext.PCECases.FindAsync(PCEId);
        
            // PCECase.CurrentStage = "Regional Manager";
            // PCECase.CurrentStatus = "New";
            // _cbeContext.PCECases.Update(PCECase);
            // await _cbeContext.SaveChangesAsync();

            // var cases = await _cbeContext.PCECases.Include(res => res.District).FirstOrDefaultAsync(res => res.Id == PCECase.CaseId);
            // var user = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.Id == UserId);

            // CreateUser checker;

            // if(user.District.Name == "Head Office")
            // {
            //     checker = await _cbeContext.CreateUsers.FirstOrDefaultAsync(res => res.District.Name == "Head Office" && res.Role.Name == "Checker Manager");
            //     var caseAssignment = new CaseAssignment()
            //     {
            //         PCEId = PCEId,
            //         UserId = checker.Id,
            //         Status = "New",
            //         AssignmentDate = DateTime.Now
            //     };
            //     await _cbeContext.CaseAssignments.AddAsync(caseAssignment);
            // }
            // else
            // {
            //      checker = await _cbeContext.CreateUsers.FirstOrDefaultAsync(res => res.DistrictId == user.DistrictId && res.Role.Name == "District Valuation Manager");
            //     var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.PCEId == PCEId && res.UserId == checker.Id).FirstOrDefaultAsync();
            //     caseAssignment.Status = "Checker New";
            //      _cbeContext.CaseAssignments.Update(caseAssignment);
            // }
           
            // _cbeContext.PCECases.Update(PCECase);
            // await _cbeContext.SaveChangesAsync();
            // await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            // {
            //     CaseId = PCECase.CaseId,
            //     Activity = $"<strong>Case send for Checkeing to Checkr Unit.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {cases.District.Name}.",
            //     CurrentStage = "Maker Manager"
            // });
            return true;
        }
    
     
        // public async Task<IActionResult> SomethingPCEEvaluation(Guid Id, PCEEvaluationPostDto Dto)
        // {
        //     var pceentity = await _PCEEvaluationService.EditPCEEvaluation(Id, PCEEvaluation);

        //     var PCEAssesment = await _cbeContext.PCEEvaluation.FirstOrDefaultAsync(res => res.Id == Id);
        //     var PCE = await _cbeContext.PCEs.FindAsync(PCEEvaluation.PCEId);
        //     Guid? checkerID = Guid.Empty;
        //     if (PCEAssesment.CheckerUserID == null)
        //     {
        //         var correction = await _cbeContext.Corrections.FirstOrDefaultAsync(res => res.PCEID == pceentity.PCEId);

        //         checkerID = correction.CommentedByUserId;
        //     }
        //     else
        //     {
        //         checkerID = PCEAssesment.CheckerUserID;
        //     }
            
        //     var Userid = base.GetCurrentUserId();
        //     var PCECaseAssignmentChange = await _cbeContext.PCECaseAssignments.Where(res => res.UserId == Userid && res.PCEId == PCEEvaluation.PCEId).FirstOrDefaultAsync();
        //     PCECaseAssignmentChange.Status = "Pending";
        //     var PCECaseAssignment = await _cbeContext.PCECaseAssignments.Where(res => res.PCEId == PCEEvaluation.PCEId && res.UserId == checkerID).FirstOrDefaultAsync();
            
        //     if (PCE.CurrentStatus.Contains("Remark"))
        //     {
        //         PCE.CurrentStatus = "Remark Verfication";
        //         PCECaseAssignment.Status = "Remark Verfication";
        //     }
        //     else
        //     {
        //         PCE.CurrentStatus = "New";
        //         PCECaseAssignment.Status = "New";
        //     }

        //     PCECaseAssignment.AssignmentDate = DateTime.Now;
        //     _cbeContext.Update(PCECaseAssignment);
        //     _cbeContext.Update(PCECaseAssignmentChange);

        //     PCE.CurrentStage = "Checker Officer";

        //     _cbeContext.Update(PCE);
        //     await _cbeContext.SaveChangesAsync();
            
        //     return RedirectToAction("MyReturnedPCEs", "MoPCECase");
            
        // }

        
    }
}