﻿using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using Microsoft.CodeAnalysis.Operations;
//using mechanical.Services.ProductionCaseTimeLineService;
//using mechanical.Models.Dto.ProductionCaseTimeLineDto;
using System.ComponentModel.DataAnnotations;
using mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto;

using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;

using mechanical.Models.Dto.UploadFileDto;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;
using mechanical.Models.Dto.ConstMngAgrMachineryDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Services.PCE.ProductionCapacityServices
{
    public class ProductionCapacityServices : IProductionCapacityServices
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
       // private readonly IProductionUploadFileService _productionUploadFileService;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadFileService _uploadFileService;

        public ProductionCapacityServices(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
           // _productionUploadFileService = productionUploadFileService;
            _IPCECaseTimeLineService = IPCECaseTimeLineService;
            _httpContextAccessor = httpContextAccessor;
            _uploadFileService = uploadFileService;

        }
        public async Task<ProductionCapacity> CreateProductionCapacity(Guid UserId, Guid PCECaseId, ProductionPostDto createProductionDto)
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
            await _cbeContext.SaveChangesAsync();
           
            await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
            {
                CaseId = production.PCECaseId,
                //Activity = $" <strong>A new collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {production.Type}.",
                Activity = $" <strong>A new Manufacturing PCE has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>Manufacturing Main Sector:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>Manufacturing Sub Sector:</i> {production.Type}.",

                CurrentStage = "Relation Manager"
            });

            return production;
        }

       
        public async Task<ProductionCapacity> CreatePlantProduction(Guid UserId, Guid PCECaseId, PlantPostDto createProductionDto)
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
            await _cbeContext.SaveChangesAsync();



            // Create sample ProductionCaseAssignment instances
            var productionCaseAssignment1 = new ProductionCaseAssignment
            {
                Id = Guid.NewGuid(),
                ProductionCapacityId = production.Id,
                UserId = UserId,
                AssignmentDate = new DateTime(2023, 6, 1),
                CompletionDate = null,
                Status = "New"
            };
            await _cbeContext.ProductionCaseAssignments.AddAsync(productionCaseAssignment1);
            await _cbeContext.SaveChangesAsync();

            await _IPCECaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
            {
                CaseId = production.PCECaseId,
                //Activity = $" <strong>A new collateral has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {production.Type}.",
                Activity = $" <strong>A new Plant PCE has been added. </strong> <br> <i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}. &nbsp; <i class='text-purple'>Plant Type</i>.{production.PlantName}",

                CurrentStage = "Relation Manager"
            });

            return production;
        }


        public async Task<bool> DeleteProduction(Guid UserId, Guid id)
        {
            var production = await _cbeContext.ProductionCapacities.Where(c => c.Id == id && c.CreatedById == UserId && c.CurrentStage == "Relation Manager").FirstOrDefaultAsync();
            if (production != null)
            {

                //delete the assignment case if its new not starting the process or not completed yet
                var deleteassignment = await _cbeContext.ProductionCaseAssignments.Where(c=>c.ProductionCapacityId == id && c.UserId == UserId && c.Status == "New").ToListAsync();
                if(deleteassignment.Any())
                {
                    _cbeContext.RemoveRange(deleteassignment);
                    await _cbeContext.SaveChangesAsync();
                }

                //delete the files if they are available
                var deletedFiles = await _cbeContext.UploadFiles.Where(c => c.CollateralId == id && c.userId == UserId).ToListAsync();
                if (deletedFiles.Any())
                {
                    _cbeContext.UploadFiles.RemoveRange(deletedFiles);
                    await _cbeContext.SaveChangesAsync();
                }


                _cbeContext.Remove(production);
                await _cbeContext.SaveChangesAsync();
                var Filrproduction = await _cbeContext.UploadFiles.Where(c => c.CollateralId == id).FirstOrDefaultAsync();
                if( Filrproduction != null)
                {
                    _cbeContext.Remove(Filrproduction);
                    await _cbeContext.SaveChangesAsync();
                }
                return true;
            }
                return false;
        }

    

    public async Task<ProductionCapacity> EditProduction(Guid UserId, Guid ProductionCapacityId, ProductionPostDto createProductionDto)
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
            if (Production.CurrentStage == "Relation Manager")
            {
                createProductionDto.PCECaseId = Production.PCECaseId;
                createProductionDto.ProductionType = Production.ProductionType;
                _mapper.Map(createProductionDto, Production);
                _cbeContext.ProductionCapacities.Update(Production);
                await _cbeContext.SaveChangesAsync();
                return Production;
            }
            throw new Exception("unable to Edit PCE");
        }




        public async Task<ProductionCapacity> EditPlantProduction(Guid UserId, Guid ProductionCapacityId, PlantEditPostDto createProductionDto)
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
            if (Production.CurrentStage == "Relation Manager")
            {
                createProductionDto.PCECaseId = Production.PCECaseId;
                createProductionDto.ProductionType = Production.ProductionType;
                _mapper.Map(createProductionDto, Production);
                _cbeContext.ProductionCapacities.Update(Production);
                await _cbeContext.SaveChangesAsync();
                return Production;
            }
            throw new Exception("unable to Edit PCE");
        }








        public async Task<IEnumerable<ReturnProductionDto>> GetProductions(Guid PCECaseId)
    {
          //  var ProductinCaseId = Guid.Parse("2cd32d1a-89bb-42c6-8a1d-7c631558ba47");
        var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == "New" && res.CurrentStage == "Relation Manager")).ToListAsync();
        return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
    }

    public async Task<IEnumerable<ReturnProductionDto>> GetPendingProductions(Guid PCECaseId)
    {
        //  var ProductinCaseId = Guid.Parse("2cd32d1a-89bb-42c6-8a1d-7c631558ba47");
        var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == "New" && res.CurrentStage != "Relation Manager")).ToListAsync();
        return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
    }
    public async Task<ReturnProductionDto> GetProduction(Guid UserId, Guid id)
    {
        var product = await _cbeContext.ProductionCapacities
                        .FirstOrDefaultAsync(c => c.Id == id);
        return _mapper.Map<ReturnProductionDto>(product);
    }
    public async Task<PlantEditPostDto> GetPlantProduction(Guid UserId, Guid id)
    {
        var product = await _cbeContext.ProductionCapacities
                        .FirstOrDefaultAsync(c => c.Id == id);
        return _mapper.Map<PlantEditPostDto>(product);
    }







        public async Task<IEnumerable<ReturnProductionDto>> GetRejectedProductions(Guid ProductionCaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == ProductionCaseId && (res.CurrentStatus == "Reject" && res.CurrentStage == "Relation Manager")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }


        public async Task<IEnumerable<ReturnProductionDto>> GetRmRejectedProductions(Guid UserId, Guid PCECaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && res.CurrentStatus == "Rejected" && res.CurrentStage == "Relation Manager").ToListAsync();
            var productionDtos = _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
            return productionDtos;
        }


        public async Task<IEnumerable<ReturnProductionDto>> GetPendProductions(Guid ProductionCaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == ProductionCaseId && (res.CurrentStage != "Relation Manager" && res.CurrentStatus != "Completed")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async  Task<IEnumerable<ReturnProductionDto>> GetRmComProductions(Guid ProductionCaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == ProductionCaseId && res.CurrentStage == "Checker officer" && res.CurrentStatus == "Completed").ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        // public async Task<IEnumerable<ProductionAssignmentDto>> GetMyAssignmentProductions(Guid UserId, Guid ProductionCaseId)
        // {
        //     var userSupervised = await _cbeContext.CreateUsers.Where(res => res.SupervisorId == UserId).ToListAsync();
        //     var ProductionAssigmentDtos = new List<ProductionAssignmentDto>();

        //     foreach (var item in userSupervised)
        //     {
        //         var productionCaseAssignment = await _cbeContext.ProductionCaseAssignments.Include(x => x.User).Include(x => x.ProductionCapacity).Where(res => res.UserId == item.Id && res.ProductionCapacity.PCECaseId == ProductionCaseId).ToListAsync();
        //         productionCaseAssignment = productionCaseAssignment.DistinctBy(res => res.ProductionCapacityId).ToList();
        //         foreach (var items in productionCaseAssignment)
        //         {
        //             var productionAssigmentDto = new ProductionAssignmentDto
        //             {
        //                 ProductionCapacityId = items.ProductionCapacityId,
        //                 PCECaseId = ProductionCaseId,
        //                 PropertyOwner = items.ProductionCapacity.PropertyOwner,
        //                 ProductionCaseAssignmentId = items.Id,
        //                 Role = items.ProductionCapacity.Role,
        //                 Type = EnumToDisplayName(items.ProductionCapacity.Type),
        //                 Category = EnumToDisplayName(items.ProductionCapacity.Category),
        //                 User = items.User.Name,
        //                 AssignmentDate = items.AssignmentDate,
        //                 Status = items.Status,
        //             };
        //             ProductionAssigmentDtos.Add(productionAssigmentDto);
        //         }
        //     }
        //     return ProductionAssigmentDtos;
        // }

        public async Task<IEnumerable<ProductionAssignmentDto>> GetMyAssignmentProductions(Guid UserId, Guid PCECaseId)
        {
            var supervisedUsers = await _cbeContext.CreateUsers.Where(user => user.SupervisorId == UserId).Select(user => user.Id).ToListAsync();

            var productionAssignments = await _cbeContext.ProductionCaseAssignments
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
                                            ProductionCaseAssignmentId = assignment.Id,
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
        



        public async Task<IEnumerable<ReturnProductionDto>> MyReturnedProductions(Guid UserId)
        {
            List<ProductionCaseAssignment> caseAssignments = await _cbeContext.ProductionCaseAssignments.Where(ca => ca.UserId == UserId && ca.Status == "Correction").ToListAsync();
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

        public async Task<IEnumerable<ProductionCapacityCorrectionReturnDto>> GetComments(Guid CollateralId)
        {
            var comments = await _cbeContext.ProductionCapacityCorrections.Where(ca => ca.ProductionCapacityId == CollateralId).ToListAsync();
            if (comments != null)
            {
                return _mapper.Map<IEnumerable<ProductionCapacityCorrectionReturnDto>>(comments);
            }


            return null;
        }

        public async Task<IEnumerable<ReturnProductionDto>> GetRemarkProductions(Guid UserId, Guid PCECaseId)
        {
            var productioncaseAssignments = await _cbeContext.ProductionCaseAssignments
                                                            .Include(res => res.ProductionCapacity)
                                                            .Where(res => res.UserId == UserId 
                                                                        && res.ProductionCapacity.PCECaseId == PCECaseId 
                                                                        && res.Status.Contains("Remark"))
                                                            .ToListAsync();
            var productions = productioncaseAssignments.Select(res => res.ProductionCapacity);
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async  Task<ReturnProductionDto> GetProductionCapacityById(Guid productionId)
        {
            var productionById = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(res => res.Id == productionId);
            return _mapper.Map<ReturnProductionDto>(productionById);

        }
        public async Task<ReturnProductionDto> GetManufuctringProductionCapacityEvalutionById(Guid productionId)       {
 
            var productionById = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(res => res.Id == productionId);
            return _mapper.Map<ReturnProductionDto> (productionById);

        }
        public async Task<PlantEditPostDto> GetPlantProductionCapacityEvalutionById(Guid productionId)
        {

            var productionById = await _cbeContext.PCEEvaluations.Include(res => res.PCE).FirstOrDefaultAsync(res => res.PCEId == productionId);
            return _mapper.Map<PlantEditPostDto>(productionById);

        }
        public async Task<PCEEvaluationReturnDto> GetValuationById(Guid productionId)
        {

            var productionById = await _cbeContext.PCEEvaluations.Include(res => res.PCE).FirstOrDefaultAsync(res => res.PCEId == productionId);
            return _mapper.Map<PCEEvaluationReturnDto>(productionById);

        }
       


        private async Task UploadFile(Guid UserId, string Category, ProductionCapacity production, IFormFile? file)
        {
            if (file != null)
            {
                await _uploadFileService.CreateUploadFile(UserId, new CreateFileDto()
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
            if (await _uploadFileService.DeleteFile(file.Id))
            {
                return true;
            }
            return false;
        }
        

      
        public async Task<bool> UploadProductionFile(Guid UserId, IFormFile file, Guid caseId, string DocumentCatagory)
        {
            var collateral = await _cbeContext.ProductionCapacities.FindAsync(caseId);
            //var collateral = await _cbeContext.ProductionCapacities
            //        .FirstOrDefaultAsync(pc => pc.PCECaseId == caseId);
            if (collateral == null)
            {
                return false;
            }

            var CollateralFile = new CreateFileDto()
            {
                File = file ?? throw new ArgumentNullException(nameof(file)),
                CollateralId = caseId,
                Catagory = DocumentCatagory,
                CaseId =collateral.PCECaseId,

            };
            if (await _uploadFileService.CreateUploadFile(UserId, CollateralFile) != Guid.Empty)
            {
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<ReturnProductionDto>> GetRmComCollaterals(Guid PCECaseId)
        {
            var collaterals = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && res.CurrentStage == "Relation Manager" && res.CurrentStatus == "Completed").ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(collaterals);
        }
    }
}

