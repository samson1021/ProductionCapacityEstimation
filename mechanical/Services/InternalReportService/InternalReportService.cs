﻿using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.InternalReport;
using mechanical.Models.Entities;
using mechanical.Models.Enum;
using mechanical.Models.PCE.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;


namespace mechanical.Services.InternalReportService
{
    public class InternalReportService : IInternalReportService
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
        public async Task<(IEnumerable<ValuationReportDto> DistinctCases, IEnumerable<ValuationReportDto> AllProductionCapacities)> GetInternalCaseReport(Guid userId)
        {
            string userRole = _httpContextAccessor.HttpContext.Session.GetString("userRole");
            if (userRole == "")
            {
                return (DistinctCases: Enumerable.Empty<ValuationReportDto>(), AllProductionCapacities: Enumerable.Empty<ValuationReportDto>());

            }
          
            var caseAssignmentss = _cbeContext.CaseAssignments
                        .Include(ca => ca.Collateral)
                            .ThenInclude(pc => pc.Case)
                                .ThenInclude(c => c.CaseOriginator)
                        .Include(ca => ca.Collateral)
                            .ThenInclude(pc => pc.Case)
                                .ThenInclude(pce => pce.District)
                        .Include(ca => ca.User)
                            .ThenInclude(u => u.Role)
                        as IQueryable<CaseAssignment>;

            if (userRole != "Higher Official")
            {
                caseAssignmentss = caseAssignmentss.Where(ca => ca.UserId == userId);
            }

            var caseAssignments = await caseAssignmentss.ToListAsync();



            if (!caseAssignments.Any())
            {
                return (DistinctCases: Enumerable.Empty<ValuationReportDto>(), AllProductionCapacities: Enumerable.Empty<ValuationReportDto>());
            }
            var CollateralIds = caseAssignments.Select(ca => ca.CollateralId).ToHashSet(); // Use HashSet for O(1) lookups
            var CaseIds = caseAssignments.Select(ca => ca.Collateral.CaseId).Distinct().ToHashSet();
            var CaseSchedules = await _cbeContext.CaseSchedules
                .Where(s => CaseIds.Contains(s.CaseId) && s.Status == "Approved") // Filter approved schedules here
                .ToListAsync();

            var CaseSchedulesByCaseId = CaseSchedules.GroupBy(s => s.CaseId).ToDictionary(g => g.Key, g => g.ToList());
            var constructionEvaluations = await _cbeContext.ConstMngAgrMachineries
                                            .Include(e => e.CheckerUser)
                                            .Include(e => e.EvaluatorUser)
                                            .Where(e => CollateralIds.Contains(e.CollateralId))
                                            .ToListAsync();
            var industrialEvaluations = await _cbeContext.IndBldgFacilityEquipment
                                            .Include(e => e.CheckerUser)
                                            .Include(e => e.EvaluatorUser)
                                            .Where(e => CollateralIds.Contains(e.CollateralId))
                                            .ToListAsync();
            var vehicleEvaluations = await _cbeContext.MotorVehicles
                                            .Include(e => e.CheckerUser)
                                            .Include(e => e.EvaluatorUser)
                                            .Where(e => CollateralIds.Contains(e.CollateralId))
                                            .ToListAsync();
            var caseSchedules = await _cbeContext.CaseSchedules
                .Where(s => CaseIds.Contains(s.CaseId)) // Filter approved schedules here
                .ToListAsync();



            // Get all users with the relevant roles in one query
            //var roleNames = new List<string>
            //        {
            //            "Maker Manager", "Maker TeamLeader", "Maker Officer",
            //            "Checker Manager", "Checker TeamLeader", "Checker Officer",
            //            "District Valuation Manager"
            //        };

            //var usersWithRoles = await _cbeContext.Users
            //    .Include(u => u.Role)
            //    .Where(u => roleNames.Contains(u.Role.Name))
            //    .ToDictionaryAsync(u => u.Id, u => u.Role.Name);

       
     
            var allCollateralDtos = new List<ValuationReportDto>();

      
            foreach (var caseAssignment in caseAssignments)
            {
                var c = caseAssignment.Collateral;
                var Case = c.Case;
                var CollateralStatus = caseAssignment.Status;
                var CollateralCompletionDate = caseAssignment.CompletionDate;
                CaseSchedulesByCaseId.TryGetValue(Case.Id, out var caseSpecificSchedules);
                var scheduleDate = caseSpecificSchedules?
                                        .OrderByDescending(s => s.CreatedAt)
                                        .FirstOrDefault()?.ScheduleDate ?? DateTime.Now; // Or another default value
                IEnumerable<dynamic> currentCollateralCategoryEvaluations = Enumerable.Empty<dynamic>();
                switch (c.Category)
                {
                    case MechanicalCollateralCategory.CMAMachinery:
                        currentCollateralCategoryEvaluations = constructionEvaluations?.Cast<dynamic>() ?? Enumerable.Empty<dynamic>();
                        break;
                    case MechanicalCollateralCategory.IBFEqupment:
                        currentCollateralCategoryEvaluations = industrialEvaluations?.Cast<dynamic>() ?? Enumerable.Empty<dynamic>();
                        break;
                    case MechanicalCollateralCategory.MOV:
                        currentCollateralCategoryEvaluations = vehicleEvaluations?.Cast<dynamic>() ?? Enumerable.Empty<dynamic>();
                        break;
                }

                var collateralEvaluations = currentCollateralCategoryEvaluations
                    .Where(e => e.CollateralId == c.Id)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToList();
                var firstEvaluation = collateralEvaluations.FirstOrDefault();
                DateTime? lastEvaluationDate = null;
                DateTime? FirstEvaluationDate = null;

                if (firstEvaluation != null)
                {
                    lastEvaluationDate = firstEvaluation.LastUpdatedAt;
                    FirstEvaluationDate = firstEvaluation.CreatedAt;
                }

                var reevaluationCount = collateralEvaluations.Count > 0 ? collateralEvaluations.Count - 1 : 0;

                var justifications = "";
                var timeConsumed = "";


                var makerManagerAssignment = await _cbeContext.CaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.CollateralId == c.Id &&
                                 ca.User.Role.Name == "Maker Manager")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                var makerTeamLeaderAssignment = await _cbeContext.CaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.CollateralId == c.Id &&
                                 ca.User.Role.Name == "Maker TeamLeader")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                var makerOfficerAssignment = await _cbeContext.CaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.CollateralId == c.Id &&
                                 ca.User.Role.Name == "Maker Officer")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                var checkerManagerAssignment = await _cbeContext.CaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.CollateralId == c.Id &&
                                 ca.User.Role.Name == "Checker Manager")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                var checkerTeamLeaderAssignment = await _cbeContext.CaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.CollateralId == c.Id &&
                                 ca.User.Role.Name == "Checker TeamLeader")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                var checkerOfficerAssignment = await _cbeContext.CaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.CollateralId == c.Id &&
                                 ca.User.Role.Name == "Checker Officer")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                var districtEvaluationManagerAssignment = await _cbeContext.CaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.CollateralId == c.Id &&
                                 ca.User.Role.Name == "District Valuation Manager")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();



                var dto = new ValuationReportDto
                {
                    Id = c.Id,
                    CaseNo = Case.CaseNo,
                    CurrentStatus = CollateralStatus,
                    CreatedAt = c.CreationDate,
                    ApplicantName = Case.ApplicantName,
                    CasePriority = " ",
                    CaseFRQ = c.NumberOfReturns.ToString(),
                    RequestedOrgan = Case.Segment ?? " ",
                    CustomerApplicantRelationship = Case.CaseOriginator?.Name ?? "",
                    PurposeOfValuationRequest = c.Purpose,
                    DistrictName = Case.District?.Name ?? "Head Office",
                    RequestedEngineer = c.CollateralType ?? "Mechanical",
                    ProcessingWCDays = null,
                    QuantityForSimilarMechanicalItem = "",
                    AssignedNo = -1,
                    DeliveredNo = -1,
                    ReturnedWithAdvice = -1,
                    OnHandNo = -1,
                    DeliveredPercentage = -1,
                    SDTAccomplishment = null,
                    FulfillmentOfDocumentation = "Complete",
                    ScheduledVisitDate = scheduleDate,
                    //Location =  $"{c.Region}, {c.City}, {c.SubCity}, {c.Wereda}, {c.Kebele}",
                    Location = string.Join(", ", new[] { c.Region, c.City, c.SubCity, c.Wereda, c.Kebele }
                                .Where(s => !string.IsNullOrEmpty(s))),
                    QuantityComplexityOfProperty = " ",

                    LHCTitleDeedSerialNo = c.SerialNo ?? c.InvoiceNo ?? c.EngineMotorNo ?? c.PlateNo ?? c.ChassisNo ?? null,
                    TypeOfProperty = c.Type.ToString(),
                    PropertyCategory = c.Category.ToString(),

                    SiteInspectionDate = null, // it need some modification
                    DateReturnedWithAdviceToRO = null, // this is the date filled when the case return mo to rm
                    ReasonOfReturn = null, // the reason the case is returned from the mo to rm

                    //maker section
                    NameOfValuator = firstEvaluation?.EvaluatorUser?.Name ?? makerOfficerAssignment?.User?.Name ?? "",
                    DateCaseDeliveredToValuationOffice = makerManagerAssignment?.AssignmentDate,
                    DateCaseAssignedToTeamLeader = makerTeamLeaderAssignment?.AssignmentDate,
                    DateCaseAssignedToValuators = makerOfficerAssignment?.AssignmentDate,

                    LastRecentValuationDate = lastEvaluationDate ?? FirstEvaluationDate, // need modification after jon update the evauation date.
                    /// needs modification 

                    NetDaysConsumed = scheduleDate != null && firstEvaluation != null && CollateralStatus == "Complete"
                                       ? GetDateDifference(CollateralCompletionDate, scheduleDate)
                                       : firstEvaluation != null && CollateralStatus != "Complete"
                                           ? scheduleDate != null ? GetDateDifference(DateTime.Now, scheduleDate)
                                           : GetDateDifference(DateTime.Now, makerOfficerAssignment.AssignmentDate) : null,
                    DurationReceiptGrossDays = CollateralStatus == "Complete" ? GetDateDifference(CollateralCompletionDate, makerManagerAssignment.AssignmentDate)
                                                    : GetDateDifference(DateTime.Now, makerManagerAssignment.AssignmentDate),

                    DurationAssignedToTMGrossDays = Case.District?.Name == "Head Office" &&
                                                        makerManagerAssignment?.AssignmentDate != null &&
                                                        makerTeamLeaderAssignment?.AssignmentDate != null
                                                            ? GetDateDifference(
                                                                makerTeamLeaderAssignment.AssignmentDate,
                                                                makerManagerAssignment.AssignmentDate)
                                                            : null,
                    DurationAssignedGrossDays = makerOfficerAssignment?.AssignmentDate != null && Case.District?.Name == "Head Office" && makerTeamLeaderAssignment?.AssignmentDate != null
                                                        ? GetDateDifference(makerOfficerAssignment.AssignmentDate, makerTeamLeaderAssignment.AssignmentDate)
                                                        : makerOfficerAssignment?.AssignmentDate != null && makerManagerAssignment?.AssignmentDate != null
                                                            ? GetDateDifference(makerOfficerAssignment.AssignmentDate, makerManagerAssignment.AssignmentDate)
                                                            : null,
                    // start checking
                    NameOfChecker = firstEvaluation?.CheckerUser?.Name ?? checkerOfficerAssignment?.User?.Name ?? "",
                    DateSentForChecking = checkerManagerAssignment?.AssignmentDate,
                    DateCaseAssignedToCheckerTeamLeader = Case.District?.Name == "Head Office" ? checkerTeamLeaderAssignment?.AssignmentDate : null,
                    DateCaseAssignedToCheckerValuators = checkerOfficerAssignment?.AssignmentDate,
                    DateReportSentToRequestingOrgan = CollateralStatus == "Complete" ? CollateralCompletionDate : null, //this is the date co sent to

                    GrossDaysConsumedChecker = checkerOfficerAssignment?.AssignmentDate != null
                                                ? (CollateralStatus == "Complete" && CollateralCompletionDate != null
                                                    ? GetDateDifference(CollateralCompletionDate, checkerOfficerAssignment.AssignmentDate)
                                                    : GetDateDifference(DateTime.Now, checkerOfficerAssignment.AssignmentDate))
                                                : " ",
                    DurationCheckerReceiptGrossDays = checkerManagerAssignment?.AssignmentDate != null
                                                        ? (CollateralStatus == "Complete" && CollateralCompletionDate != null
                                                            ? GetDateDifference(CollateralCompletionDate, checkerManagerAssignment.AssignmentDate)
                                                            : GetDateDifference(DateTime.Now, checkerManagerAssignment.AssignmentDate))
                                                        : " ",

                    DurationCheckerAssignedToTMGrossDays = Case.District?.Name == "Head Office" &&
                                                              checkerManagerAssignment?.AssignmentDate != null &&
                                                              checkerTeamLeaderAssignment?.AssignmentDate != null
                                                                ? GetDateDifference(checkerTeamLeaderAssignment.AssignmentDate, checkerManagerAssignment.AssignmentDate)
                                                                : " ",
                    DurationCheckerAssignedGrossDays = checkerOfficerAssignment?.AssignmentDate != null && Case.District?.Name == "Head Office" && checkerTeamLeaderAssignment?.AssignmentDate != null
                                                        ? GetDateDifference(checkerOfficerAssignment.AssignmentDate, checkerTeamLeaderAssignment.AssignmentDate)
                                                        : checkerOfficerAssignment?.AssignmentDate != null && checkerManagerAssignment?.AssignmentDate != null
                                                            ? GetDateDifference(checkerOfficerAssignment.AssignmentDate, checkerManagerAssignment.AssignmentDate)
                                                            : " ",


                    TotalNumberOfComments = null,
                    DateCommentReceivedFromChecking = null,
                };

                allCollateralDtos.Add(dto);
            }

            var distinctCaseDtos = new List<ValuationReportDto>();


            var caseAssignmentsList = caseAssignments.ToList(); // Execute query first

            var uniqueCases = caseAssignmentsList
                .Where(ca => ca.Collateral?.Case != null) // Safe navigation
                .Select(ca => ca.Collateral!.Case!)       // Now in-memory, safe to use !
                .DistinctBy(pce => pce.Id)                // Works on IEnumerable
                .ToList();
            var mostRecentCollateralAssignments = caseAssignmentsList
             .GroupBy(ca => ca.CollateralId)
             .Select(g => g.OrderByDescending(ca => ca.AssignmentDate) // Order by AssignmentDate to get the latest
                            .FirstOrDefault())
             .Where(ca => ca != null) // Filter out any nulls if a group was empty (unlikely with GroupBy)
             .ToList();


            foreach (var uniqueCase in uniqueCases)
            {
                var totalcollateralForCase = mostRecentCollateralAssignments
                                             .Where(ca => ca.Collateral.CaseId == uniqueCase.Id)
                                             .ToList();

                // 1. First select all collaterals for this case
                var caseCollaterals = await _cbeContext.Collaterals
                    .Where(c => c.CaseId == uniqueCase.Id)
                    .Select(c => new { c.Category, c.Type, c.ManufactureYear })
                    .ToListAsync();

                // 2. Then filter and group similar collaterals
                var similarCollateralGroups = caseCollaterals
                    .GroupBy(c => new
                    {
                        Category = c.Category.ToString(),
                        Type = c.Type,
                        ManufactureYear = c.ManufactureYear
                    })
                    .Where(g => g.Count() > 1) // Only show groups with duplicates
                    .OrderByDescending(g => g.Count())
                    .ToList();

                // 3. Format the similar items description
                var similarItemsDescription = similarCollateralGroups.Any()
                    ? string.Join(", ",
                        similarCollateralGroups.Select(g =>
                            $"{g.Count()}({g.Key.Category}, {g.Key.Type}, {g.Key.ManufactureYear})"))
                    : "No similar items";


                var totalCollaterals = totalcollateralForCase.Count;
                var deliveredCollaterals = totalcollateralForCase.Count(ca => ca.Status == "Complete");
                var returnedCollaterals = totalcollateralForCase.Count(ca => ca.Status == "Returned"); // Assuming "Returned" is a status
                var onHandCollaterals = totalcollateralForCase.Count(ca => ca.Status != "Complete" && ca.Status != "Returned"); // Assuming these are "in progress" or "pending"
                double deliveredPercentage = totalCollaterals > 0 ? ((double)deliveredCollaterals / (totalCollaterals - returnedCollaterals)) * 100 : 0;
                var dto = new ValuationReportDto
                {
                    Id = uniqueCase.Id,
                    CaseNo = uniqueCase.CaseNo,
                    CurrentStatus = uniqueCase.Status,
                    CreatedAt = uniqueCase.CreationAt,
                    ApplicantName = uniqueCase.ApplicantName,
                    RequestedOrgan = uniqueCase.Segment ?? " ",
                    CustomerApplicantRelationship = uniqueCase.CaseOriginator?.Name ?? "",
                    DistrictName = uniqueCase.District?.Name ?? "Head Office",

                    AssignedNo = totalCollaterals,
                    DeliveredNo = deliveredCollaterals,
                    ReturnedWithAdvice = returnedCollaterals,
                    OnHandNo = onHandCollaterals,
                    DeliveredPercentage = deliveredPercentage,
                    QuantityForSimilarMechanicalItem = similarItemsDescription,

                    CasePriority = " ",
                    CaseFRQ = "",
                    PurposeOfValuationRequest = "",
                    RequestedEngineer = "",
                    ProcessingWCDays = null,
                    SDTAccomplishment = "",
                    FulfillmentOfDocumentation = "Complete",
                    ScheduledVisitDate = null,
                    Location = "",
                    QuantityComplexityOfProperty = " ",
                    LHCTitleDeedSerialNo = null,
                    TypeOfProperty = "",
                    PropertyCategory = "",
                    SiteInspectionDate = null,
                    DateReturnedWithAdviceToRO = null,
                    ReasonOfReturn = null,
                    //maker section
                    NameOfValuator = "",
                    DateCaseDeliveredToValuationOffice = null,
                    DateCaseAssignedToTeamLeader = null,
                    DateCaseAssignedToValuators = null,
                    LastRecentValuationDate = null,
                    NetDaysConsumed = null,
                    DurationReceiptGrossDays = null,
                    DurationAssignedToTMGrossDays = null,
                    DurationAssignedGrossDays = null,
                    // start checking
                    NameOfChecker = "",
                    DateSentForChecking = null,
                    DateCaseAssignedToCheckerTeamLeader = null,
                    DateCaseAssignedToCheckerValuators = null,
                    DateReportSentToRequestingOrgan = null, //this is the date co sent to
                    // it needs some modification when the collateral evaluation is retured to correct by checker, and this is the time checker officer uses to check the collateral, .. timeConsumed != null ? (int?)timeConsumed.Duration.TotalDays : null,
                    GrossDaysConsumedChecker = null,
                    DurationCheckerReceiptGrossDays = null,
                    DurationCheckerAssignedToTMGrossDays = null,
                    DurationCheckerAssignedGrossDays = null,
                    TotalNumberOfComments = null,
                    DateCommentReceivedFromChecking = null,
                };
                distinctCaseDtos.Add(dto);
            }
            return (DistinctCases: distinctCaseDtos, AllProductionCapacities: allCollateralDtos);
        }


        /// ..   start pce internal report

        public async Task<(IEnumerable<ValuationReportDto> DistinctCases, IEnumerable<ValuationReportDto> AllProductionCapacities)> GetInternalPCECaseReport(Guid userId)
        {

            // Step 1: Fetch all relevant PCECaseAssignments for the user, eager loading all primary related entities.
            string userRole = _httpContextAccessor.HttpContext.Session.GetString("userRole");

            if (userRole == "")
            {
                return (DistinctCases: Enumerable.Empty<ValuationReportDto>(), AllProductionCapacities: Enumerable.Empty<ValuationReportDto>());
            }

            var caseAssignmentsQuery = _cbeContext.PCECaseAssignments
                .Include(ca => ca.ProductionCapacity)
                    .ThenInclude(pc => pc.PCECase)
                        .ThenInclude(pce => pce.PCECaseOriginator)
                .Include(ca => ca.ProductionCapacity)
                    .ThenInclude(pc => pc.PCECase)
                        .ThenInclude(pce => pce.District)
                .Include(ca => ca.User)
                as IQueryable<PCECaseAssignment>;

            // Apply user filter only if not Higher Official
            if (userRole != "Higher Official")
            {
                caseAssignmentsQuery = caseAssignmentsQuery.Where(ca => ca.UserId == userId);
            }

            var caseAssignments = await caseAssignmentsQuery.ToListAsync();





            // If no assignments, return empty results early
            if (!caseAssignments.Any())
            {
                return (DistinctCases: Enumerable.Empty<ValuationReportDto>(), AllProductionCapacities: Enumerable.Empty<ValuationReportDto>());
            }

            // Collect all unique ProductionCapacity IDs and PCECase IDs from the fetched assignments
            var productionCapacityIds = caseAssignments.Select(ca => ca.ProductionCapacityId).ToHashSet(); // Use HashSet for O(1) lookups
            var pceCaseIds = caseAssignments.Select(ca => ca.ProductionCapacity.PCECaseId).Distinct().ToHashSet();

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


            //caseFRQ
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



                // Get Maker Manager assignment
                var makerManagerAssignment = await _cbeContext.PCECaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.ProductionCapacityId == pc.Id &&
                                 ca.User.Role.Name == "Maker Manager")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();
                // Get Maker Manager assignment
                var makerTeamLeaderAssignment = await _cbeContext.PCECaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.ProductionCapacityId == pc.Id &&
                                 ca.User.Role.Name == "Maker TeamLeader")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                // Get Maker Manager assignment
                var makerOfficerAssignment = await _cbeContext.PCECaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.ProductionCapacityId == pc.Id &&
                                 ca.User.Role.Name == "Maker Officer")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();
                // Get Maker Manager assignment
                var checkerManagerAssignment = await _cbeContext.PCECaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.ProductionCapacityId == pc.Id &&
                                 ca.User.Role.Name == "Checker Manager")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();
                var checkerTeamLeaderAssignment = await _cbeContext.PCECaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.ProductionCapacityId == pc.Id &&
                                 ca.User.Role.Name == "Checker TeamLeader")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();

                var checkerOfficerAssignment = await _cbeContext.PCECaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.ProductionCapacityId == pc.Id &&
                                 ca.User.Role.Name == "Checker Officer")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();


                var districtEvaluationManagerAssignment = await _cbeContext.PCECaseAssignments
                    .Include(ca => ca.User)
                        .ThenInclude(u => u.Role)
                    .Where(ca => ca.ProductionCapacityId == pc.Id &&
                                 ca.User.Role.Name == "District Valuation Manager")
                    .OrderByDescending(ca => ca.AssignmentDate)
                    .FirstOrDefaultAsync();
                var dto = new ValuationReportDto
                {

                    Id = pc.Id,
                    CaseNo = pceCase.CaseNo,
                    CurrentStatus = pc.CurrentStatus,
                    CreatedAt = pc.CreatedAt, // Set to PCECase CreatedAt
                    ApplicantName = pceCase.ApplicantName,
                    CasePriority = " ", // This seems constant, consider if it should be dynamic
                    CaseFRQ = (pcReestimations?.Count ?? 0).ToString(),
                    RequestedOrgan = pceCase.Segment ?? " ",
                    CustomerApplicantRelationship = pceCase.PCECaseOriginator?.Name ?? "",
                    DateCaseDeliveredToValuationOffice = makerManagerAssignment?.AssignmentDate,
                    DateCaseAssignedToTeamLeader = makerTeamLeaderAssignment?.AssignmentDate,
                    DateCaseAssignedToValuators = makerOfficerAssignment?.AssignmentDate,
                    PurposeOfValuationRequest = pc.Purpose,
                    DistrictName = pceCase.District?.Name ?? "Head Office",
                    LastRecentValuationDate = pcEvaluations.Any() ? pcEvaluations.Max(e => e.CreatedAt) : null,
                    DurationReceiptGrossDays = pc.CurrentStatus == "Completed" && pcEvaluations.Any() && pcEvaluations.First().CompletedAt != null
                                                    ?GetDateDifference(pcEvaluations.First().CompletedAt.Value, pc.CreatedAt)
                                                    : pceCase.CreatedAt != default ? GetDateDifference(DateTime.Now, pc.CreatedAt) : null,
                    DurationAssignedGrossDays = makerOfficerAssignment?.AssignmentDate != null && pceCase.District?.Name == "Head Office" && makerTeamLeaderAssignment?.AssignmentDate != null
                                                        ? GetDateDifference(makerOfficerAssignment.AssignmentDate, makerTeamLeaderAssignment.AssignmentDate)
                                                        : makerOfficerAssignment?.AssignmentDate != null && makerManagerAssignment?.AssignmentDate != null
                                                            ? GetDateDifference(makerOfficerAssignment.AssignmentDate, makerManagerAssignment.AssignmentDate)
                                                            : null,
                    DurationAssignedToTMGrossDays = pceCase.District?.Name == "Head Office" &&
                                                        makerManagerAssignment?.AssignmentDate != null &&
                                                        makerTeamLeaderAssignment?.AssignmentDate != null
                                                            ? GetDateDifference(
                                                                makerTeamLeaderAssignment.AssignmentDate,
                                                                makerManagerAssignment.AssignmentDate)
                                                            : null,
                    RequestedEngineer = "Mechanical", // Constant
                    ProcessingWCDays = null, // Constant
                    QuantityForSimilarMechanicalItem = "", // Constant or placeholder, review this logic
                    NameOfValuator = firstEvaluation?.Evaluator?.Name ?? " ",
                    AssignedNo = -1, // Review this logic, seems constant or placeholder
                    DeliveredNo = -1, // Review this logic, seems constant or placeholder
                    ReturnedWithAdvice = -1, // Review this logic, seems constant or placeholder
                    OnHandNo = -1, // Review this logic, seems constant or placeholder
                    DeliveredPercentage = -1, // Review this logic, seems constant or placeholder
                    NetDaysConsumed = scheduleDate != null && firstEvaluation != null && firstEvaluation.CompletedAt.HasValue
                                       ? GetDateDifference(firstEvaluation.CompletedAt.Value, scheduleDate.ScheduleDate)
                                       : firstEvaluation != null && firstEvaluation.CompletedAt.HasValue
                                           ?GetDateDifference(DateTime.Now, firstEvaluation.CompletedAt.Value)
                                           : null,

                    SDTAccomplishment = " ", // Constant
                    FulfillmentOfDocumentation = "Complete", // Constant
                    ScheduledVisitDate = scheduleDate?.ScheduleDate,
                    Location = firstEvaluation?.InspectionPlace ?? $"{pc.Region}, {pc.City}, {pc.SubCity}, {pc.Wereda}, {pc.Kebele}",
                    QuantityComplexityOfProperty = " ", // Constant                    LHCTitleDeedSerialNo = pc.LHCNumber ?? pc.SerialNo,

                    LHCTitleDeedSerialNo = pc.LHCNumber ?? pc.SerialNo,
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
                    GrossDaysConsumed = pceCase.CompletedAt != null ? GetDateDifference(pceCase.CreatedAt, pceCase.CompletedAt.Value) : GetDateDifference(pceCase.CreatedAt, DateTime.Now),
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




                // 1. First select all collaterals for this case
                var caseProductions = await _cbeContext.ProductionCapacities
                    .Where(c => c.PCECaseId == pceCase.Id)
                    .Select(c => new { c.Category, c.Type, c.ManufactureYear })
                    .ToListAsync();

                // 2. Then filter and group similar collaterals
                var similarCollateralGroups = caseProductions
                    .GroupBy(c => new
                    {
                        Category = c.Category.ToString(),
                        Type = c.Type,
                        ManufactureYear = c.ManufactureYear
                    })
                    .Where(g => g.Count() > 1) // Only show groups with duplicates
                    .OrderByDescending(g => g.Count())
                    .ToList();

                // 3. Format the similar items description
                var similarItemsDescription = similarCollateralGroups.Any()
                    ? string.Join(", ",
                        similarCollateralGroups.Select(g =>
                            $"{g.Count()}({g.Key.Category}, {g.Key.Type}, {g.Key.ManufactureYear})"))
                    : "No similar items";





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
                    CasePriority = " ",
                    CaseFRQ = caseEvaluations.Count.ToString(),
                    RequestedOrgan = pceCase.PCECaseOriginator.Department ?? "RM",
                    CustomerApplicantRelationship = pceCase.PCECaseOriginator?.Name ?? "RM Name",
                    DateCaseDeliveredToValuationOffice = makerManagerAssignment?.AssignmentDate,
                    DateCaseAssignedToTeamLeader = makerTeamLeaderAssignment?.AssignmentDate,
                    DateCaseAssignedToValuators = makerOfficerAssignment?.AssignmentDate,
                    PurposeOfValuationRequest = firstProductionCapacity?.Purpose ?? " ",
                    LastRecentValuationDate = caseEvaluations.Any() ? caseEvaluations.Max(e => e.CreatedAt) : null,
                    DurationReceiptGrossDays = pceCase.Status == "Completed" && caseEvaluations.Any() && caseEvaluations.FirstOrDefault()?.CompletedAt != null
                                                ? GetDateDifference(caseEvaluations.First().CompletedAt.Value, pceCase.CreatedAt)
                                                : caseEvaluations.FirstOrDefault()?.CompletedAt == null ? GetDateDifference(pceCase.CreatedAt, DateTime.Now) : null,
                    DistrictName = pceCase.District?.Name ?? "Head Office",
                    DurationAssignedGrossDays = makerOfficerAssignment?.AssignmentDate != null && pceCase.District?.Name == "Head Office" && makerTeamLeaderAssignment?.AssignmentDate != null
                                                    ? GetDateDifference(makerOfficerAssignment.AssignmentDate, makerTeamLeaderAssignment.AssignmentDate)
                                                    : makerOfficerAssignment?.AssignmentDate != null && makerManagerAssignment?.AssignmentDate != null
                                                        ? GetDateDifference(makerOfficerAssignment.AssignmentDate, makerManagerAssignment.AssignmentDate)
                                                        : null,
                    DurationAssignedToTMGrossDays = pceCase.District?.Name != "Head Office" &&
                                                    makerManagerAssignment?.AssignmentDate != null &&
                                                    makerTeamLeaderAssignment?.AssignmentDate != null
                                                        ?GetDateDifference(
                                                            makerTeamLeaderAssignment.AssignmentDate,
                                                            makerManagerAssignment.AssignmentDate)
                                                        : null,
                    RequestedEngineer = "Mechanical",
                    ProcessingWCDays = null,
                    QuantityForSimilarMechanicalItem = similarItemsDescription ?? "",
                    NameOfValuator = firstEvaluation?.Evaluator?.Name ?? makerOfficerAssignment?.User?.Name ?? "",
                    AssignedNo = caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id),
                    DeliveredNo = caseAssignments.Count(ca => ca.ProductionCapacity?.PCECaseId == pceCase.Id && ca.Status == "Completed"),
                    ReturnedWithAdvice = caseReestimations.Count,
                    OnHandNo = caseAssignments.Count(ca => ca.ProductionCapacity?.PCECaseId == pceCase.Id && ca.Status != "Completed"),
                    DeliveredPercentage = caseAssignments.Any(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id)
                                            ? (double)caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id && ca.Status == "Completed") / caseAssignments.Count(ca => ca.ProductionCapacity.PCECaseId == pceCase.Id) * 100
                                            : 0,
                    NetDaysConsumed = scheduleDate != null && firstEvaluation != null && firstEvaluation.CompletedAt.HasValue
                                        ? GetDateDifference( firstEvaluation.CompletedAt.Value, scheduleDate.ScheduleDate)
                                        : scheduleDate != null
                                            ?GetDateDifference(DateTime.Now, scheduleDate.ScheduleDate)
                                            : null,
                    SDTAccomplishment = " ",
                    FulfillmentOfDocumentation = pceCase.BusinessLicenseId != null ? "Complete" : "Incomplete",
                    ScheduledVisitDate = scheduleDate?.ScheduleDate,
                    Location = firstEvaluation?.InspectionPlace ?? (firstProductionCapacity != null ? $"{firstProductionCapacity.Region}, {firstProductionCapacity.City}, {firstProductionCapacity.SubCity}, {firstProductionCapacity.Wereda}, {firstProductionCapacity.Kebele}" : " "),
                    QuantityComplexityOfProperty = " ",
                    LHCTitleDeedSerialNo = firstProductionCapacity?.LHCNumber ?? " ",
                    TypeOfProperty = firstProductionCapacity?.Type ?? " ",
                    PropertyCategory = firstProductionCapacity?.Category.ToString() ?? " ",
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


        //public TimeSpan GetDateDifference(DateTime date1, DateTime date2)
        //{
        //    return date1 - date2;
        //}
        public string GetDateDifference(DateTime date1, DateTime date2)
        {
                if (date1 == null || date2 == null)
                    return " ";

                TimeSpan difference = date1 > date2 ? date1 - date2 : date2 - date1; // Ensure positive difference
            return $"{difference.Days} days, {difference.Hours} hours, {difference.Minutes} minutes";
        }

    }
}
