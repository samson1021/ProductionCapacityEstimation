using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.PCECase;
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;
using mechanical.Models.PCE.Entities;
using Microsoft.CodeAnalysis.Operations;
using AutoMapper;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.PCE.UploadFileService;
using mechanical.Models.PCE.Dto.PCEUploadFileDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Services.PCE.PCECaseTimeLineService;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;

namespace mechanical.Services.PCE.PCECollateralService
{
    public class PCECollateralService : IPCECollateralService
    {
        private readonly IMapper _mapper;
        private readonly IPCEUploadFileService _uploadFileService;
        private readonly CbeContext _cbeContext;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;


        public PCECollateralService(IMapper mapper, IPCEUploadFileService pCEUploadFileService, CbeContext cbeContext, IPCECaseTimeLineService iPCECaseTimeLineService)
        {
            _mapper = mapper;
            _uploadFileService = pCEUploadFileService;
            _cbeContext = cbeContext;
            _IPCECaseTimeLineService = iPCECaseTimeLineService;
        }

        public async Task<ProductionCapacity> CreatePCECollateral(PlantCapacityEstimationPostDto plantCapacityEstimationPostDto)
        {
            var collaterals = _mapper.Map<ProductionCapacity>(plantCapacityEstimationPostDto);
            collaterals.Id = Guid.NewGuid();

            //try
            //{
            //    await UploadFile(plantCapacityEstimationPostDto.CreatedById, "Commercial Invoice", collaterals, plantCapacityEstimationPostDto.CommercialInvoice);
            //    await UploadFile(plantCapacityEstimationPostDto.CreatedById, "Custom Declaration", collaterals, plantCapacityEstimationPostDto.customDeclaration);
            //    await UploadFile(plantCapacityEstimationPostDto.CreatedById, "Bussiness Licence", collaterals, plantCapacityEstimationPostDto.BussinessLicence);
            //    await UploadFile(plantCapacityEstimationPostDto.CreatedById, "Land Holding Certificate", collaterals, plantCapacityEstimationPostDto.LHC);

            //    if (plantCapacityEstimationPostDto.OtherDocument != null)
            //    {
            //        foreach (var otherDocument in plantCapacityEstimationPostDto.OtherDocument)
            //        {
            //            await UploadFile(plantCapacityEstimationPostDto.CreatedById, "Other Supportive Document", collaterals, otherDocument);
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    throw new Exception("unable to upload file");
            //}

            collaterals.CreationDate = DateTime.Now;
            collaterals.CurrentStage = "Relation Manager";
            collaterals.CurrentStatus = "New";
            collaterals.ProductionType = "Plant";
            await _cbeContext.ProductionCapacities.AddAsync(collaterals);
            await _cbeContext.SaveChangesAsync();


            await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
            {
                CaseId = collaterals.PCECaseId,
                //Activity = $" <strong>A new PCE Plant collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {collaterals.OwnerOfPlant}. &nbsp; <i class='text-purple'>Role:</i> {collaterals.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collaterals.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collaterals.CollateralType)}.",
                Activity = $" <strong>A new PCE Plant collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {collaterals.OwnerName}. &nbsp;",
                CurrentStage = "Relation Manager"
            });

            return collaterals;
        }

        private async Task UploadFile(Guid userId, string category, ProductionCapacity PCEcollateral, IFormFile? file)
        {
            if (file != null)
            {
                await _uploadFileService.CreateUploadFile(userId, new PCECreateFileDto()
                {
                    File = file,
                    CaseId = PCEcollateral.PCECaseId,
                    PlantCapacityEstimationId = PCEcollateral.Id,
                    Catagory = category
                });
            }
        }

        public async Task<IEnumerable<PCEReturnCollateralDto>> GetCollaterals(Guid CaseId)
        {
            var collaterals = await _cbeContext.PlantCapacityEstimations.Where(res => res.CaseId == CaseId && (res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager")).ToListAsync();
            return _mapper.Map<IEnumerable<PCEReturnCollateralDto>>(collaterals);
        }

        public async Task<PCEReturnCollateralDto> GetCollateral(Guid id)
        {
            var collateral = await _cbeContext.ProductionCapacities
                           .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<PCEReturnCollateralDto>(collateral);
        }






        //Task<PlantCapacityEstimation> EditCollateral(Guid userId, Guid CollaterlId, PlantCapacityEstimationPostDto createCollateralDto);


        //public async Task<PlantCapacityEstimation> EditCollateral(Guid userId, Guid CollaterlId, PlantCapacityEstimationEditPostDto createCollateralDto)
        //{
        //    var collateral = await _cbeContext.PlantCapacityEstimations.FindAsync(CollaterlId);
        //    if (collateral == null)
        //    {
        //        throw new Exception("collateral not Found");
        //    }
        //    if (collateral.CreatedById != userId)
        //    {
        //        throw new Exception("you don't have permission");
        //    }
        //    if (collateral.CurrentStage == "Relation Manager")
        //    {
        //        createCollateralDto.CaseId = collateral.CaseId;
        //        createCollateralDto.CollateralType = collateral.CollateralType;
        //        _mapper.Map(createCollateralDto, collateral);
        //        _cbeContext.PlantCapacityEstimations.Update(collateral);
        //        await _cbeContext.SaveChangesAsync();
        //        return collateral;
        //    }
        //    throw new Exception("unable to Edit collateral");
        //}
        public async Task<ProductionCapacity> EditCollateral(Guid userId, Guid CollaterlId, PlantCapacityEstimationEditPostDto createCollateralDto)
        {
            var collateral = await _cbeContext.ProductionCapacities.FindAsync(CollaterlId);
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
                // Update only the modified properties
                createCollateralDto.CaseId = collateral.PCECaseId;
                createCollateralDto.CollateralType = collateral.ProductionType;
                _mapper.Map(createCollateralDto, collateral, typeof(PlantCapacityEstimationEditPostDto), typeof(ProductionCapacity));
                _cbeContext.ProductionCapacities.Update(collateral);
                await _cbeContext.SaveChangesAsync();
                return collateral;
            }

            throw new Exception("unable to Edit collateral");
        }


        public async Task<bool> DeleteCollateralFile(Guid userId, Guid Id)
        {
            var file = await _cbeContext.PCEUploadFiles.FindAsync(Id);
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

        public async Task<bool> UploadCollateralFile(Guid userId, IFormFile file, Guid pcecaseId, string DocumentCatagory, Guid caseId)
        {
            var collateral = await _cbeContext.ProductionCapacities.FindAsync(pcecaseId);
            if (collateral == null)
            {
                return false;
            }

            var CollateralFile = new PCECreateFileDto()
            {
                File = file ?? throw new ArgumentNullException(nameof(file)),
                PlantCapacityEstimationId = pcecaseId,
                Catagory = DocumentCatagory,
                CaseId = caseId

            };
            if (await _uploadFileService.CreateUploadFile(userId, CollateralFile) != Guid.Empty)
            {
                return true;
            }
            return false;
        }



        //public async Task<bool> DeleteCocllateral(Guid userId, Guid id)
        //{
        //    var collateral = await _cbeContext.PlantCapacityEstimations.Where(c => c.Id == id && c.CreatedById == userId && c.CurrentStage == "Relation Manager").FirstOrDefaultAsync();
        //    //if (collateral != null)
        //    //{
        //    //    _cbeContext.Remove(collateral);
        //    //    await _cbeContext.SaveChangesAsync();
        //    //    return true;
        //    //}

        //    if (collateral != null)
        //    {
        //        // Delete all associated PCEUploadFiles records
        //        var uploadFiles = await _cbeContext.PCEUploadFiles
        //            .Where(f => f.PlantCapacityEstimationId == id)
        //            .ToListAsync();

        //        _cbeContext.PCEUploadFiles.RemoveRange(uploadFiles);
        //        await _cbeContext.SaveChangesAsync();

        //        // Delete the PlantCapacityEstimations record
        //        _cbeContext.PlantCapacityEstimations.Remove(collateral);
        //        await _cbeContext.SaveChangesAsync();

        //        await transaction.CommitAsync();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}



        public async Task<bool> DeleteCocllateral(Guid userId, Guid id)
        {
            using (var transaction = await _cbeContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Find the PlantCapacityEstimations record
                    var collateral = await _cbeContext.PlantCapacityEstimations
                        .Where(c => c.Id == id && c.CreatedById == userId && c.CurrentStage == "Relation Manager")
                        .FirstOrDefaultAsync();

                    if (collateral != null)
                    {
                        // Delete all associated PCEUploadFiles records
                        var uploadFiles = await _cbeContext.PCEUploadFiles
                            .Where(f => f.PlantCapacityEstimationId == id)
                            .ToListAsync();

                        _cbeContext.PCEUploadFiles.RemoveRange(uploadFiles);
                        await _cbeContext.SaveChangesAsync();

                        // Delete the PlantCapacityEstimations record
                        _cbeContext.PlantCapacityEstimations.Remove(collateral);
                        await _cbeContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }









    }
}