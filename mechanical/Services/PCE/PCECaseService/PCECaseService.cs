using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Models.PCE.Dto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using OpenXmlParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using OpenXmlText = DocumentFormat.OpenXml.Wordprocessing.Text;
using OpenXmlTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using OpenXmlTableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using OpenXmlTableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
  
// using iText.Kernel.Pdf;
// using iText.Layout;
// using iText.Layout.Element;
// using iText.Layout.Properties;
// using iTextDocument = iText.Layout.Document;
// using iTextParagraph = iText.Layout.Element.Paragraph;
// using iTextTable = iText.Layout.Element.Table;
// using iTextAlignment = iText.Layout.Properties.TextAlignment;

using PdfSharp.Pdf;
using PdfSharp.Drawing;

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
            var user = _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefault(res => res.Id == userId);
            var httpContext = _httpContextAccessor.HttpContext;
            var loanCase = _mapper.Map<PCECase>(pCECaseDto);
            loanCase.Id = Guid.NewGuid();
            loanCase.CurrentStage = "Relation Manager";
            loanCase.CurrentStatus = "New";
            loanCase.DistrictId = user.DistrictId;
            loanCase.RMUserId = userId;
            loanCase.CreationDate = DateTime.Now;
            await _cbeContext.PCECases.AddAsync(loanCase);
            await _cbeContext.SaveChangesAsync();
            await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
            {
                CaseId = loanCase.Id,
                Activity = $"<strong>A new case with ID {loanCase.CaseNo} has been created</strong>",
                CurrentStage = "Relation Manager"
            });
            return loanCase;

        }


       // Task<IEnumerable<PCECaseDto>> GetPCENewCases(Guid userId);
        public async Task<IEnumerable<PCENewCaseDto>> GetPCENewCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager"))
                .Where(res => res.RMUserId == userId && res.CurrentStatus == "New").ToListAsync();

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


            //var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager"))
            //   .Where(res => res.RMUserId == userId && res.CurrentStatus == "New").ToListAsync();
            //var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);

            //foreach (var caseDto in caseDtos)
            //{      
            //    var PendingNoOfCollaterals = 0;
            //    var cols = await _cbeContext.ProductionCapacities.Where(c => c.PCECaseId == caseDto.Id).ToListAsync();
            //    foreach(var col in cols)
            //    {
            //        var pendingCount =  _cbeContext.ProductionCaseAssignments
            //                .Where(pc => pc.UserId == userId && pc.ProductionCapacityId == col.Id && pc.Status == "Pending")
            //                .ToListAsync();
            //        PendingNoOfCollaterals = PendingNoOfCollaterals+ pendingCount.Count();
            //    }
            //    caseDto.TotalNoOfCollateral = _cbeContext.ProductionCapacities
            //        .Where(pc => pc.PCECaseId == caseDto.Id)
            //        .Count();

            //    caseDto.NoOfCollateral = PendingNoOfCollaterals;
            //}
            //return caseDtos;
            //PendingPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => (coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete") && coll.CurrentStage != "Relation Manager")).CountAsync(),
            //    PendingPCECollateralCount = await _cbeContext.ProductionCapacities.Where(coll => coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete" && coll.CurrentStage != "Relation Manager").CountAsync(),
            //    CompletedPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => coll.CurrentStage == "Relation Manager" && coll.CurrentStatus == "Complete")).CountAsync(),
            //    CompletedPCECollateralCount = await _cbeContext.ProductionCapacities.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "Complete").CountAsync(),
            //    TotalPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId).CountAsync(),
            //    TotalPCECollateralCount = await _cbeContext.ProductionCapacities.Where(res => res.CreatedById == userId).CountAsync(),

            //casedto

            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => ( res.CurrentStage != "Relation Manager")&&((res.CurrentStatus != "Complete" && res.CurrentStage != "Checker Officer"))))
                       .Where(res => res.RMUserId == userId && (res.ProductionCapacities.Any(collateral => ( collateral.CurrentStage != "Relation Manager") && ((collateral.CurrentStatus != "Complete" && collateral.CurrentStage != "Checker Officer")))))

                       .ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
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
                //var result = _cbeContext.PCECases.Include(res => res.District)
                //    .Include(res => res.ProductionCapacities).Include(res => res.BussinessLicence)
                //    .Where(c => c.Id == id && c.RMUserId == userId).FirstOrDefault();
                //var lastResult = _mapper.Map<PCECaseReturntDto>(result);

                //return lastResult;

                var pceCaseResult = _cbeContext.PCECases
                                   .Include(res => res.District)
                                   .Include(res => res.BussinessLicence)
                                   .Where(c => c.Id == id && c.RMUserId == userId)
                                   .FirstOrDefault();

                // Fetch the related ProductionCapacities
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
            var pceCase = await _cbeContext.PCECases.FirstOrDefaultAsync(c => c.Id == userId);

            if (pceCase != null)
            {
                pceCase.ApplicantName = caseDto.ApplicantName;
                pceCase.CustomerEmail = caseDto.CustomerEmail;
                pceCase.CustomerUserId = caseDto.CustomerUserId;

                await _cbeContext.SaveChangesAsync();

                return _mapper.Map<PCECaseReturntDto>(pceCase);
            }
            else
            {
                // If the PCECase is not found, return null
                return _mapper.Map<PCECaseReturntDto>(pceCase);
            }
        }





        public async Task<IEnumerable<PCENewCaseDto>> GetPCECompleteCases(Guid userId)
        {
            //var cases = await _cbeContext.PCECases.Where(res => res.CurrentStatus == "Completed" && res.CurrentStage == "Relation Manager").ToListAsync();
            //var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            //return caseDtos;
            var cases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer"))
           .Where(res => res.RMUserId == userId && (res.ProductionCapacities.Any(res => res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer"))).ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == caseDto.Id);
            }
            return caseDtos;
        }

        public async Task<IEnumerable<PCENewCaseDto>> GetPCERejectedCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Where(res => res.CurrentStatus == "Rejected" && res.CurrentStage == "Relation Manager").ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return caseDtos;
        }


        public async Task<IEnumerable<PCENewCaseDto>> GetPCETotalCases(Guid userId)
        {
            var cases = await _cbeContext.PCECases.Where(res => res.CurrentStage == "Relation Manager").ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(cases);
            return caseDtos;
        }

  

   
        public async Task<CreateNewCaseCountDto> GetDashboardPCECaseCount(Guid userId)
        {

            var newPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New")).CountAsync();

            // this condition is used to check if the collateral is not add then we must check the case table only 
            if (newPCECaseCount == 0)
            {
                newPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId).CountAsync();
            }
            return new CreateNewCaseCountDto()
            {
                NewPCECaseCount = newPCECaseCount,
                NewPCECollateralCount = await _cbeContext.ProductionCapacities.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "New").CountAsync(),
                PendingPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => (coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete") && coll.CurrentStage != "Relation Manager")).CountAsync(),
                PendingPCECollateralCount = await _cbeContext.ProductionCapacities.Where(coll => coll.CurrentStage != "Checker Officer" && coll.CurrentStatus != "Complete" && coll.CurrentStage != "Relation Manager").CountAsync(),
                CompletedPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId && res.ProductionCapacities.Any(coll => coll.CurrentStage == "Relation Manager" && coll.CurrentStatus == "Complete")).CountAsync(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCapacities.Where(collateral => collateral.CurrentStage == "Relation Manager" && collateral.CurrentStatus == "Complete").CountAsync(),
                TotalPCECaseCount = await _cbeContext.PCECases.Where(res => res.RMUserId == userId).CountAsync(),
                TotalPCECollateralCount = await _cbeContext.ProductionCapacities.Where(res => res.CreatedById == userId).CountAsync(),
            };

        }
        public async Task<CreateNewCaseCountDto> GetMyDashboardCaseCount()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            // var NewCollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            // var PendCollateral = await _cbeContext.CaseAssignments.Include(res => res.Collateral).Where(res => res.UserId == userId && res.Status == "Pending").ToListAsync();
            var CompCollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.Status == "Complete").ToListAsync();
     

            return new CreateNewCaseCountDto()
            {

                PCSCompletedCaseCount = CompCollateral.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == Guid.Parse(httpContext.Session.GetString("userId")) && res.Status == "Complete").CountAsync(),

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
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.Id == Id && res.CurrentStatus == "Evaluated" && res.CurrentStage == "Relational Manager").ToListAsync();            
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
            var pceCaseSchedule = await _cbeContext.ProductionCaseSchedules.Where(res => res.PCECaseId == Id && res.Status == "Approved").FirstOrDefaultAsync();
                     


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
            
            //var pceEvaluations = await _cbeContext.PCEEvaluations
            //                         .Include(e => e.ShiftHours)
            //                         .Include(e => e.TimeConsumedToCheck)
            //                         .Include(res => res.Evaluator).Where(res => res.PCEId == Id).ToListAsync();
            var pceCaseSchedule = await _cbeContext.ProductionCaseSchedules.Where(res => res.PCECaseId == Id && res.Status == "Approved").FirstOrDefaultAsync();



            //var pceEntity = await _cbeContext.PCEEvaluations
            //                         .Include(e => e.ShiftHours)
            //                         .Include(e => e.TimeConsumedToCheck)
            //                         .Include(e => e.PCE)
            //                         .ThenInclude(e => e.PCECase)
            //                         // .Include(pe => pe.UploadFiles)
            //                         .FirstOrDefaultAsync(e => e.Id == Id);


            return new PCEReportDataDto
            {
                PCESCase = pceCase,
                Productions = productions,
                PCEEvaluations = pceEvaluations,
                PCECaseSchedule = pceCaseSchedule
            };
        }






        public async Task<byte[]> GenerateDOCX(PCEReportDataDto pceReportData)
        {
            var pceCase = pceReportData.PCESCase;
            var productions = pceReportData.Productions;
            var pceEvaluations = pceReportData.PCEEvaluations;
            var pceCaseSchedule = pceReportData.PCECaseSchedule;

            string commaSeparatedRegions = "";
            string commaSeparatedPropertyOf = "";
            string commaSeparatedType = "";

            if (productions != null)
            {
                var uniqueRegions = productions.Select(m => m.Region).Distinct();
                commaSeparatedRegions = string.Join(", ", uniqueRegions);
                var propertyOf = productions.Select(m => m.PropertyOwner).Distinct();
                commaSeparatedPropertyOf = string.Join(", ", propertyOf);
                var type = productions.Select(m => m.Type).Distinct();
                commaSeparatedType = string.Join(", ", type);
            }

            var makers = pceEvaluations.Select(m => m.Evaluator).Distinct();
            var pceCaseScheduleDate = pceCaseSchedule?.ScheduleDate ?? DateTime.Now;

            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
                {
                    var mainPart = wordDocument.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document(new Body());
                    }

                    var body = mainPart.Document.Body;

                    body.Append(CreateParagraph("Commercial Bank of Ethiopia", 20, true));
                    body.Append(CreateParagraph($"PCE Case Ref. No.: {pceCase.CaseNo}", 14, true));
                    body.Append(CreateParagraph($"Date: {pceCaseScheduleDate:MMMM dd, yyyy}", 12, false));

                    body.Append(CreateParagraph("PART I: EXECUTIVE SUMMARY", 14, true));
                    body.Append(CreateParagraph($"This report, requested by {pceCase.ApplicantName}, documents the visit to the production/plant capacity estimation located in the following regions: {commaSeparatedRegions} on {pceCaseScheduleDate:MMMM dd, yyyy} for the purpose of valuing the motor vehicle present.", 12, false));

                    body.Append(CreateParagraph("General Description", 14, true));
                    body.Append(CreateParagraph($"Property of: ({commaSeparatedPropertyOf})", 12, false));
                    body.Append(CreateParagraph($"Machinery and Equipment Location: ({commaSeparatedRegions})", 12, false));
                    body.Append(CreateParagraph($"Type of Production: ({commaSeparatedType})", 12, false));
                    body.Append(CreateParagraph($"Date of Valuation: {pceCaseScheduleDate:MMMM dd, yyyy}", 12, false));

                    body.Append(CreateParagraph("Additional Information", 14, true));
                    body.Append(CreateParagraph($"District: {pceCase.District?.Name}", 12, false));
                    body.Append(CreateParagraph($"Creator: {pceCase.RMUser?.Name}", 12, false));

                    if (pceCase.BussinessLicence != null)
                    {
                        body.Append(CreateParagraph($"Business License: [Link to License]", 12, false));
                    }
                    else
                    {
                        body.Append(CreateParagraph("Business License: Not Available", 12, false));
                    }

                    body.Append(CreateParagraph("PART II: ASSESSMENT AND REMARKS", 14, true));
                    body.Append(CreateParagraph("Our survey assessment and remarks are as follows:", 12, false));

                    if (pceEvaluations != null && pceEvaluations.Any())
                    {
                        foreach (var item in pceEvaluations)
                        {
                            if (!string.IsNullOrEmpty(item.SurveyRemark))
                            {
                                body.Append(CreateParagraph(item.SurveyRemark, 12, false));
                            }
                        }
                    }
                    else
                    {
                        body.Append(CreateParagraph("No survey remarks available.", 12, false));
                    }

                    body.Append(CreateParagraph("Maker’s Name:", 14, true));
                    body.Append(CreateParagraph("Signature:", 14, true));
                    body.Append(CreateParagraph("Date:", 14, true));

                    if (makers != null && makers.Any())
                    {
                        foreach (var item in makers)
                        {
                            body.Append(CreateParagraph(item.Name, 12, false));
                            body.Append(CreateParagraph("Signature Placeholder", 12, false));
                            body.Append(CreateParagraph(pceCaseScheduleDate.ToString("MMMM dd, yyyy"), 12, false));
                        }
                    }
                    else
                    {
                        body.Append(CreateParagraph("No makers available.", 12, false));
                    }

                    body.Append(CreateParagraph("PART III: COST SUMMARY", 14, true));
                    body.Append(CreateParagraph("RC, Total Replacement Cost (new):", 12, false));
                    body.Append(CreateParagraph("Total Estimation/Market Value:", 12, false));
                    body.Append(CreateParagraph($"Based on the above, we certify that the total present market value of the motor vehicle as of {pceCaseScheduleDate:MMMM dd, yyyy} is [Placeholder for value] Birr", 12, false));

                    mainPart.Document.Save();
                }
                return memoryStream.ToArray();
            }
        }

        private OpenXmlParagraph CreateParagraph(string text, int fontSize, bool isBold)
        {
            var runProperties = new RunProperties();
            runProperties.Append(new DocumentFormat.OpenXml.Wordprocessing.FontSize { Val = fontSize.ToString() });
            if (isBold)
            {
                runProperties.Append(new Bold());
            }

            var run = new Run();
            run.Append(runProperties);
            run.Append(new Text(text));

            var paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new Justification { Val = JustificationValues.Left });

            var paragraph = new OpenXmlParagraph();
            paragraph.Append(paragraphProperties);
            paragraph.Append(run);

            return paragraph;
        }

        public async Task<byte[]> GeneratePDF(PCEReportDataDto pceReportData)
        {
            var pceCase = pceReportData.PCESCase;
            var productions = pceReportData.Productions;
            var pceEvaluations = pceReportData.PCEEvaluations;
            var pceCaseSchedule = pceReportData.PCECaseSchedule;

            string commaSeparatedRegions = "";
            string commaSeparatedPropertyOf = "";
            string commaSeparatedType = "";

            if (productions != null)
            {
                var uniqueRegions = productions.Select(m => m.Region).Distinct();
                commaSeparatedRegions = string.Join(", ", uniqueRegions);
                var propertyOf = productions.Select(m => m.PropertyOwner).Distinct();
                commaSeparatedPropertyOf = string.Join(", ", propertyOf);
                var type = productions.Select(m => m.Type).Distinct();
                commaSeparatedType = string.Join(", ", type);
            }

            var makers = pceEvaluations.Select(m => m.Evaluator).Distinct();
            var pceCaseScheduleDate = pceCaseSchedule?.ScheduleDate ?? DateTime.Now;

            using (var memoryStream = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                var titleFont = new XFont("Verdana", 20); //, XFontStyle.Bold);
                var headerFont = new XFont("Verdana", 14); //, XFontStyle.Bold);
                var bodyFont = new XFont("Verdana", 12);

                gfx.DrawString("Commercial Bank of Ethiopia", titleFont, XBrushes.Black, new XRect(0, 40, page.Width, 40), XStringFormats.Center);
                gfx.DrawString($"PCE Case Ref. No.: {pceCase.CaseNo}", headerFont, XBrushes.Black, new XRect(40, 80, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Date: {pceCaseScheduleDate:MMMM dd, yyyy}", bodyFont, XBrushes.Black, new XRect(40, 100, page.Width - 80, 20), XStringFormats.TopLeft);

                gfx.DrawString("PART I: EXECUTIVE SUMMARY", headerFont, XBrushes.Black, new XRect(40, 140, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"This report, requested by {pceCase.ApplicantName}, documents the visit to the production/plant capacity estimation located in the following regions: {commaSeparatedRegions} on {pceCaseScheduleDate:MMMM dd, yyyy} for the purpose of valuing the motor vehicle present.", bodyFont, XBrushes.Black, new XRect(40, 160, page.Width - 80, 60), XStringFormats.TopLeft);

                gfx.DrawString("General Description", headerFont, XBrushes.Black, new XRect(40, 230, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Property of: ({commaSeparatedPropertyOf})", bodyFont, XBrushes.Black, new XRect(40, 250, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Machinery and Equipment Location: ({commaSeparatedRegions})", bodyFont, XBrushes.Black, new XRect(40, 270, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Type of Production: ({commaSeparatedType})", bodyFont, XBrushes.Black, new XRect(40, 290, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Date of Valuation: {pceCaseScheduleDate:MMMM dd, yyyy}", bodyFont, XBrushes.Black, new XRect(40, 310, page.Width - 80, 20), XStringFormats.TopLeft);

                gfx.DrawString("Additional Information", headerFont, XBrushes.Black, new XRect(40, 340, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"District: {pceCase.District?.Name}", bodyFont, XBrushes.Black, new XRect(40, 360, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Creator: {pceCase.RMUser?.Name}", bodyFont, XBrushes.Black, new XRect(40, 380, page.Width - 80, 20), XStringFormats.TopLeft);

                if (pceCase.BussinessLicence != null)
                {
                    gfx.DrawString($"Business License: [Link to License]", bodyFont, XBrushes.Black, new XRect(40, 400, page.Width - 80, 20), XStringFormats.TopLeft);
                }
                else
                {
                    gfx.DrawString("Business License: Not Available", bodyFont, XBrushes.Black, new XRect(40, 400, page.Width - 80, 20), XStringFormats.TopLeft);
                }

                gfx.DrawString("PART II: ASSESSMENT AND REMARKS", headerFont, XBrushes.Black, new XRect(40, 420, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("Our survey assessment and remarks are as follows:", bodyFont, XBrushes.Black, new XRect(40, 440, page.Width - 80, 20), XStringFormats.TopLeft);

                if (pceEvaluations != null && pceEvaluations.Any())
                {
                    int yOffset = 460;
                    foreach (var item in pceEvaluations)
                    {
                        if (!string.IsNullOrEmpty(item.SurveyRemark))
                        {
                            gfx.DrawString(item.SurveyRemark, bodyFont, XBrushes.Black, new XRect(40, yOffset, page.Width - 80, 20), XStringFormats.TopLeft);
                            yOffset += 20;
                        }
                    }
                }
                else
                {
                    gfx.DrawString("No survey remarks available.", bodyFont, XBrushes.Black, new XRect(40, 460, page.Width - 80, 20), XStringFormats.TopLeft);
                }

                gfx.DrawString("Maker’s Name:", headerFont, XBrushes.Black, new XRect(40, 500, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("Signature:", headerFont, XBrushes.Black, new XRect(page.Width / 2, 500, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("Date:", headerFont, XBrushes.Black, new XRect(page.Width - 80, 500, page.Width - 80, 20), XStringFormats.TopLeft);

                if (makers != null && makers.Any())
                {
                    int yOffset = 520;
                    foreach (var item in makers)
                    {
                        gfx.DrawString(item.Name, bodyFont, XBrushes.Black, new XRect(40, yOffset, page.Width / 2 - 40, 20), XStringFormats.TopLeft);
                        gfx.DrawString("Signature Placeholder", bodyFont, XBrushes.Black, new XRect(page.Width / 2, yOffset, page.Width / 2 - 40, 20), XStringFormats.TopLeft);
                        gfx.DrawString(pceCaseScheduleDate.ToString("MMMM dd, yyyy"), bodyFont, XBrushes.Black, new XRect(page.Width - 80, yOffset, 80, 20), XStringFormats.TopRight);
                        yOffset += 20;
                    }
                }
                else
                {
                    gfx.DrawString("No makers available.", bodyFont, XBrushes.Black, new XRect(40, 520, page.Width - 80, 20), XStringFormats.TopLeft);
                }

                gfx.DrawString("PART III: COST SUMMARY", headerFont, XBrushes.Black, new XRect(40, 540, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("RC, Total Replacement Cost (new):", bodyFont, XBrushes.Black, new XRect(40, 560, page.Width - 80, 20), XStringFormats.TopLeft);

                gfx.DrawString("Total Estimation/Market Value:", bodyFont, XBrushes.Black, new XRect(40, 580, page.Width - 80, 20), XStringFormats.TopLeft);

                gfx.DrawString($"Based on the above, we certify that the total present market value of the motor vehicle as of {pceCaseScheduleDate:MMMM dd, yyyy} is [Placeholder for value] Birr", bodyFont, XBrushes.Black, new XRect(40, 600, page.Width - 80, 20), XStringFormats.TopLeft);

                gfx.DrawString("Maker’s Name:", headerFont, XBrushes.Black, new XRect(40, 620, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("Signature:", headerFont, XBrushes.Black, new XRect(page.Width / 2, 620, page.Width - 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("Date:", headerFont, XBrushes.Black, new XRect(page.Width - 80, 620, page.Width - 80, 20), XStringFormats.TopLeft);
                if (makers != null && makers.Any())
                {
                    int yOffset = 640;
                    foreach (var item in makers)
                    {
                        gfx.DrawString(item.Name, bodyFont, XBrushes.Black, new XRect(40, yOffset, page.Width / 2 - 40, 20), XStringFormats.TopLeft);
                        gfx.DrawString("Signature Placeholder", bodyFont, XBrushes.Black, new XRect(page.Width / 2, yOffset, page.Width / 2 - 40, 20), XStringFormats.TopLeft);
                        gfx.DrawString(pceCaseScheduleDate.ToString("MMMM dd, yyyy"), bodyFont, XBrushes.Black, new XRect(page.Width - 80, yOffset, 80, 20), XStringFormats.TopRight);
                        yOffset += 20;
                    }
                }
                else
                {
                    gfx.DrawString("No makers available.", bodyFont, XBrushes.Black, new XRect(40, 640, page.Width - 80, 20), XStringFormats.TopLeft);
                }

                document.Save(memoryStream, false);
                return memoryStream.ToArray();
            }
        }
        
        // public async Task<byte[]> GeneratePDF(PCEReportDataDto pceReportData)
        // {
        //     try
        //     {
        //         var pceCase = pceReportData.PCESCase;
        //         var productions = pceReportData.Productions;
        //         var pceEvaluations = pceReportData.PCEEvaluations;
        //         var pceCaseSchedule = pceReportData.PCECaseSchedule;

        //         using (var memoryStream = new MemoryStream())
        //         {
        //             using (var writer = new PdfWriter(memoryStream))
        //             {
        //                 using (var pdfDocument = new PdfDocument(writer))
        //                 {
        //                     var document = new iTextDocument(pdfDocument);

        //                     // Title and Header
        //                     document.Add(new iTextParagraph($"PCECase Ref. No.: {pceCase.CaseNo}")
        //                         .SetFontSize(16)
        //                         .SetBold()
        //                         .SetTextAlignment(iTextAlignment.CENTER));

        //                     // General Info Table
        //                     var table = new iTextTable(UnitValue.CreatePercentArray(new float[] { 1, 2 }));
        //                     // var table = new iTextTable(UnitValue.CreatePercentArray(1));
        //                     table.SetWidth(UnitValue.CreatePercentValue(100));
                            
        //                     table.AddHeaderCell("Field");
        //                     table.AddHeaderCell("Value");

        //                     table.AddCell("Property of:");
        //                     table.AddCell(string.Join(", ", productions.Select(p => p.PropertyOwner).Distinct()));
                            
        //                     table.AddCell("Machinery and Equipment Location:");
        //                     table.AddCell(string.Join(", ", productions.Select(p => p.Region).Distinct()));
                            
        //                     table.AddCell("Type of Production:");
        //                     table.AddCell(string.Join(", ", productions.Select(p => p.Type).Distinct()));
                            
        //                     table.AddCell("Date of Valuation:");
        //                     table.AddCell(pceCaseSchedule?.ScheduleDate.ToString("d MMM yyyy") ?? "N/A");
                            
        //                     table.AddCell("Applicant Name:");
        //                     table.AddCell(pceCase.ApplicantName);

        //                     document.Add(table);

        //                     // Evaluation Remarks
        //                     if (pceEvaluations.Any())
        //                     {
        //                         document.Add(new iTextParagraph("Survey Assessment and Remarks:")
        //                             .SetFontSize(14)
        //                             .SetBold()
        //                             .SetTextAlignment(iTextAlignment.TopLeft)
        //                             .SetMarginTop(10));

        //                         foreach (var evaluation in pceEvaluations)
        //                         {
        //                             if (!string.IsNullOrWhiteSpace(evaluation.SurveyRemark))
        //                             {
        //                                 document.Add(new iTextParagraph(evaluation.SurveyRemark)
        //                                     .SetMarginBottom(5));
        //                             }
        //                         }
        //                     }

        //                     // Footer
        //                     document.Add(new iTextParagraph("Report generated on: " + DateTime.Now.ToString("d MMM yyyy"))
        //                         .SetFontSize(10)
        //                         .SetTextAlignment(iTextAlignment.CENTER)
        //                         .SetMarginTop(20));
        //                 }
        //             }
        //             return memoryStream.ToArray();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log the exception
        //         // Example: _logger.LogError(ex, "Error generating PDF report.");
        //         throw;
        //     }
        // }
    }
}
