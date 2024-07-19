using AutoMapper;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.CaseAssignmentDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using System.ComponentModel.DataAnnotations;
using mechanical.Models.Dto.SignatureDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.ConstMngAgrMachineryDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.UserDto;
using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Dto.CaseTerminateDto;

/////
using mechanical.Models.PCE.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;
using mechanical.Models.PCE.Dto.ProductionCaseAssignmentDto;
/////

namespace mechanical.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //create the new mapping for PCE
            CreateMap<PCECaseDto, PCECase>().ReverseMap();
            CreateMap<PCECaseReturntDto, PCECase>().ReverseMap();

            CreateMap<PCENewCaseDto, PCECase>().ReverseMap();
            CreateMap<PCECaseTimeLinePostDto, PCECaseTimeLine>().ReverseMap();
            CreateMap<PCECaseTimeLineReturnDto, PCECaseTimeLine>().ReverseMap();



            //create the new mapping for PCE
            CreateMap<PCECaseDto, PCECase>().ReverseMap();
            CreateMap<PCECaseReturntDto, PCECase>();

            CreateMap<PCECase, PCECaseReturntDto>()
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ProductionCapacities.Select(c => c.ProductionType)))
                .ForMember(dest => dest.TotalNoOfCollateral, opt => opt.MapFrom(src => src.ProductionCapacities.Count()));

            CreateMap<PCENewCaseDto, PCECase>();

            CreateMap<PCECase, PCENewCaseDto>()
                .ForMember(dest => dest.NoOfCollateral, opt => opt.MapFrom(src => src.ProductionCapacities.Select(c=>c.CurrentStatus).Count()))
                .ForMember(dest => dest.TotalNoOfCollateral, opt => opt.MapFrom(src => src.ProductionCapacities.Count()));


            CreateMap<PCECaseTimeLinePostDto, PCECaseTimeLine>().ReverseMap();
            CreateMap<PCECaseTimeLineReturnDto, PCECaseTimeLine>().ReverseMap();

            // CreateMap<PCEReturnFileDto, PCEUploadFile>().ReverseMap();
            // CreateMap<PCECreateFileDto, PCEUploadFile>().ReverseMap();           

            //manufatring PCE
            CreateMap<ProductionPostDto, ProductionCapacity>();
            CreateMap<PlantCapacityEstimationPostDto, ProductionCapacity>();
            CreateMap<ProductionCapacity, ReturnProductionDto>()
                 .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<ProductionCaseAssignmentDto, ProductionCaseAssignment>().ReverseMap();

            CreateMap<PlantPostDto, ProductionCapacity>().ReverseMap();

            CreateMap<PlantEditPostDto, ProductionCapacity>().ReverseMap();



            CreateMap<CollateralPostDto, Collateral>();
            CreateMap<CivilCollateralPostDto, Collateral>();
            CreateMap<AgricultureCollateralPostDto, Collateral>();
            CreateMap<Collateral, ReturnCollateralDto>()
                 .ForMember(dest => dest.Category, opt => opt.MapFrom(src => EnumToDisplayName(src.Category)))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => EnumToDisplayName(src.Type)));

            CreateMap<CaseCommentPostDto, CaseComment>();
            CreateMap<CaseComment,CaseCommentReturnDto>()
                .ForMember(dest=>dest.AuthorName, opt=>opt.MapFrom(src=>src.Author.Name));

            CreateMap<CaseSchedulePostDto, CaseSchedule>();
            CreateMap<CaseSchedule, CaseScheduleReturnDto>();

            CreateMap<CaseTerminatePostDto, CaseTerminate>();
            CreateMap<CaseTerminateReturnDto, CaseTerminate>();

            CreateMap<CasePostDto, Case>()
                .ForMember(dest => dest.BussinessLicence, opt => opt.Ignore());
            CreateMap<Case, CaseReturntDto>()
                .ForMember(dest => dest.District,opt=>opt.MapFrom(src => src.District.Name))
                .ForMember(dest => dest.TotalNoOfCollateral, opt => opt.MapFrom(src => src.Collaterals.Count()));
            CreateMap<Case, CaseDto>()
                .ForMember(dest => dest.RequestingUnit, opt => opt.MapFrom(src=>src.CaseOriginator.Department))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District.Name))
                .ForMember(dest => dest.NoOfCollateral, opt => opt.MapFrom(src => src.Collaterals.Count()));



            CreateMap<Case, CaseTerminateDto>()
               .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District.Name))
               .ForMember(dest => dest.NoOfCollateral, opt => opt.MapFrom(src => src.Collaterals.Count()));
            CreateMap<Case, RMCaseDto>()
              .ForMember(dest => dest.Center, opt => opt.MapFrom(src => src.District.Name))
              .ForMember(dest => dest.NoOfCollateral, opt => opt.MapFrom(src => src.Collaterals.Count()));
            CreateMap<Case,MMNewCaseDto>()
                .ForMember(dest => dest.NoOfCollateral, opt => opt.MapFrom(src => src.Collaterals.Count()));
            CreateMap<Case, MMCaseDto>()
               .ForMember(dest => dest.Center, opt => opt.MapFrom(src => src.District.Name))
               .ForMember(dest => dest.NoOfCollateral, opt => opt.MapFrom(src => src.Collaterals.Count()));

            CreateMap<CaseTimeLinePostDto, CaseTimeLine>();
            CreateMap<CorrectionPostDto, Correction>();
            CreateMap<CorrectionRetunDto, Correction>().ReverseMap();
            CreateMap<CaseTimeLineDto, CaseTimeLine>().ReverseMap();
            CreateMap<CaseTimeLine, CaseTimeLineReturnDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<CaseAssignmentDto, CaseAssignment>().ReverseMap();
            CreateMap<CaseTerminate, CaseTerminateReturnDto>().ReverseMap();

            CreateMap<SignatureDto, Signatures>();
            CreateMap<Signatures, ReturnSignatureDto>();
            CreateMap<CreateMotorVehicleDto, MotorVehicle>();
            CreateMap<MotorVehicle, ReturnMotorVehicleDto>()
                .ForMember(dest => dest.EngineType, opt => opt.MapFrom(src => EnumToDisplayName(src.EngineType)))
                .ForMember(dest => dest.TransmissionType, opt => opt.MapFrom(src => EnumToDisplayName(src.TransmissionType)))
                .ForMember(dest => dest.NoOfCylinder, opt => opt.MapFrom(src => EnumToDisplayName(src.NoOfCylinder)))
                .ForMember(dest => dest.NumberOfAxle, opt => opt.MapFrom(src => EnumToDisplayName(src.NumberOfAxle)))
                .ForMember(dest => dest.BodyType, opt => opt.MapFrom(src => EnumToDisplayName(src.BodyType)))
                .ForMember(dest => dest.coolingSystem, opt => opt.MapFrom(src => EnumToDisplayName(src.coolingSystem)))
                .ForMember(dest => dest.CabinType, opt => opt.MapFrom(src => EnumToDisplayName(src.CabinType)))
                .ForMember(dest => dest.MotorVehicleMake, opt => opt.MapFrom(src => EnumToDisplayName(src.MotorVehicleMake)))
                .ForMember(dest => dest.AllocatedPointsRange, opt => opt.MapFrom(src => EnumToDisplayName(src.AllocatedPointsRange)))
                .ForMember(dest => dest.CurrentEqpmntCondition, opt => opt.MapFrom(src => EnumToDisplayName(src.CurrentEqpmntCondition)));
         
            CreateMap<ConstMngAgrMachinery, ConstMngAgMachineryReturnDto>()
                .ForMember(dest => dest.constructionMiningAgriculturalMachineryType, opt => opt.MapFrom(src => EnumToDisplayName(src.constructionMiningAgriculturalMachineryType)))
                .ForMember(dest => dest.TransmissionType, opt => opt.MapFrom(src => EnumToDisplayName(src.TransmissionType)))
                .ForMember(dest => dest.NoOfCylinder, opt => opt.MapFrom(src => EnumToDisplayName(src.NoOfCylinder)))
                .ForMember(dest => dest.NumberOfAxle, opt => opt.MapFrom(src => EnumToDisplayName(src.NumberOfAxle)))
                .ForMember(dest => dest.EngineType, opt => opt.MapFrom(src => EnumToDisplayName(src.EngineType)))
                .ForMember(dest => dest.PowerSupply, opt => opt.MapFrom(src => EnumToDisplayName(src.PowerSupply)))
                .ForMember(dest => dest.CabinType, opt => opt.MapFrom(src => EnumToDisplayName(src.CabinType)))
                .ForMember(dest => dest.IgnitionSystem, opt => opt.MapFrom(src => EnumToDisplayName(src.IgnitionSystem)))
                .ForMember(dest => dest.CoolingType, opt => opt.MapFrom(src => EnumToDisplayName(src.CoolingType)))
                .ForMember(dest => dest.TechnologyStandard, opt => opt.MapFrom(src => EnumToDisplayName(src.TechnologyStandard)))
                .ForMember(dest => dest.AllocatedPointsRange, opt => opt.MapFrom(src => EnumToDisplayName(src.AllocatedPointsRange)))
                .ForMember(dest => dest.CurrentEqpmntCondition, opt => opt.MapFrom(src => EnumToDisplayName(src.CurrentEqpmntCondition)));


            CreateMap<ConstMngAgrMachineryPostDto, ConstMngAgrMachinery>();
            CreateMap<ConstMngAgrMachinery, ConstMngAgrMachineryPostDto>();
            CreateMap<MoRejectCaseDto, Reject>();
          
            CreateMap<IndBldgFacilityEquipmentPostDto, IndBldgFacilityEquipment>();
            CreateMap<IndBldgFacilityEquipment, IndBldgFacilityEquipmentReturnDto>()
             .ForMember(dest => dest.IndustrialBuildingMachineryType, opt => opt.MapFrom(src => EnumToDisplayName(src.IndustrialBuildingMachineryType)))
             .ForMember(dest => dest.EngineType, opt => opt.MapFrom(src => EnumToDisplayName(src.EngineType)))
             .ForMember(dest => dest.PowerSupply, opt => opt.MapFrom(src => EnumToDisplayName(src.PowerSupply)))
             .ForMember(dest => dest.TechnologyStandard, opt => opt.MapFrom(src => EnumToDisplayName(src.TechnologyStandard)))
             .ForMember(dest => dest.AllocatedPointsRange, opt => opt.MapFrom(src => EnumToDisplayName(src.AllocatedPointsRange)))
             .ForMember(dest => dest.CurrentEqpmntCondition, opt => opt.MapFrom(src => EnumToDisplayName(src.CurrentEqpmntCondition)));
            CreateMap<IndBldgFacilityEquipment, IndBldgFacilityEquipmentPostDto>();

            CreateMap<CreateUser, UserReturnDto>()
                    .ForMember(dest => dest.Role, opt => opt.MapFrom(src =>src.Role.Name))
                    .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District.Name));
            
            ///////
           
            CreateMap<UploadFile, ReturnFileDto>().ReverseMap();
            CreateMap<CreateFileDto, ReturnFileDto>()
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.File.ContentType))
                .ReverseMap();
            CreateMap<CreateFileDto, UploadFile>()
                .ForMember(dest => dest.Catagory, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return context.Items["Catagory"];
                }));

            CreateMap<TimePeriod, TimePeriodDto>().ReverseMap();
            CreateMap<DatePeriod, DatePeriodDto>().ReverseMap();
            CreateMap<DateTimePeriodDto, DateTimePeriod>()
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
                .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End))
                .ReverseMap();

            CreateMap<PCEEvaluation, PCEEvaluationReturnDto>();
            // .ForMember(dest => dest.SupportingEvidences, opt => opt.MapFrom(src => src.SupportingDocuments.Where(f => f.Catagory == "Supporting Evidence")))
            //.ForMember(dest => dest.ProductionProcessFlowDiagrams, opt => opt.MapFrom(src => src.SupportingDocuments.Where(f => f.Catagory == "Production Process Flow Diagram")));

            CreateMap<PCEEvaluationPostDto, PCEEvaluation>();
             //  .ForMember(dest => dest.SupportingDocuments, opt => opt.Ignore());
        
            CreateMap<PCEEvaluationReturnDto, PCEEvaluationPostDto>()
                .ForMember(dest => dest.SupportingEvidences, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionProcessFlowDiagrams, opt => opt.Ignore());

            CreateMap<PCEEvaluation, PCEEvaluationPostDto>()
                .ForMember(dest => dest.SupportingEvidences, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionProcessFlowDiagrams, opt => opt.Ignore())
                .ForMember(dest => dest.TimeConsumedToCheck, opt => opt.MapFrom(src => new DateTimePeriod
                {
                    Start = src.TimeConsumedToCheck.Start,
                    End = src.TimeConsumedToCheck.End
                }))
                .ReverseMap();
            CreateMap<PCERejectPostDto, ProductionReject>();
            ///////
        }

        string EnumToDisplayName<TEnum>(TEnum enumValue)
        {
            return (typeof(TEnum).GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute)?.Name ?? enumValue.ToString();
        }
    }
}
