using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.InternalReport;
using mechanical.Models.PCE.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;


namespace mechanical.Services.InternalReportService
{
    public class InternalReportService: IInternalReportService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IUploadFileService _uploadFileService;
        public InternalReportService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
            _uploadFileService = uploadFileService;
        }
        public async Task<(IEnumerable<ValuationReportDto> DistinctCases, IEnumerable<ValuationReportDto> AllProductionCapacities)> GetInternalPCECaseReport(Guid userId)
        {
            // Step 1: Fetch all relevant PCECaseAssignments for the user, eager loading all primary related entities.
            var caseAssignments = await _cbeContext.PCECaseAssignments
                .Include(ca => ca.ProductionCapacity)
                    .ThenInclude(pc => pc.PCECase)
                        .ThenInclude(pce => pce.PCECaseOriginator)
                .Include(ca => ca.ProductionCapacity)
                    .ThenInclude(pc => pc.PCECase)
                        .ThenInclude(pce => pce.District) 
                .Include(ca => ca.User) 
                .Where(ca => ca.UserId == userId)
                .ToListAsync();

            // If no assignments, return empty results early
            if (!caseAssignments.Any())
            {
                return (DistinctCases: Enumerable.Empty<ValuationReportDto>(), AllProductionCapacities: Enumerable.Empty<ValuationReportDto>());
            }

            // Collect all unique ProductionCapacity IDs and PCECase IDs from the fetched assignments
            var productionCapacityIds = caseAssignments.Select(ca => ca.ProductionCapacityId).ToHashSet(); // Use HashSet for O(1) lookups
            var pceCaseIds = caseAssignments.Select(ca => ca.ProductionCapacity.PCECaseId).Distinct().ToHashSet();

            // Step 2: Fetch all PCEEvaluations, PCECaseSchedules, ProductionReestimations, and ALL relevant PCECaseAssignments
            // for the collected IDs in a few separate, targeted queries.
            // Use ToListAsync() for immediate execution.

            var evaluations = await _cbeContext.PCEEvaluations
                .Include(e => e.Justifications)
                .Include(e => e.TimeConsumedToCheck)
                .Include(e => e.Evaluator)
                .Where(e => productionCapacityIds.Contains(e.PCEId))
                .ToListAsync();

            var pceCaseSchedules = await _cbeContext.PCECaseSchedules
                .Where(s => pceCaseIds.Contains(s.PCECaseId) && s.Status == "Approved") // Filter approved schedules here
                .ToListAsync();

            var reestimations = await _cbeContext.ProductionReestimations
                .Where(r => productionCapacityIds.Contains(r.ProductionCapacityId))
                .ToListAsync();

            // Consolidate all role-based PCECaseAssignments into a single query
            // This is a major optimization from 8 separate queries.
            var allRoleAssignments = await _cbeContext.PCECaseAssignments
                .Include(ca => ca.User)
                    .ThenInclude(u => u.Role)
                .Where(ca => productionCapacityIds.Contains(ca.ProductionCapacityId) &&
                             (ca.User.Role.Name == "Maker Manager" ||
                              ca.User.Role.Name == "Maker TeamLeader" ||
                              ca.User.Role.Name == "Maker Officer" ||
                              ca.User.Role.Name == "Checker Manager" ||
                              ca.User.Role.Name == "Checker TeamLeader" ||
                              ca.User.Role.Name == "Checker Officer" ||
                              ca.User.Role.Name == "District Valuation Manager"))
                .ToListAsync();

            // Step 3: Organize fetched data into efficient lookup dictionaries
            // This replaces repeated .Where().FirstOrDefault() or .Where().ToList() calls in loops.

            var evaluationsByPCEId = evaluations.GroupBy(e => e.PCEId).ToDictionary(g => g.Key, g => g.ToList());
            var reestimationsByPCId = reestimations.GroupBy(r => r.ProductionCapacityId).ToDictionary(g => g.Key, g => g.ToList());
            var pceCaseSchedulesByPCECaseId = pceCaseSchedules.GroupBy(s => s.PCECaseId).ToDictionary(g => g.Key, g => g.ToList());

            // Group assignments by ProductionCapacityId and then by Role Name for quick lookup
            var assignmentsLookup = allRoleAssignments
                .GroupBy(ca => ca.ProductionCapacityId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(
                        ca => ca.User.Role.Name,
                        ca => ca // Store the assignment itself, not just the user
                    )
                );

            // Helper to get assignment by PCId and RoleName
            PCECaseAssignment GetAssignment(Guid pcId, string roleName)
            {
                return assignmentsLookup.TryGetValue(pcId, out var pcAssignments) && pcAssignments.TryGetValue(roleName, out var assignment)
                    ? assignment
                    : null;
            }

            // Step 4: Populate AllProductionCapacityDtos (one DTO per ProductionCapacity)
            var allProductionCapacityDtos = new List<ValuationReportDto>();

            foreach (var caseAssignment in caseAssignments)
            {
                var pc = caseAssignment.ProductionCapacity;
                var pceCase = pc.PCECase;

                // Get related data efficiently using dictionary lookups
                evaluationsByPCEId.TryGetValue(pc.Id, out var pcEvaluations);
                pcEvaluations = pcEvaluations ?? new List<PCEEvaluation>(); // Ensure it's not null

                reestimationsByPCId.TryGetValue(pc.Id, out var pcReestimations);
                pcReestimations = pcReestimations ?? new List<ProductionReestimation>();

                pceCaseSchedulesByPCECaseId.TryGetValue(pceCase.Id, out var caseSpecificSchedules);
                var scheduleDate = caseSpecificSchedules?.OrderByDescending(s => s.CreatedAt).FirstOrDefault();

                var firstEvaluation = pcEvaluations.FirstOrDefault();
                var justifications = firstEvaluation?.Justifications?.ToList() ?? new List<Justification>();
                var timeConsumed = firstEvaluation?.TimeConsumedToCheck;

                // Find assignments for specific roles using the helper
                var makerManagerAssignment = GetAssignment(pc.Id, "Maker Manager");
                var makerTeamLeaderAssignment = GetAssignment(pc.Id, "Maker TeamLeader");
                var makerOfficerAssignment = GetAssignment(pc.Id, "Maker Officer");
                var checkerManagerAssignment = GetAssignment(pc.Id, "Checker Manager");
                var checkerTeamLeaderAssignment = GetAssignment(pc.Id, "Checker TeamLeader");
                var checkerOfficerAssignment = GetAssignment(pc.Id, "Checker Officer");
                var districtEvaluationManagerAssignment = GetAssignment(pc.Id, "District Valuation Manager");


                // DTO Population Logic (remains largely similar, but data access is faster)
                var dto = new ValuationReportDto
                {

                    Id= pc.Id,
                    CaseNo = pceCase.CaseNo,
                    CurrentStatus = pceCase.Status,
                    CreatedAt = pc.CreatedAt, // Set to PCECase CreatedAt
                    ApplicantName = pceCase.ApplicantName,
                    CasePriority = "N/A", // This seems constant, consider if it should be dynamic
                    CaseFRQ = pcEvaluations.Count.ToString(),
                    RequestedOrgan = pceCase.Segment ?? "N/A",
                    CustomerApplicantRelationship = pceCase.PCECaseOriginator?.Name ?? "RM Name",

                    DateCaseDeliveredToValuationOffice = makerManagerAssignment?.AssignmentDate,
                    DateCaseAssignedToTeamLeader = makerTeamLeaderAssignment?.AssignmentDate,
                    DateCaseAssignedToValuators = makerOfficerAssignment?.AssignmentDate,
                    PurposeOfValuationRequest = pc.Purpose,
                    LastRecentValuationDate = pcEvaluations.Any() ? pcEvaluations.Max(e => e.CreatedAt) : null,
                    DurationReceiptGrossDays = pceCase.Status == "Completed" && pcEvaluations.Any() && pcEvaluations.First().CompletedAt != null
                                                ? (int?)GetDateDifference(pceCase.CreatedAt, pcEvaluations.First().CompletedAt.Value).TotalDays
                                                : pceCase.CreatedAt != default ? (int?)GetDateDifference(pceCase.CreatedAt, DateTime.Now).TotalDays : null,
                    DistrictName = pceCase.District?.Name ?? "Head Office",
                    DurationAssignedGrossDays = makerOfficerAssignment?.AssignmentDate != null && pceCase.District?.Name == "Head Office" && makerTeamLeaderAssignment?.AssignmentDate != null
                                                    ? (int?)Math.Round(GetDateDifference(makerTeamLeaderAssignment.AssignmentDate, makerOfficerAssignment.AssignmentDate).TotalDays)
                                                    : makerOfficerAssignment?.AssignmentDate != null && makerManagerAssignment?.AssignmentDate != null
                                                        ? (int?)Math.Round(GetDateDifference(makerManagerAssignment.AssignmentDate, makerOfficerAssignment.AssignmentDate).TotalDays)
                                                        : null,
                    DurationAssignedToTMGrossDays = makerManagerAssignment?.AssignmentDate != null && makerTeamLeaderAssignment?.AssignmentDate != null
                                                    ? (int?)Math.Round(GetDateDifference(makerManagerAssignment.AssignmentDate, makerTeamLeaderAssignment.AssignmentDate).TotalDays)
                                                    : null,
                    RequestedEngineer = "Mechanical", // Constant
                    ProcessingWCDays = null, // Constant
                    QuantityForSimilarMechanicalItem = -1, // Constant or placeholder, review this logic
                    NameOfValuator = firstEvaluation?.Evaluator?.Name ?? "N/A",
                    AssignedNo = -1, // Review this logic, seems constant or placeholder
                    DeliveredNo = -1, // Review this logic, seems constant or placeholder
                    ReturnedWithAdvice = -1, // Review this logic, seems constant or placeholder
                    OnHandNo = -1, // Review this logic, seems constant or placeholder
                    DeliveredPercentage = -1, // Review this logic, seems constant or placeholder
                    NetDaysConsumed = scheduleDate != null && firstEvaluation != null && firstEvaluation.CompletedAt.HasValue
                                        ? (int?)Math.Round(GetDateDifference(scheduleDate.ScheduleDate, firstEvaluation.CompletedAt.Value).TotalDays)
                                        : firstEvaluation != null && firstEvaluation.CompletedAt.HasValue
                                            ? (int?)Math.Round(GetDateDifference(firstEvaluation.CompletedAt.Value, DateTime.Now).TotalDays)
                                            : null,
                    SDTAccomplishment = "N/A", // Constant
                    FulfillmentOfDocumentation = "Complete", // Constant
                    ScheduledVisitDate = scheduleDate?.ScheduleDate,
                    Location = firstEvaluation?.InspectionPlace ?? $"{pc.Region}, {pc.City}, {pc.SubCity}, {pc.Wereda}, {pc.Kebele}",
                    QuantityComplexityOfProperty = "N/A", // Constant
                    LHCTitleDeedSerialNo = pc.LHCNumber,
                    TypeOfProperty = pc.Type,
                    PropertyCategory = pc.Category.ToString(),
                    SiteInspectionDate = firstEvaluation != null ? firstEvaluation.InspectionDate.ToDateTime(TimeOnly.MinValue) : null,
                    //after this all are the attribute filled by the checker officer and working in the collateral not in the pce
                    DateSentForChecking = null,
                    TotalNumberOfComments = null,
                    DateCommentReceivedFromChecking = null,
                    DateReportSentToRequestingOrgan = null, //this is the date co sent to rm
                    DateReturnedWithAdviceToRO = null, // this is the date filled when the case return mo to rm
                    GrossDaysConsumedChecker = null,   // this is the time checker officer uses to check the pce, .. timeConsumed != null ? (int?)timeConsumed.Duration.TotalDays : null,
                    GrossDaysConsumed = pceCase.CompletedAt != null ? (int?)GetDateDifference(pceCase.CreatedAt, pceCase.CompletedAt.Value).TotalDays : (int?)GetDateDifference(pceCase.CreatedAt, DateTime.Now).TotalDays,
                    ReasonOfReturn = null // the reason the case is returned from the mo to rm
                    };
                allProductionCapacityDtos.Add(dto);
            }

            // Step 5: Populate DistinctCaseDtos (one DTO per unique PCECase)
            var distinctCaseDtos = new List<ValuationReportDto>();
            var uniqueCases = caseAssignments
                                .Select(ca => ca.ProductionCapacity.PCECase)
                                .DistinctBy(pce => pce.Id)
                                .ToList(); // Keep this as it derives unique cases from the initial fetch

            foreach (var pceCase in uniqueCases)
            {
                // Get all ProductionCapacities for this PCECase from the initially fetched data
                var productionCapacitiesForCase = caseAssignments
                    .Where(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id)
                    .Select(ca => ca.ProductionCapacity)
                    .ToList();

                var pcIdsForCase = productionCapacitiesForCase.Select(pc => pc.Id).ToHashSet();

                // Aggregate evaluations and reestimations for this PCECase
                var caseEvaluations = evaluations.Where(e => pcIdsForCase.Contains(e.PCEId)).ToList();
                var caseReestimations = reestimations.Where(r => pcIdsForCase.Contains(r.ProductionCapacityId)).ToList();
                var allJustifications = caseEvaluations.SelectMany(e => e.Justifications ?? new List<Justification>()).ToList();
                var allTimeConsumed = caseEvaluations.Select(e => e.TimeConsumedToCheck).Where(t => t != null).ToList();

                var firstProductionCapacity = productionCapacitiesForCase.FirstOrDefault();
                var firstEvaluation = caseEvaluations.FirstOrDefault();
                var scheduleDate = pceCaseSchedulesByPCECaseId.TryGetValue(pceCase.Id, out var schedulesForCase)
                    ? schedulesForCase.OrderByDescending(s => s.CreatedAt).FirstOrDefault()
                    : null;

                // Find assignments for specific roles (earliest date across all ProductionCapacities for this PCECase)
                // Group by role and get the min assignment date
                var roleAssignmentsForCase = allRoleAssignments
                    .Where(ca => pcIdsForCase.Contains(ca.ProductionCapacityId))
                    .GroupBy(ca => ca.User.Role.Name)
                    .ToDictionary(g => g.Key, g => g.OrderBy(ca => ca.AssignmentDate).FirstOrDefault());

                PCECaseAssignment GetEarliestAssignmentForCase(string roleName)
                {
                    return roleAssignmentsForCase.TryGetValue(roleName, out var assignment) ? assignment : null;
                }

                var makerManagerAssignment = GetEarliestAssignmentForCase("Maker Manager");
                var makerTeamLeaderAssignment = GetEarliestAssignmentForCase("Maker TeamLeader");
                var makerOfficerAssignment = GetEarliestAssignmentForCase("Maker Officer");
                var checkerManagerAssignment = GetEarliestAssignmentForCase("Checker Manager");
                var checkerTeamLeaderAssignment = GetEarliestAssignmentForCase("Checker TeamLeader");
                var checkerOfficerAssignment = GetEarliestAssignmentForCase("Checker Officer");
                var districtEvaluationManagerAssignment = GetEarliestAssignmentForCase("District Valuation Manager");

                var dto = new ValuationReportDto
                {

                    Id = pceCase.Id,
                    CurrentStatus = pceCase.Status,

                    CaseNo = pceCase.CaseNo,
                    CreatedAt = pceCase.CreatedAt,
                    ApplicantName = pceCase.ApplicantName,
                    CasePriority = "N/A",
                    CaseFRQ = caseEvaluations.Count.ToString(),
                    RequestedOrgan = pceCase.PCECaseOriginator.Department ?? "RM",
                    CustomerApplicantRelationship = pceCase.PCECaseOriginator?.Name ?? "RM Name",
                    DateCaseDeliveredToValuationOffice = makerManagerAssignment?.AssignmentDate,
                    DateCaseAssignedToTeamLeader = makerTeamLeaderAssignment?.AssignmentDate,
                    DateCaseAssignedToValuators = makerOfficerAssignment?.AssignmentDate,
                    PurposeOfValuationRequest = firstProductionCapacity?.Purpose ?? "N/A",
                    LastRecentValuationDate = caseEvaluations.Any() ? caseEvaluations.Max(e => e.CreatedAt) : null,
                    DurationReceiptGrossDays = pceCase.Status == "Completed" && caseEvaluations.Any() && caseEvaluations.FirstOrDefault()?.CompletedAt != null
                                                ? (int?)GetDateDifference(pceCase.CreatedAt, caseEvaluations.First().CompletedAt.Value).TotalDays
                                                : pceCase.CreatedAt != default ? (int?)GetDateDifference(pceCase.CreatedAt, DateTime.Now).TotalDays : null,
                    DistrictName = pceCase.District?.Name ?? "Head Office",
                    DurationAssignedGrossDays = makerOfficerAssignment?.AssignmentDate != null && pceCase.District?.Name == "Head Office" && makerTeamLeaderAssignment?.AssignmentDate != null
                                                    ? (int?)Math.Round(GetDateDifference(makerTeamLeaderAssignment.AssignmentDate, makerOfficerAssignment.AssignmentDate).TotalDays)
                                                    : makerOfficerAssignment?.AssignmentDate != null && makerManagerAssignment?.AssignmentDate != null
                                                        ? (int?)Math.Round(GetDateDifference(makerManagerAssignment.AssignmentDate, makerOfficerAssignment.AssignmentDate).TotalDays)
                                                        : null,
                    DurationAssignedToTMGrossDays = makerManagerAssignment?.AssignmentDate != null && makerTeamLeaderAssignment?.AssignmentDate != null
                                                    ? (int?)Math.Round(GetDateDifference(makerManagerAssignment.AssignmentDate, makerTeamLeaderAssignment.AssignmentDate).TotalDays)
                                                    : null,
                    RequestedEngineer = "Mechanical",
                    ProcessingWCDays = null,
                    QuantityForSimilarMechanicalItem = productionCapacitiesForCase.Count,
                    NameOfValuator = firstEvaluation?.Evaluator?.Name ?? "N/A",
                    AssignedNo = caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id),
                    DeliveredNo = caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id && ca.Status == "Completed"),
                    ReturnedWithAdvice = caseReestimations.Count,
                    OnHandNo = caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id && ca.Status != "Completed"),
                    DeliveredPercentage = caseAssignments.Any(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id)
                                            ? (double)caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id && ca.Status == "Completed") / caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id) * 100
                                            : 0,
                    NetDaysConsumed = scheduleDate != null && firstEvaluation != null && firstEvaluation.CompletedAt.HasValue
                                        ? (int?)Math.Round(GetDateDifference(scheduleDate.ScheduleDate, firstEvaluation.CompletedAt.Value).TotalDays)
                                        : firstEvaluation != null && firstEvaluation.CompletedAt.HasValue
                                            ? (int?)Math.Round(GetDateDifference(firstEvaluation.CompletedAt.Value, DateTime.Now).TotalDays)
                                            : null,
                    SDTAccomplishment = "N/A",
                    FulfillmentOfDocumentation = pceCase.BusinessLicenseId != null ? "Complete" : "Incomplete",
                    ScheduledVisitDate = scheduleDate?.ScheduleDate,
                    Location = firstEvaluation?.InspectionPlace ?? (firstProductionCapacity != null ? $"{firstProductionCapacity.Region}, {firstProductionCapacity.City}, {firstProductionCapacity.SubCity}, {firstProductionCapacity.Wereda}, {firstProductionCapacity.Kebele}" : "N/A"),
                    QuantityComplexityOfProperty = "N/A",
                    LHCTitleDeedSerialNo = firstProductionCapacity?.LHCNumber ?? "N/A",
                    TypeOfProperty = firstProductionCapacity?.Type ?? "N/A",
                    PropertyCategory = firstProductionCapacity?.Category.ToString() ?? "N/A",
                    SiteInspectionDate = firstEvaluation != null ? firstEvaluation.InspectionDate.ToDateTime(TimeOnly.MinValue) : null,


                    DateSentForChecking = null,
                    TotalNumberOfComments = null,
                    DateCommentReceivedFromChecking = null,
                    DateReportSentToRequestingOrgan = null, //this is the date co sent to rm
                    DateReturnedWithAdviceToRO = null, // this is the date filled when the case return mo to rm
                    GrossDaysConsumedChecker = null,   // this is the time checker officer uses to check the pce, .. timeConsumed != null ? (int?)timeConsumed.Duration.TotalDays : null,
                    GrossDaysConsumed = null,//
                    ReasonOfReturn = null // the reason the case is returned from the mo to rm

                };
                distinctCaseDtos.Add(dto);
            }

            return (DistinctCases: distinctCaseDtos, AllProductionCapacities: allProductionCapacityDtos);
        }


        public TimeSpan GetDateDifference(DateTime date1, DateTime date2)
        {
            return date1 - date2;
        }

        public async Task<IEnumerable<InternalCaseReportDto>> GetInternalCaseReport(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral)
                                    .ThenInclude(res => res.Case)
                                    .ThenInclude(res => res.CaseOriginator)
                                    .Where(Ca => Ca.UserId == userId)
                                    .ToListAsync();

            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case) 
                                    .DistinctBy(c => c.Id).ToList();

            var caseDtos = _mapper.Map<IEnumerable<InternalCaseReportDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }    
    }
}
