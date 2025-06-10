using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Text.Encodings.Web;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

using mechanical.Data;
using mechanical.Utils;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.UploadFileService;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.PCECaseTimeLineService;

namespace mechanical.Services.PCE.ProductionCapacityService
{
    public class ProductionCapacityService : IProductionCapacityService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IUploadFileService _UploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductionCapacityService> _logger;
        private readonly IPCEEvaluationService _PCEEvaluationService;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;

        public ProductionCapacityService(CbeContext cbeContext, IMapper mapper, ILogger<ProductionCapacityService> logger, IHttpContextAccessor httpContextAccessor, IPCEEvaluationService PCEEvaluationService, IUploadFileService uploadFileService, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
            _PCEEvaluationService = PCEEvaluationService;
            _IPCECaseTimeLineService = IPCECaseTimeLineService;
            _httpContextAccessor = httpContextAccessor;
            _UploadFileService = uploadFileService;

        }

        public async Task<ProductionCapacity> CreateProductionCapacity(Guid UserId, Guid PCECaseId, ProductionPostDto createProductionDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                // Encode/Sanitize inputs in Dto to avoid unsafe data being saved
                EncodingHelper.EncodeObject(createProductionDto);
                // SanitizerHelper.SanitizeObject(createProductionDto);

                var production = _mapper.Map<ProductionCapacity>(createProductionDto);
                production.Id = Guid.NewGuid();
                production.PCECaseId = PCECaseId;

                try
                {
                    await this.UploadFile(UserId, "PCE Owner LHC Certificate", production, createProductionDto.LHCDocument);
                    await this.UploadFile(UserId, "PCE Shade Rent Agreement", production, createProductionDto.ShadeRentAgreement);
                    await this.UploadFile(UserId, "PCE Business License", production, createProductionDto.BusinessLicense);
                    await this.UploadFile(UserId, "PCE Machine Specification Document", production, createProductionDto.MachineSpecificationDocument);
                    await this.UploadFile(UserId, "PCE Machine Operation Manual", production, createProductionDto.MachineOperationManual);

                    if (createProductionDto.OtherDocuments != null)
                    {
                        foreach (var otherDocument in createProductionDto.OtherDocuments)
                        {
                            await this.UploadFile(UserId, "PCE Other Supportive Document", production, otherDocument);
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("unable to upload file");
                }

                production.CreatedAt = DateTime.UtcNow;
                production.CreatedById = UserId;
                production.CurrentStage = "Relation Manager";
                production.CurrentStatus = "New";
                production.ProductionType = "Manufacturing";

                await _cbeContext.ProductionCapacities.AddAsync(production);

                var pceCaseAssignment = new PCECaseAssignment
                {
                    ProductionCapacityId = production.Id,
                    UserId = UserId,
                    AssignmentDate = DateTime.UtcNow,
                    CompletedAt = null,
                    Status = "New"
                };
                await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);

                // Sanitize `Activity` field to avoid any XSS issues when storing user-generated content
                // Using HtmlEncoder to encode activity content
                var activity = $"<strong>A new Manufacturing PCE has been added. </strong> <br> " +
                                    $"<i class='text-purple'>Property Owner:</i> {HtmlEncoder.Default.Encode(production.PropertyOwner)}. &nbsp; " +
                                    $"<i class='text-purple'>Role:</i> {HtmlEncoder.Default.Encode(production.Role)}. &nbsp; " +
                                    $"<i class='text-purple'>Manufacturing Main Sector:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; " +
                                    $"<i class='text-purple'>Manufacturing Sub Sector:</i> {HtmlEncoder.Default.Encode(production.Type)}.";

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    PCECaseId = production.PCECaseId,
                    Activity = activity,
                    CurrentStage = "Relation Manager"
                });

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return production;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating production.");
            }
        }

        public async Task<bool> DeleteProduction(Guid UserId, Guid id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var production = await _cbeContext.ProductionCapacities
                                                    .Include(p => p.PCECase)
                                                        .ThenInclude(p => p.ProductionCapacities)
                                                    .Where(c => c.Id == id && c.CreatedById == UserId && c.CurrentStage == "Relation Manager" && c.CurrentStatus == "New")
                                                    .FirstOrDefaultAsync();
                if (production == null)
                {
                    return false;
                }

                var deleteassignment = await _cbeContext.PCECaseAssignments.Where(c => c.ProductionCapacityId == id && c.UserId == UserId && c.Status == "New").ToListAsync();
                if (deleteassignment.Any())
                {
                    _cbeContext.RemoveRange(deleteassignment);
                }

                var deletedFiles = await _cbeContext.UploadFiles.Where(c => c.CollateralId == id && c.userId == UserId).ToListAsync();
                if (deletedFiles.Any())
                {
                    _cbeContext.UploadFiles.RemoveRange(deletedFiles);
                }

                _cbeContext.Remove(production);

                var Fileproduction = await _cbeContext.UploadFiles.Where(c => c.CollateralId == id).FirstOrDefaultAsync();
                if (Fileproduction != null)
                {
                    _cbeContext.Remove(Fileproduction);
                }

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    PCECaseId = production.PCECaseId,
                    Activity = $" <strong>A Manufacturing production with <class='text-purple'>Id: {production.Id} has been deleted. </strong>.",
                    CurrentStage = "Relation Manager"
                });


                var allCompleted = production.PCECase?.ProductionCapacities?.All(pc => pc.CurrentStatus == "Completed") ?? false;

                if (allCompleted)
                {
                    production.PCECase.Status = "Completed";
                    _cbeContext.PCECases.Update(production.PCECase);
                }

                var allNew = production.PCECase?.ProductionCapacities?.All(pc => pc.CurrentStatus == "New") ?? false;
                if (allNew)
                {
                    production.PCECase.Status = "New";
                    _cbeContext.PCECases.Update(production.PCECase);
                }

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting production");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while deleting production.");
            }
        }

        public async Task<ProductionCapacity> EditProduction(Guid UserId, Guid ProductionCapacityId, ProductionEditDto createProductionDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var production = await _cbeContext.ProductionCapacities.FindAsync(ProductionCapacityId);
                if (production == null)
                {
                    throw new Exception("PCE not Found");
                }
                if (production.CreatedById != UserId)
                {
                    throw new Exception("you don't have permission");
                }
                if (production.CurrentStage != "Relation Manager")
                {
                    throw new Exception("unable to Edit PCE");
                }
                createProductionDto.PCECaseId = production.PCECaseId;
                _mapper.Map(createProductionDto, production);

                if (createProductionDto.MachineryInstalledPlace == "Private Owned LHC")
                {
                    production.Industrialpark = null;
                }
                else if (createProductionDto.MachineryInstalledPlace == "Industrial Park")
                {
                    production.LHCNumber = null;
                    production.OwnerName = null;
                }

                production.UpdatedAt = DateTime.UtcNow;
                production.UpdatedById = UserId;
                _cbeContext.ProductionCapacities.Update(production);

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    PCECaseId = production.PCECaseId,
                    Activity = $" <strong>A Manufacturing production with <class='text-purple'>Id: {production.Id} has been Updated. </strong>.",


                    CurrentStage = "Relation Manager"
                });

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return production;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating production");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating production.");
            }
        }

        private async Task UploadFile(Guid UserId, string Category, ProductionCapacity production, IFormFile? file)
        {
            if (file != null)
            {
                await _UploadFileService.CreateUploadFile(UserId, new CreateFileDto()
                {
                    File = file,
                    CaseId = production.PCECaseId,
                    CollateralId = production.Id,
                    Category = Category
                });
            }
        }

        public async Task<bool> DeleteProductionFile(Guid UserId, Guid Id)
        {
            var file = await _cbeContext.UploadFiles.FindAsync(Id);
            if (file == null)
            {
                return false;
            }
            if (file.userId != UserId)
            {
                return false;
            }
            if (await _UploadFileService.DeleteFile(file.Id))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UploadProductionFile(Guid UserId, IFormFile File, Guid ProductionId, string DocumentCategory)
        {
            var production = await _cbeContext.ProductionCapacities.FindAsync(ProductionId);

            if (production == null)
            {
                return false;
            }

            var productionFile = new CreateFileDto()
            {
                File = File ?? throw new ArgumentNullException(nameof(File)),
                CollateralId = production.Id,
                Category = DocumentCategory,
                CaseId = production.PCECaseId,

            };

            if (await _UploadFileService.CreateUploadFile(UserId, productionFile) != Guid.Empty)
            {
                return true;
            }

            return false;
        }

        public async Task<ProductionReturnDto> GetProduction(Guid UserId, Guid Id)
        {
            var production = await _cbeContext.ProductionCapacities.AsNoTracking().Include(pc => pc.PCECase).FirstOrDefaultAsync(pc => pc.Id == Id);
            var pceAssignment = await _cbeContext.PCECaseAssignments.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == Id && res.UserId == UserId);
            var productionDto = _mapper.Map<ProductionReturnDto>(production);
            productionDto.AssignmentStatus = pceAssignment?.Status;
            return productionDto;
        }

        public async Task<ProductionDetailDto> GetProductionDetails(Guid UserId, Guid Id)
        {

            var pce = await GetProduction(UserId, Id);
            var reestimation = await _cbeContext.ProductionReestimations.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == Id);
            var relatedFiles = await _UploadFileService.GetUploadFileByCollateralId(Id);
            var valuationHistory = await _PCEEvaluationService.GetValuationHistory(UserId, Id);
            var returnedProductions = await _cbeContext.ReturnedProductions
                                                        .AsNoTracking()
                                                        .Include(pr => pr.ReturnedBy)
                                                        .Where(pr => pr.PCEId == Id)
                                                        .OrderByDescending(pr => pr.ReturnedAt)
                                                        .ToListAsync();

            return new ProductionDetailDto
            {
                PCECase = pce.PCECase,
                ProductionCapacity = _mapper.Map<ProductionReturnDto>(pce),
                PCEValuationHistory = valuationHistory,
                Reestimation = reestimation,
                RelatedFiles = relatedFiles,
                ReturnedProductions = _mapper.Map<IEnumerable<ReturnedProductionDto>>(returnedProductions)
            };
        }


        public async Task<IEnumerable<ProductionReturnDto>> GetProductions(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            var productions = await _cbeContext.PCECaseAssignments
                                            .AsNoTracking()
                                            .Where(a => a.UserId == UserId
                                                && (Status == null || Status == "All" || a.Status == Status))
                                            .Join(
                                                _cbeContext.ProductionCapacities,
                                                pca => pca.ProductionCapacityId,
                                                pc => pc.Id,
                                                (pca, pc) => new
                                                {
                                                    ProductionCapacity = pc,
                                                    AssignmentStatus = pca.Status,
                                                    AssignmentDate = pca.AssignmentDate,
                                                    PCECase = pc.PCECase
                                                })
                                            .Where(x => (x.AssignmentStatus != null || x.ProductionCapacity.AssignedEvaluatorId == UserId)
                                                && (PCECaseId == null || x.ProductionCapacity.PCECaseId == PCECaseId)
                                                && (Stage == null || x.ProductionCapacity.CurrentStage == Stage)
                                            // && (Status == null || Status.Equals("All", StringComparison.OrdinalIgnoreCase) || x.ProductionCapacity.CurrentStatus == Status) 
                                            // && (string.IsNullOrEmpty(Status) || x.ProductionCapacity.CurrentStatus != "Returned")
                                            )
                                            .OrderByDescending(x => x.AssignmentDate)
                                            .ToListAsync();

            return productions.Select(x =>
            {
                var dto = _mapper.Map<ProductionReturnDto>(x.ProductionCapacity);
                dto.AssignmentStatus = x.AssignmentStatus;
                dto.PCECase = x.PCECase;
                return dto;
            }).ToList();
        }

        // public async Task<int> GetProductionCount(Guid UserId, Guid? PCECaseId, string Stage = null, string Status = null)
        // {
        //     return (await GetProductions(UserId, PCECaseId, Stage, Status)).Count();
        // }

        public async Task<int> GetProductionCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            return await _cbeContext.PCECaseAssignments
                .AsNoTracking()
                .Where(a => a.UserId == UserId)
                .Join(
                    _cbeContext.ProductionCapacities,
                    pca => pca.ProductionCapacityId,
                    pc => pc.Id,
                    (pca, pc) => new
                    {
                        ProductionCapacity = pc,
                        AssignmentStatus = pca.Status
                    })
                .Where(x => (x.AssignmentStatus != null || x.ProductionCapacity.AssignedEvaluatorId == UserId)
                    && (PCECaseId == null || x.ProductionCapacity.PCECaseId == PCECaseId)
                    && (Stage == null || x.ProductionCapacity.CurrentStage == Stage)
                // && (Status == null || Status.Equals("All", StringComparison.OrdinalIgnoreCase) || x.ProductionCapacity.CurrentStatus == Status) 
                // && (string.IsNullOrEmpty(Status) || x.ProductionCapacity.CurrentStatus != "Returned")
                )
                .CountAsync();
        }

        public async Task<ProductionCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null)
        {
            var Statuses = new[] { "New", "Pending", "Completed", "Reestimate", "Reestimated", "All" };
            var tasks = Statuses.Select(Status => GetProductionCountAsync(UserId, PCECaseId, Stage, Status)).ToList();

            var counts = await Task.WhenAll(tasks);

            return new ProductionCountDto()
            {
                NewProductionCount = counts[0],
                PendingProductionCount = counts[1],
                CompletedProductionCount = counts[2],
                ResubmittedProductionCount = counts[3], // returned 
                ReestimatedProductionCount = counts[4],
                TotalProductionCount = counts[5]
                // TotalProductionCount = await GetProductionCountAsync(UserId, PCECaseId, Stage)
            };
        }

        // public async Task<IEnumerable<ProductionReturnDto>> GetReturnedProductions(Guid UserId)
        // {
        //     var returnedCapacitiesQuery = from return in _cbeContext.ReturnedProductions
        //                                   join capacity in _cbeContext.ProductionCapacities
        //                                   on return.PCEId equals capacity.Id
        //                                   // .AsNoTracking()
        //                                   where return.ReturnedBy == UserId
        //                                   select capacity;

        //     var returnedCapacities = await returnedCapacitiesQuery.ToListAsync();


        //     return _mapper.Map<IEnumerable<ReturnedProductionDto>>(returnedCapacities); ;
        // }

        public async Task<IEnumerable<ProductionReturnDto>> GetRemarkProductions(Guid UserId, Guid PCECaseId)
        {
            var pceCaseAssignments = await _cbeContext.PCECaseAssignments
                                                            .Include(res => res.ProductionCapacity)
                                                            .Where(res => res.UserId == UserId
                                                                        && res.ProductionCapacity.PCECaseId == PCECaseId
                                                                        && res.Status.Contains("Remark"))
                                                            .ToListAsync();
            var productions = pceCaseAssignments.Select(res => res.ProductionCapacity);
            return _mapper.Map<IEnumerable<ProductionReturnDto>>(productions);
        }

        public async Task<IEnumerable<ProductionAssignmentDto>> GetAssignedProductions(Guid UserId, Guid PCECaseId)
        {
            var supervisedUsers = await _cbeContext.Users.Where(user => user.SupervisorId == UserId).Select(user => user.Id).ToListAsync();

            var productionAssignments = await _cbeContext.PCECaseAssignments
                                                        .AsNoTracking()
                                                        .Include(x => x.User)
                                                        .Include(x => x.ProductionCapacity)
                                                        .Where(assignment => supervisedUsers.Contains(assignment.UserId) && assignment.ProductionCapacity.PCECaseId == PCECaseId)
                                                        .ToListAsync();

            var distinctAssignments = productionAssignments
                                        // .DistinctBy(assignment => assignment.ProductionCapacityId)
                                        .GroupBy(assignment => assignment.ProductionCapacityId)
                                        .Select(g => g.OrderByDescending(assignment => assignment.AssignmentDate).FirstOrDefault())
                                        .Select(assignment => new ProductionAssignmentDto
                                        {
                                            ProductionCapacityId = assignment.ProductionCapacityId,
                                            PCECaseId = PCECaseId,
                                            PropertyOwner = assignment.ProductionCapacity.PropertyOwner,
                                            PCECaseAssignmentId = assignment.Id,
                                            Role = assignment.ProductionCapacity.Role,
                                            Type = assignment.ProductionCapacity.Type,
                                            Category = "Test Category",
                                            // Category = EnumToDisplayName(assignment.ProductionCapacity.Category),
                                            User = assignment.User.Name,
                                            AssignmentDate = assignment.AssignmentDate,
                                            Status = assignment.Status,
                                        }).ToList();

            return distinctAssignments.ToList();
        }

        string EnumToDisplayName<TEnum>(TEnum enumValue)
        {
            return (typeof(TEnum).GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute)?.Name ?? enumValue.ToString();
        }
        // HO
        public async Task<IEnumerable<ProductionReturnDto>> GetHOProductions(Guid? PCECaseId = null, string Status = null)
        {
            // If the status is "Pending", filter for CurrentStage "FIRETN"
            if (string.Equals(Status, "Pending", StringComparison.OrdinalIgnoreCase) && PCECaseId.HasValue)
            {
                var productions = await _cbeContext.ProductionCapacities
                    .AsNoTracking()
                    .Where(pca => pca.PCECaseId == PCECaseId && pca.CurrentStage != "Relation Manager")
                    .ToListAsync();
                foreach (var item in productions)
                {
                    if (item.CurrentStage != "Relation Manager")
                    {
                        item.CurrentStatus = "Pending";
                    }
                }
                // Map to ProductionReturnDto
                return _mapper.Map<IEnumerable<ProductionReturnDto>>(productions);
            }
            if (Status== "Completed")
            {
                var productions = await _cbeContext.ProductionCapacities
                    .AsNoTracking()
                    .Where(pca => pca.PCECaseId == PCECaseId && pca.CurrentStage == "Relation Manager"&& pca.CurrentStatus== "Completed")
                    .ToListAsync();                
                // Map to ProductionReturnDto
                return _mapper.Map<IEnumerable<ProductionReturnDto>>(productions);
            }
            if (Status == "New")
            {
                var productions = await _cbeContext.ProductionCapacities
                    .AsNoTracking()
                    .Where(pca => pca.PCECaseId == PCECaseId && pca.CurrentStage == "Relation Manager" && pca.CurrentStatus == "New")
                    .ToListAsync();
                // Map to ProductionReturnDto
                return _mapper.Map<IEnumerable<ProductionReturnDto>>(productions);
            }
            if (Status == "Returned")
            {
                var productions = await _cbeContext.ProductionCapacities
                    .AsNoTracking()
                    .Where(pca => pca.PCECaseId == PCECaseId && pca.CurrentStage == "Relation Manager" && pca.CurrentStatus == "Returned")
                    .ToListAsync();
                // Map to ProductionReturnDto
                return _mapper.Map<IEnumerable<ProductionReturnDto>>(productions);
            }
            // Default case: get all productions based on the PCECaseId
            var allProductions = await _cbeContext.ProductionCapacities
                .AsNoTracking()
                .Where(pca => PCECaseId == null || pca.PCECaseId == PCECaseId)
                .ToListAsync();

            // Update CurrentStatus for items not in "Relation Manager"
            foreach (var item in allProductions)
            {
                if (item.CurrentStage != "Relation Manager")
                {
                    item.CurrentStatus = "Pending";
                }
            }

            // Map to ProductionReturnDto
            var productionDto = _mapper.Map<IEnumerable<ProductionReturnDto>>(allProductions);
            return productionDto;
        }
        public async Task<ProductionReturnDto> GetHOProduction( Guid Id)
        {
            var production = await _cbeContext.ProductionCapacities.AsNoTracking().Include(pc => pc.PCECase).FirstOrDefaultAsync(pc => pc.Id == Id);
            var pceAssignment = await _cbeContext.PCECaseAssignments.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == Id /*&& res.UserId == UserId*/);
            var productionDto = _mapper.Map<ProductionReturnDto>(production);
            productionDto.AssignmentStatus = pceAssignment?.Status;
            return productionDto;
        }

        public async Task<ProductionDetailDto> GetHOProductionDetails(Guid Id)
        {

            var pce = await GetHOProduction(Id);
            var reestimation = await _cbeContext.ProductionReestimations.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == Id);
            var relatedFiles = await _UploadFileService.GetUploadFileByCollateralId(Id);
            var valuationHistory = await _PCEEvaluationService.GetHOValuationHistory(Id);
            var returnedProductions = await _cbeContext.ReturnedProductions
                                                        .AsNoTracking()
                                                        .Include(pr => pr.ReturnedBy)
                                                        .Where(pr => pr.PCEId == Id)
                                                        .OrderByDescending(pr => pr.ReturnedAt)
                                                        .ToListAsync();

            return new ProductionDetailDto
            {
                PCECase = pce.PCECase,
                ProductionCapacity = _mapper.Map<ProductionReturnDto>(pce),
                PCEValuationHistory = valuationHistory,
                Reestimation = reestimation,
                RelatedFiles = relatedFiles,
                ReturnedProductions = _mapper.Map<IEnumerable<ReturnedProductionDto>>(returnedProductions)
            };
        }


    }
}

