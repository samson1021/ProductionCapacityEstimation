using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.ProductionCaseDto;
using mechanical.Models.Dto.ProductionCaseTimeLineDto;
using mechanical.Models.Dto.ProductionUploadFileDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.ProductionUploadFileService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.ProductionCaseService
{
    public class ProductionCaseService : IProductionCaseService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IProductionUploadFileService _productionUploadFileService;
        public ProductionCaseService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICaseTimeLineService caseTimeLineService, IProductionUploadFileService productionUploadFileService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _productionUploadFileService = productionUploadFileService;
            
        }

       
        public async Task<RetunProductionCaseDto> GetProductionCase(Guid userId, Guid id)
        {
            var loanCase = await _cbeContext.ProductionCases
                           .Include(res => res.ProductionBussinessLicence).Include(res => res.District).Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == id && c.CaseOriginatorId == userId);
            return _mapper.Map<RetunProductionCaseDto>(loanCase);
        }

        public async Task<RetunProductionCaseDto> GetProductionCaseDetail(Guid id)
        {
            var loanCase = await _cbeContext.ProductionCases
                           .Include(res => res.ProductionBussinessLicence).Include(res => res.District).Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<RetunProductionCaseDto>(loanCase);
        }

       

        public async Task<IEnumerable<ProductionCaseDto>> GetNewProductionCases(Guid userId)
        {
            var pc = await _cbeContext.ProductionCases.Include(x => x.ProductionCapacities.Where(res => res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager"))
                       .Where(res => res.CaseOriginatorId == userId && res.Status == "New")
                       .ToListAsync();
            var pcdtos = _mapper.Map<IEnumerable<ProductionCaseDto>>(pc);
            foreach (var pcdto in pcdtos)
            {
                pcdto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.ProductionCaseId == pcdto.Id);
            }
            return pcdtos;
        }


        private async Task UploadFile(Guid userId, string Category, ProductionCapacity productionCapacity, IFormFile? file)
        {
            if (file != null)
            {
                await _productionUploadFileService.CreateProductionUploadFile(userId, new CreateProductionFileDto()
                {
                    File = file,
                    ProductionCaseId = productionCapacity.ProductionCaseId,
                    ProductionCapacityId = productionCapacity.Id,
                    Catagory = Category
                });
            }
        }

        //public Task<Case> CreateCase(Guid userId, ProductionCasePostDto createCaseDto)
        //{
        //    throw new NotImplementedException();
        //}

        

        public async Task<ProductionCase> CreateProductionCase(Guid userId, ProductionCasePostDto createCaseDto)
        {
            var user = _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefault(res => res.Id == userId);
            if (user == null)
            {
                throw new Exception("user not found");
            }
            var loanCase = _mapper.Map<ProductionCase>(createCaseDto);
            loanCase.Id = Guid.NewGuid();
            if (createCaseDto.ProductionBussinessLicence != null)
            {
                var BussinessLicence = new CreateProductionFileDto()
                {
                    File = createCaseDto.ProductionBussinessLicence ?? throw new ArgumentNullException(nameof(createCaseDto.ProductionBussinessLicence)),
                    ProductionCaseId = loanCase.Id,
                    Catagory = " Production Bussiness Licence"
                };
                loanCase.ProductionBussinessLicenceId = await _productionUploadFileService.CreateProductionUploadFile(userId, BussinessLicence);
            }
            loanCase.CaseOriginatorId = userId;
            loanCase.CreationAt = DateTime.Now;
            loanCase.DistrictId = user.DistrictId;
            loanCase.Status = "New";
            await _cbeContext.ProductionCases.AddAsync(loanCase);
            await _cbeContext.SaveChangesAsync();

            //await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            //{
            //    CaseId = loanCase.Id,
            //    Activity = $"<strong>A new case with ID {loanCase.CaseNo} has been created</strong>",
            //    CurrentStage = user.Role.Name
            //});

            return loanCase;
        }

        public async Task<IEnumerable<ProductionCaseDto>> GetProductionRmLatestCases(Guid userId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var cases = await _cbeContext.ProductionCases
                    .Include(x => x.ProductionCapacities)
                    .Include(x => x.District)
                    .Where(res => res.CaseOriginatorId == userId)
                    .OrderByDescending(res => res.CreationAt).Take(7).ToListAsync();
            return _mapper.Map<IEnumerable<ProductionCaseDto>>(cases);
        }

        public async Task<IEnumerable<ProductionCaseDto>> GetProductionMmLatestCases(Guid userId)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var NewCollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.ProductionCase).ThenInclude(res => res.CaseOriginator).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            var cases = NewCollateral.Select(res => res.ProductionCapacity.ProductionCase).Distinct().OrderByDescending(res => res.CreationAt).Take(7);
            return _mapper.Map<IEnumerable<ProductionCaseDto>>(cases);
        }

        public async Task<ProductionCaseTerminate> ApproveProductionCaseTermination(Guid id)
        {
            var caseTerminate = await _cbeContext.ProductionCaseTerminates.FindAsync(id);
            if (caseTerminate == null)
            {
                throw new Exception("Production Case Schedule not Found");
            }
            caseTerminate.Status = "Approved";
            _cbeContext.Update(caseTerminate);

            var cases = await _cbeContext.ProductionCases.FindAsync(caseTerminate.ProductionCaseId);
            cases.Status = "Terminate";
            var collaterals = await _cbeContext.ProductionCapacities.Where(res => res.ProductionCaseId == caseTerminate.ProductionCaseId).ToListAsync();

            foreach (var collateral in collaterals)
            {
                collateral.CurrentStage = "Relation Manager";
                collateral.CurrentStatus = "Terminate";
                var caseAssignments = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == collateral.Id).ToListAsync();
                foreach (var caseAssignment in caseAssignments)
                {
                    caseAssignment.Status = "Terminate";

                }
                _cbeContext.ProductionCaseAssignments.UpdateRange(caseAssignments);
            }

            _cbeContext.ProductionCapacities.UpdateRange(collaterals);

            await _cbeContext.SaveChangesAsync();
            return caseTerminate;
        }

           public async Task<bool> DeleteProuctionBussinessLicence(Guid Id)
        {
            var cases = await _cbeContext.ProductionCases.FindAsync(Id);
            if (cases == null)
            {
                return false;
            }
            if (cases.ProductionBussinessLicenceId != null && await _productionUploadFileService.DeleteProductionFile(cases.ProductionBussinessLicenceId.Value))
            {
                cases.ProductionBussinessLicenceId = null;
                _cbeContext.Update(cases);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> UploadProductionBussinessLicence(Guid userId, IFormFile file, Guid caseId)
        {
            var cases = await _cbeContext.ProductionCases.FindAsync(caseId);
            if (cases == null)
            {
                return false;
            }
            var BussinessLicence = new CreateProductionFileDto()
            {
                File = file ?? throw new ArgumentNullException(nameof(file)),
                ProductionCaseId = caseId,
                Catagory = "PCE Bussiness Licence"
            };
            cases.ProductionBussinessLicenceId = await _productionUploadFileService.CreateProductionUploadFile(userId, BussinessLicence);
            _cbeContext.Update(cases);
            await _cbeContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProductionCase> EditProductionCase(Guid userId, Guid id, ProductionCasePostDto createCaseDto)
        {
            var loanCase = await _cbeContext.ProductionCases
                .Include(res => res.ProductionBussinessLicence)
                .FirstOrDefaultAsync(c => c.Id == id && c.CaseOriginatorId == userId);
            if (loanCase != null)
            {
                _mapper.Map(createCaseDto, loanCase);
                _cbeContext.Update(loanCase);
                await _cbeContext.SaveChangesAsync();
                return loanCase;
            }
            throw new Exception("case with this Id is not found");
        }
    }
}
