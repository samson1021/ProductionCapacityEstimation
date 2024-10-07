using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Operations;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.UploadFileService;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto;
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
                var production = _mapper.Map<ProductionCapacity>(createProductionDto);
                production.Id = Guid.NewGuid();
                production.PCECaseId = PCECaseId;
                try
                {
                    await this.UploadFile(UserId, "PCE Owner LHC Certificate", production, createProductionDto.UploadLHC);
                    await this.UploadFile(UserId, "PCE Shade Rent Agreement", production, createProductionDto.ploadshaderentagreement);
                    await this.UploadFile(UserId, "PCE Business license", production, createProductionDto.Uploadbusinesslicense);
                    await this.UploadFile(UserId, "PCE Machine specification document", production, createProductionDto.Machinespecificationdocumen);
                    await this.UploadFile(UserId, "PCE Machine operation manual", production, createProductionDto.Machineoperationmanual);
                    
                    if (createProductionDto.OtherDocument != null)
                    {
                        foreach (var otherDocument in createProductionDto.OtherDocument)
                        {
                            await this.UploadFile(UserId, "PCE Other Supportive Document", production, otherDocument);
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("unable to upload file");
                }

                production.CreationDate = DateTime.Now;
                production.CreatedById = UserId;
                production.CurrentStage = "Relation Manager";
                production.CurrentStatus = "New";
                production.ProductionType = "Manufacturing";

                await _cbeContext.ProductionCapacities.AddAsync(production);

                var pceCaseAssignment = new PCECaseAssignment
                {
                    ProductionCapacityId = production.Id,
                    UserId = UserId,
                    AssignmentDate = DateTime.Now,
                    CompletionDate = null,
                    Status = "New"
                };
                await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);         
            
                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = production.PCECaseId,
                    Activity = $" <strong>A new Manufacturing PCE has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>Manufacturing Main Sector:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>Manufacturing Sub Sector:</i> {production.Type}.",

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
                var production = await _cbeContext.ProductionCapacities.Where(c => c.Id == id && c.CreatedById == UserId && c.CurrentStage == "Relation Manager" && c.CurrentStatus == "New").FirstOrDefaultAsync();
                if (production == null)
                {
                    return false;
                }

                var deleteassignment = await _cbeContext.PCECaseAssignments.Where(c=>c.ProductionCapacityId == id && c.UserId == UserId && c.Status == "New").ToListAsync();
                if(deleteassignment.Any())
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
                if( Fileproduction != null)
                {
                    _cbeContext.Remove(Fileproduction);
                }
                
                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = production.PCECaseId,
                    Activity = $" <strong>A PCE Manufacturing with <class='text-purple'>Id: {production.Id} has been deleted. </strong>.",
                    CurrentStage = "Relation Manager"
                });

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

        public async Task<ProductionCapacity> EditProduction(Guid UserId, Guid ProductionCapacityId, ProductionPostDto createProductionDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                var Production = await _cbeContext.ProductionCapacities.FindAsync(ProductionCapacityId);
                if (Production == null)
                {
                    throw new Exception("PCE not Found");
                }
                if (Production.CreatedById != UserId)
                {
                    throw new Exception("you don't have permission");
                }
                if (Production.CurrentStage != "Relation Manager")
                {
                 throw new Exception("unable to Edit PCE");
                }                
                createProductionDto.PCECaseId = Production.PCECaseId;
                createProductionDto.ProductionType = Production.ProductionType;                
                _mapper.Map(createProductionDto, Production);
                
                if (createProductionDto.MachineryInstalledPlace == "Private Owned LHC")
                {
                    Production.Industrialpark = null;                     
                }
                else if(createProductionDto.MachineryInstalledPlace == "Industrial Park")
                {
                    Production.LHCNumber = null;
                    Production.OwnerName = null;
                }
               
                _cbeContext.ProductionCapacities.Update(Production);

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = Production.PCECaseId,
                    Activity = $" <strong>A PCE Manufacturing with <class='text-purple'>Id: {Production.Id} has been Updated. </strong>.",


                    CurrentStage = "Relation Manager"
                });

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Production;
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
                    Catagory = Category
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
              
        public async Task<bool> UploadProductionFile(Guid UserId, IFormFile file, Guid caseId, string DocumentCatagory)
        {
            var production = await _cbeContext.ProductionCapacities.FindAsync(caseId); //        .FirstOrDefaultAsync(pc => pc.PCECaseId == caseId);
            if (production == null)
            {
                return false;
            }

            var productionFile = new CreateFileDto()
            {
                File = file ?? throw new ArgumentNullException(nameof(file)),
                CollateralId = caseId,
                Catagory = DocumentCatagory,
                CaseId =production.PCECaseId,

            };
            if (await _UploadFileService.CreateUploadFile(UserId, productionFile) != Guid.Empty)
            {
                return true;
            }
            return false;
        }

        ///////////////////////// Plant ////////////////
       
        public async Task<ProductionCapacity> CreatePlantProduction(Guid UserId, Guid PCECaseId, PlantPostDto createProductionDto)
        {
            
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                var production = _mapper.Map<ProductionCapacity>(createProductionDto);
                production.Id = Guid.NewGuid();
                var produId= production.Id;
                try
                {
                    await this.UploadFile(UserId, "Plant LHC Certificate", production, createProductionDto.LHC);
                    await this.UploadFile(UserId, "Plant Commercial Invoice", production, createProductionDto.CommercialInvoice);
                    await this.UploadFile(UserId, "Plant Custom Declaration", production, createProductionDto.customDeclaration);
                    await this.UploadFile(UserId, "Plant Bussiness Licence", production, createProductionDto.BussinessLicence);
                    if (createProductionDto.CBEPartialFinancing != null)
                    {
                        await this.UploadFile(UserId, "CBE Partial Financing Supportive Document", production, createProductionDto.CBEPartialFinancing);
                    }

                    if (createProductionDto.OtherDocument != null)
                    {
                        foreach (var otherDocument in createProductionDto.OtherDocument)
                        {
                            await this.UploadFile(UserId, "Plant Other Supportive Document", production, otherDocument);
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("unable to upload file");
                }

                production.CreationDate = DateTime.Now;
                production.CreatedById = UserId;
                production.CurrentStage = "Relation Manager";
                production.CurrentStatus = "New";
                //production.ProductionType = "Plant";
                await _cbeContext.ProductionCapacities.AddAsync(production);

                // Create sample PCECaseAssignment instances
                var pceCaseAssignment = new PCECaseAssignment
                {
                    ProductionCapacityId = production.Id,
                    UserId = UserId,
                    AssignmentDate = DateTime.Now,
                    CompletionDate = null,
                    Status = "New"
                };
                await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);

                await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = production.PCECaseId,
                    Activity = $" <strong>A new Plant PCE has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}. &nbsp; <i class='text-purple'>Plant Type</i>.{production.PlantName}",

                    CurrentStage = "Relation Manager"
                });

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return production;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating plant");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating plant.");
            } 
        }
        public async Task<ProductionCapacity> EditPlantProduction(Guid UserId, Guid ProductionCapacityId, PlantEditPostDto createProductionDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                var Production = await _cbeContext.ProductionCapacities.FindAsync(ProductionCapacityId);
                if (Production == null)
                {
                    throw new Exception("PCE not Found");
                }
                if (Production.CreatedById != UserId)
                {
                    throw new Exception("you don't have permission");
                }
                if (Production.CurrentStage != "Relation Manager")
                {
                    throw new Exception("unable to Edit PCE");
                }
                createProductionDto.PCECaseId = Production.PCECaseId;
                createProductionDto.ProductionType = Production.ProductionType;
                _mapper.Map(createProductionDto, Production);
                _cbeContext.ProductionCapacities.Update(Production);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Production;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plant");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating the plant.");
            } 
        }

        public async Task<PlantEditPostDto> GetPlantProduction(Guid UserId, Guid id)
        {
            var product = await _cbeContext.ProductionCapacities
                            .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<PlantEditPostDto>(product);
        }  

        ///////////////////////// End Plant ////////////////

        public async Task<ReturnProductionDto> GetProduction(Guid UserId, Guid Id)
        {
            var production = await _cbeContext.ProductionCapacities.Include(pc => pc.PCECase).FirstOrDefaultAsync(pc => pc.Id == Id);
            return _mapper.Map<ReturnProductionDto>(production);
        }  

        public async Task<PCEDetailDto> GetPCEDetails(Guid UserId, Guid PCEId)
        {

            var pce = await _cbeContext.ProductionCapacities.AsNoTracking().Include(pc => pc.PCECase).FirstOrDefaultAsync(res => res.Id == PCEId);
            var pceAssignment = await _cbeContext.PCECaseAssignments.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId && res.UserId == UserId);
            var reestimation = await _cbeContext.ProductionReestimations.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId); 
            var rejectedProduction = await _cbeContext.ProductionRejects.AsNoTracking().FirstOrDefaultAsync(res => res.PCEId == PCEId);
            var relatedFiles = await _UploadFileService.GetUploadFileByCollateralId(PCEId);          
            var valuationHistory = await _PCEEvaluationService.GetValuationHistory(UserId, PCEId);
 
            var assignment_Status = pceAssignment.Status; 
            CreateUser user = null; 

            if (rejectedProduction != null)
            {
                user = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == rejectedProduction.RejectedBy);
            }

            return new PCEDetailDto
            {
                PCECase = pce.PCECase,
                ProductionCapacity = _mapper.Map<ReturnProductionDto>(pce),
                PCEValuationHistory = valuationHistory,
                Reestimation = reestimation,
                RelatedFiles = relatedFiles,
                RejectedProduction = rejectedProduction,
                RejectedBy = user,
                Assignment_Status = assignment_Status

            };
        }                

        public async Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            // var query = _cbeContext.ProductionCapacities.AsNoTracking()
            //                                             .Where(pc => pc.PCECaseAssignments
            //                                             .Any(pca => pca.UserId == UserId || pc.EvaluatorUserID == UserId));

            var query = _cbeContext.ProductionCapacities
                                    .AsNoTracking()
                                    .Include(pc => pc.PCECase)
                                    .Join(
                                        _cbeContext.PCECaseAssignments,
                                        pc => pc.Id,
                                        pca => pca.ProductionCapacityId,
                                        (pc, pca) => new { ProductionCapacity = pc, PCECaseAssignment = pca }
                                        )
                                        .Where(x => (x.PCECaseAssignment.UserId == UserId || x.ProductionCapacity.EvaluatorUserID == UserId)
                                                && (Status == null || Status == "All" || x.PCECaseAssignment.Status == Status))
                                    .Select(x => x.ProductionCapacity); 

            if (PCECaseId.HasValue)
            {
                query = query.Where(pc => pc.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(pc => pc.CurrentStage == Stage);
            }

            // if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            // {
            //     query = query.Where(pc => pc.CurrentStatus == Status);
            // }
            // else
            // {
            //     query = query.Where(pc => pc.CurrentStatus != "Rejected");
            // }

            var productions = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        // public async Task<int> GetProductionsCount(Guid UserId, Guid? PCECaseId, string Stage = null, string Status = null)
        // {
        //     return (await GetProductions(UserId, PCECaseId, Stage, Status)).Count();
        // }

        public async Task<int> GetProductionsCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            var query = _cbeContext.ProductionCapacities
                                    .AsNoTracking()
                                    .Join(
                                        _cbeContext.PCECaseAssignments,
                                        pc => pc.Id,
                                        pca => pca.ProductionCapacityId,
                                        (pc, pca) => new { ProductionCapacity = pc, PCECaseAssignment = pca }
                                    )
                                    .Where(x => (x.PCECaseAssignment.UserId == UserId || x.ProductionCapacity.EvaluatorUserID == UserId)
                                            && (Status == null || Status == "All" || x.PCECaseAssignment.Status == Status))
                                    .Select(x => x.ProductionCapacity); 

            if (PCECaseId.HasValue)
            {
                query = query.Where(x => x.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(x => x.CurrentStage == Stage);
            }

            // if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            // {
            //     query = query.Where(x => x.CurrentStatus == Status);
            // }
            // else
            // {
            //     query = query.Where(pc => pc.CurrentStatus != "Rejected");
            // }

            return await query.CountAsync();
        }

        public async Task<ProductionsCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null)
        {
            var Statuses = new[] { "New", "Pending", "Completed", "Reestimate", "Reestimated", "All" };
            var tasks = Statuses.Select(Status => GetProductionsCountAsync(UserId, PCECaseId, Stage, Status)).ToList();

            var counts = await Task.WhenAll(tasks);

            return new ProductionsCountDto()
            {
                NewProductionsCount = counts[0],
                PendingProductionsCount = counts[1],
                CompletedProductionsCount = counts[2],
                ResubmittedProductionsCount = counts[3], // rejected 
                ReestimatedProductionsCount = counts[4],
                TotalProductionsCount = counts[5]
                // TotalProductionsCount = await GetProductionsCountAsync(UserId, PCECaseId, Stage)
            };
        }
    
        // public async Task<IEnumerable<ReturnProductionDto>> GetReturnedProductions(Guid UserId)
        // {
       
        //     var rejectedCapacitiesQuery = from reject in _cbeContext.ProductionRejects
        //                                   join capacity in _cbeContext.ProductionCapacities
        //                                   on reject.PCEId equals capacity.Id
        //                                   // .AsNoTracking()
        //                                   where reject.RejectedBy == UserId
        //                                   select capacity;

        //     var rejectedCapacities = await rejectedCapacitiesQuery.ToListAsync();


        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(rejectedCapacities); ;
        // }
        
        public async Task<IEnumerable<ReturnProductionDto>> GetRemarkProductions(Guid UserId, Guid PCECaseId)
        {
            var pceCaseAssignments = await _cbeContext.PCECaseAssignments
                                                            .Include(res => res.ProductionCapacity)
                                                            .Where(res => res.UserId == UserId 
                                                                        && res.ProductionCapacity.PCECaseId == PCECaseId 
                                                                        && res.Status.Contains("Remark"))
                                                            .ToListAsync();
            var productions = pceCaseAssignments.Select(res => res.ProductionCapacity);
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }     
        
        public async Task<IEnumerable<ProductionAssignmentDto>> GetAssignedProductions(Guid UserId, Guid PCECaseId)
        {
            var supervisedUsers = await _cbeContext.CreateUsers.Where(user => user.SupervisorId == UserId).Select(user => user.Id).ToListAsync();

            var productionAssignments = await _cbeContext.PCECaseAssignments
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

        // public async Task<IEnumerable<ProductionCapacityCorrectionReturnDto>> GetComments(Guid ProductionCapacityId)
        // {
        //     var comments = await _cbeContext.ProductionCapacityCorrections.Where(ca => ca.ProductionCapacityId == ProductionCapacityId).ToListAsync();
            
        //     if (comments != null)
        //     {
        //         return _mapper.Map<IEnumerable<ProductionCapacityCorrectionReturnDto>>(comments);
        //     }

        //     return null;
        // }

        // public async Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid PCECaseId)
        // {
        //     var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager")).ToListAsync();
        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        // }

        // public async Task<IEnumerable<ReturnProductionDto>> GetPendingProductions(Guid PCECaseId)
        // {
        //     var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == "New" && res.CurrentStage != "Relation Manager")).ToListAsync();
        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        // }
        // public async Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid PCECaseId)
        // {
        //     var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == "Reject" && res.CurrentStage == "Relation Manager")).ToListAsync();
        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        // }


        // public async Task<IEnumerable<ReturnProductionDto>> GetRmRejectedProductions(Guid UserId, Guid PCECaseId)
        // {
        //     var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && res.CurrentStatus == "Rejected" && res.CurrentStage == "Relation Manager").ToListAsync();
        //     var productionDtos = _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        //     return productionDtos;
        // }


        // public async Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid PCECaseId)
        // {
        //     var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStage != "Relation Manager" && res.CurrentStatus != "Completed")).ToListAsync();
        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        // }

        // public async Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid PCECaseId)
        // {
        //     var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && res.CurrentStage == "Relation Manager" && res.CurrentStatus == "Completed").ToListAsync();
        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        // }

        // public async Task<ReturnProductionDto> GetManufuctringProductionCapacityEvalutionById(Guid productionId)       {
 
        //     var productionById = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(res => res.Id == productionId);
        //     return _mapper.Map<ReturnProductionDto> (productionById);

        // }
        // public async Task<PlantEditPostDto> GetPlantProductionCapacityEvalutionById(Guid productionId)
        // {

        //     var productionById = await _cbeContext.PCEEvaluations.Include(res => res.PCE).FirstOrDefaultAsync(res => res.PCEId == productionId);
        //     return _mapper.Map<PlantEditPostDto>(productionById);

        // }
        // public async Task<PCEEvaluationReturnDto> GetValuationById(Guid productionId)
        // {

        //     var productionById = await _cbeContext.PCEEvaluations.Include(res => res.PCE).FirstOrDefaultAsync(res => res.PCEId == productionId);
        //     return _mapper.Map<PCEEvaluationReturnDto>(productionById);

        // }
    }
}

