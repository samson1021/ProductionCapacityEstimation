using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;
using mechanical.Models.Dto.ProductionCapacityDto;
using Microsoft.CodeAnalysis.Operations;
using mechanical.Services.ProductionCaseTimeLineService;
using mechanical.Models.Dto.ProductionCaseTimeLineDto;
using System.ComponentModel.DataAnnotations;
using mechanical.Models.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.ProductionUploadFileService;
using mechanical.Models.Dto.ProductionUploadFileDto;

namespace mechanical.Services.ProductionCapacityServices
{
    public class ProductionCapacityServices : IProductionCapacityServices
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IProductionUploadFileService _productionUploadFileService;
        private readonly IProductionCaseTimeLineService _productionCaseTimeLineService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductionCapacityServices(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IProductionUploadFileService productionUploadFileService, IProductionCaseTimeLineService productionCaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _productionUploadFileService = productionUploadFileService;
           _productionCaseTimeLineService = productionCaseTimeLineService;
            _httpContextAccessor = httpContextAccessor;

        }
        public async Task<ProductionCapacity> CreateProductionCapacity(Guid userId, Guid productionCaseId, ProductionPostDto createProductionDto)
        {
            var production = _mapper.Map<ProductionCapacity>(createProductionDto);
            production.Id = Guid.NewGuid();
            production.ProductionCaseId = productionCaseId;
           try
            {
                await this.ProductionUploadFile(userId, "PCE Owner LHC Certificate", production, createProductionDto.UploadLHC);
                await this.ProductionUploadFile(userId, "PCE Shade Rent Agreement", production, createProductionDto.ploadshaderentagreement);
                await this.ProductionUploadFile(userId, "PCE Business license", production, createProductionDto.Uploadbusinesslicense);
                await this.ProductionUploadFile(userId, "PCE Machine specification document", production, createProductionDto.Machinespecificationdocumen);
                await this.ProductionUploadFile(userId, "PCE Machine operation manual", production, createProductionDto.Machineoperationmanual);
                if (createProductionDto.OtherDocument != null)
                {
                    foreach (var otherDocument in createProductionDto.OtherDocument)
                    {
                        await this.ProductionUploadFile(userId, "PCE Other Supportive Document", production, otherDocument);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("unable to upload file");
            }

            production.CreationDate = DateTime.Now;
            production.CreatedById = userId;
            production.CurrentStage = "Relation Manager";
            production.CurrentStatus = "New";
            production.ProductionType = "Manufacturing";


            await _cbeContext.ProductionCapacities.AddAsync(production);
            await _cbeContext.SaveChangesAsync();
           
            await _productionCaseTimeLineService.CreateProductionCaseTimeLine(new ProductionCaseTimeLinePostDto
            {
                ProductionCaseId = production.ProductionCaseId,
                //Activity = $" <strong>A new collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {production.Type}.",
                Activity = $" <strong>A new PCE has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {production.Type}.",

                CurrentStage = "Relation Manager"
            });

            return production;
        }
     

        public async Task<bool> DeleteProduction(Guid userId, Guid id)
        {
            var production = await _cbeContext.ProductionCapacities.Where(c => c.Id == id && c.CreatedById == userId && c.CurrentStage == "Relation Manager").FirstOrDefaultAsync();
            if (production != null)
            {
                _cbeContext.Remove(production);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

    

    public async Task<ProductionCapacity> EditProduction(Guid userId, Guid ProductionCapacityId, ProductionPostDto createProductionDto)
        {
            var Production = await _cbeContext.ProductionCapacities.FindAsync(ProductionCapacityId);
            if (Production == null)
            {
                throw new Exception("PCE not Found");
            }
            if (Production.CreatedById != userId)
            {
                throw new Exception("you don't have permission");
            }
            if (Production.CurrentStage == "Relation Manager")
            {
                createProductionDto.ProductionCaseId = Production.ProductionCaseId;
                createProductionDto.ProductionType = Production.ProductionType;
                _mapper.Map(createProductionDto, Production);
                _cbeContext.ProductionCapacities.Update(Production);
                await _cbeContext.SaveChangesAsync();
                return Production;
            }
            throw new Exception("unable to Edit PCE");
        }



    public async Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid ProductionCaseId)
    {
          //  var ProductinCaseId = Guid.Parse("2cd32d1a-89bb-42c6-8a1d-7c631558ba47");
        var productions = await _cbeContext.ProductionCapacities.Where(res => res.ProductionCaseId == ProductionCaseId && (res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager")).ToListAsync();
        return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
    }

    public async Task<ReturnProductionDto> GetProduction(Guid userId, Guid id)
    {
        var product = await _cbeContext.ProductionCapacities
                        .FirstOrDefaultAsync(c => c.Id == id);
        return _mapper.Map<ReturnProductionDto>(product);
    }

        public async Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid ProductionCaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.ProductionCaseId == ProductionCaseId && (res.CurrentStatus == "Reject" && res.CurrentStage == "Relation Manager")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }




        public async Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid ProductionCaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.ProductionCaseId == ProductionCaseId && (res.CurrentStage != "Relation Manager" && res.CurrentStatus != "Complete")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async  Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid ProductionCaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.ProductionCaseId == ProductionCaseId && res.CurrentStage == "Checker officer" && res.CurrentStatus == "Complete").ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async Task<IEnumerable<ProductionAssignmentDto>> GetMyAssignmentProductions(Guid UserId, Guid ProductionCaseId)
        {
            var userSupervised = await _cbeContext.CreateUsers.Where(res => res.SupervisorId == UserId).ToListAsync();
            var ProductionAssigmentDtos = new List<ProductionAssignmentDto>();

            foreach (var item in userSupervised)
            {
                var productionCaseAssignment = await _cbeContext.ProductionCaseAssignments.Include(x => x.User).Include(x => x.ProductionCapacity).Where(res => res.UserId == item.Id && res.ProductionCapacity.ProductionCaseId == ProductionCaseId).ToListAsync();
                productionCaseAssignment = productionCaseAssignment.DistinctBy(res => res.ProductionCapacityId).ToList();
                foreach (var items in productionCaseAssignment)
                {
                    var productionAssigmentDto = new ProductionAssignmentDto
                    {
                        ProductionCapacityId = items.ProductionCapacityId,
                        ProductionCaseId = ProductionCaseId,
                        PropertyOwner = items.ProductionCapacity.PropertyOwner,
                        ProductionCaseAssignmentId = items.Id,
                        Role = items.ProductionCapacity.Role,
                        Type = EnumToDisplayName(items.ProductionCapacity.Type),
                        Category = EnumToDisplayName(items.ProductionCapacity.Category),
                        User = items.User.Name,
                        AssignmentDate = items.AssignmentDate,
                        Status = items.Status,
                    };
                    ProductionAssigmentDtos.Add(productionAssigmentDto);
                }
            }
            return ProductionAssigmentDtos;
        }
        string EnumToDisplayName<TEnum>(TEnum enumValue)
        {
            return (typeof(TEnum).GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute)?.Name ?? enumValue.ToString();
        }




        public async Task<IEnumerable<ReturnProductionDto>> MyReturnedProductions(Guid userId)
        {
            List<ProductionCaseAssignment> caseAssignments = await _cbeContext.ProductionCaseAssignments.Where(ca => ca.UserId == userId && ca.Status == "Correction").ToListAsync();
            List<ProductionCapacity> collaterals = await _cbeContext.ProductionCapacities.Where(ca => ca.CurrentStage == "Maker Officer" && ca.CurrentStatus == "Correction").ToListAsync();

            List<ReturnCollateralDto> mTLreturnCollateralDtos = new List<ReturnCollateralDto>();
            if (collaterals != null)
            {
                foreach (var caseAssignment in caseAssignments)
                {
                    var collatearal = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(ca => ca.Id == caseAssignment.ProductionCapacityId && ca.CurrentStatus == "Correction");
                    if (collatearal != null)
                    {
                        mTLreturnCollateralDtos.Add(_mapper.Map<ReturnCollateralDto>(collatearal));
                    }
                }
            }
            return _mapper.Map<List<ReturnProductionDto>>(collaterals);

        }

        public async Task<IEnumerable<ProductionCapcityCorrectionReturnDto>> GetComments(Guid CollateralId)
        {
            var comments = await _cbeContext.ProductionCapcityCorrections.Where(ca => ca.ProductionCapacityId == CollateralId).ToListAsync();
            if (comments != null)
            {
                return _mapper.Map<IEnumerable<ProductionCapcityCorrectionReturnDto>>(comments);
            }


            return null;
        }

        public async Task<IEnumerable<ReturnProductionDto>> GetRemarkProducts(Guid userId, Guid ProductionCaseId)
        {
            var productioncaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.ProductionCapacity.ProductionCaseId == ProductionCaseId && res.Status.Contains("Remark")).ToListAsync();
            var productions = productioncaseAssignments.Select(res => res.ProductionCapacity);
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async  Task<ReturnProductionDto> GetProductionCapacityById(Guid productionId)
        {
            var productionById = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(res => res.Id == productionId);
            return _mapper.Map<ReturnProductionDto>(productionById);

        }

        private async Task ProductionUploadFile(Guid userId, string Category, ProductionCapacity production, IFormFile? file)
        {
            if (file != null)
            {
                await _productionUploadFileService.CreateProductionUploadFile(userId, new CreateProductionFileDto()
                {
                    File = file,
                    ProductionCaseId = production.ProductionCaseId,
                    ProductionCapacityId = production.Id,
                    Catagory = Category
                });
            }
        }


        public async Task<bool> DeleteProductionFile(Guid userId, Guid Id)
        {
            var file = await _cbeContext.ProductionUploadFiles.FindAsync(Id);
            if (file == null)
            {
                return false;
            }
            if (file.userId != userId)
            {
                return false;
            }
            if (await _productionUploadFileService.DeleteProductionFile(file.Id))
            {
                return true;
            }
            return false;
        }
        

        public async Task<bool> UploadProductionFile(Guid userId, IFormFile file, Guid ProductionCaseId, string DocumentCatagory)
        {
            var production = await _cbeContext.ProductionCapacities.FindAsync(ProductionCaseId);
            if (production == null)
            {
                return false;
            }

            var ProductionFile = new CreateProductionFileDto()
            {
                File = file ?? throw new ArgumentNullException(nameof(file)),
                ProductionCapacityId = ProductionCaseId,
                Catagory = DocumentCatagory,

            };
            if (await _productionUploadFileService.CreateProductionUploadFile(userId, ProductionFile) != Guid.Empty)
            {
                return true;
            }
            return false;
        }
    }
    
}
    
