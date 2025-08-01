﻿using AutoMapper;
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
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseCommentDto;
using mechanical.Models.PCE.Dto.PCECaseScheduleDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.PCECaseTerminateDto;
using mechanical.Models.PCE.Dto.PCECaseAssignmentDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.Login;
using mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto;
using mechanical.Models.Dto.InternalReport;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Models.Dto.NotificationDto;


namespace mechanical.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
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
            CreateMap<Case, CaseReturnDto>()
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

            CreateMap<IndBldgFacilityEquipmentCostsPostDto, IndBldgFacilityEquipmentCosts>();
            CreateMap<IndBldgFacilityEquipmentCosts, IndBldgFacilityEquipmentCostsReturnDto>();
            CreateMap<IndBldgFacilityEquipmentCosts,IndBldgFacilityEquipmentCostsDto>().ReverseMap();

            CreateMap<IndBldgFacilityEquipment, IndBldgFacilityEquipmentPostDto>();

            CreateMap<User, UserReturnDto>()
                    .ForMember(dest => dest.Role, opt => opt.MapFrom(src =>src.Role.Name))
                    .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District.Name));
            
            CreateMap<User, ReturnUserDto>()
                    .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src =>src.Role.Name))
                    .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.District.Name));

            CreateMap<User, UserAdAttribute>().ReverseMap();
            CreateMap<User, User>()
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<UploadFile, ReturnFileDto>().ReverseMap();
            CreateMap<UploadFile, ReturnPCEReportFileDto>().ReverseMap();
            CreateMap<CreateFileDto, ReturnFileDto>()
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.File.ContentType));
                // .ReverseMap();
            CreateMap<CreateFileDto, UploadFile>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return context.Items["Category"];
                }));
            ///////
            
            //create the new mapping for PCE
            CreateMap<PCECaseDto, PCECase>().ReverseMap();
            CreateMap<PCECaseReturnDto, PCECase>().ReverseMap();
            CreateMap<PCECaseTimeLinePostDto, PCECaseTimeLine>().ReverseMap();
            CreateMap<PCECaseTimeLineReturnDto, PCECaseTimeLine>().ReverseMap();
            CreateMap<PCECase, PCECaseReturnDto>()
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District.Name))
                .ForMember(dest => dest.NoOfProductions, opt => opt.MapFrom(src => src.ProductionCapacities.Select(c=>c.CurrentStatus).Count()))
                .ForMember(dest => dest.TotalNoOfProductions, opt => opt.MapFrom(src => src.ProductionCapacities.Count()));

            //manufatring PCE
            CreateMap<ProductionPostDto, ProductionCapacity>().ReverseMap();
            CreateMap<ProductionCapacity, ProductionEditDto>().ReverseMap();
            CreateMap<ProductionCapacity, ProductionReturnDto>()
                    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));

            CreateMap<PCECaseAssignmentDto, PCECaseAssignment>().ReverseMap();
            ////////////
            CreateMap<PCECase, PCECaseTerminateDto>()
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.District.Name))
                .ForMember(dest => dest.TotalNoOfProductions, opt => opt.MapFrom(src => src.ProductionCapacities.Count()));

            CreateMap<PCECaseTerminatePostDto, PCECaseTerminate>().ReverseMap();
            CreateMap<PCECaseTerminateReturnDto, PCECaseTerminate>().ReverseMap();

            CreateMap<PCECaseCommentPostDto, PCECaseComment>();
            CreateMap<PCECaseComment, PCECaseCommentReturnDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            ////////////

            CreateMap<PCECaseSchedulePostDto, PCECaseSchedule>();
            CreateMap<PCECaseSchedule, PCECaseScheduleReturnDto>();

            /////////////
            CreateMap<DateTimeRange, DateTimeRangePostDto>().ReverseMap();
            CreateMap<DateTimeRange, DateTimeRangeReturnDto>().ReverseMap();
            CreateMap<DateTimeRange, DateTimeRangeUpdateDto>().ReverseMap();
            CreateMap<DateTimeRangeReturnDto, DateTimeRangeUpdateDto>().ReverseMap();

            // CreateMap<DateRange, DateRangeDto>().ReverseMap();
            CreateMap<ReturnedProductionPostDto, ReturnedProduction>();
            CreateMap<ReturnedProduction, ReturnedProductionDto>();

            CreateMap<PCEEvaluationReturnDto, PCEEvaluationPostDto>();
            CreateMap<PCEEvaluationReturnDto, PCEEvaluationUpdateDto>()
                .ForMember(dest => dest.WitnessForm, opt => opt.MapFrom(src => src.WitnessForm))
                .ForMember(dest => dest.SupportingEvidences, opt => opt.MapFrom(src => src.SupportingEvidences))
                .ForMember(dest => dest.ProductionProcessFlowDiagrams, opt => opt.MapFrom(src => src.ProductionProcessFlowDiagrams))
                .ReverseMap()
                // .ForMember(dest => dest.PCE, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedFiles, opt => opt.Ignore());

            CreateMap<PCEEvaluation, PCEEvaluationPostDto>()
                .ForMember(dest => dest.WitnessForm, opt => opt.Ignore())
                .ForMember(dest => dest.SupportingEvidences, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionProcessFlowDiagrams, opt => opt.Ignore())
                .ForMember(dest => dest.TimeConsumedToCheck, opt => opt.MapFrom(src => src.TimeConsumedToCheck))
                .ReverseMap()
                .ForMember(dest => dest.TimeConsumedToCheck, opt => opt.MapFrom(src => src.TimeConsumedToCheck));

            CreateMap<PCEEvaluation, PCEEvaluationUpdateDto>()
                .ForMember(dest => dest.WitnessForm, opt => opt.Ignore())
                .ForMember(dest => dest.SupportingEvidences, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionProcessFlowDiagrams, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionLines, opt => opt.MapFrom(src => src.ProductionLines.OrderBy(pl => pl.LineName)))
                .ForMember(dest => dest.Justifications, opt => opt.MapFrom(src => src.Justifications.OrderBy(j => (int)j.Reason)))
                .ForMember(dest => dest.TimeConsumedToCheck, opt => opt.MapFrom(src => src.TimeConsumedToCheck))
                .ReverseMap()
                .ForMember(dest => dest.ProductionLines, opt => opt.MapFrom(src => src.ProductionLines.OrderBy(pl => pl.LineName)))
                .ForMember(dest => dest.Justifications, opt => opt.MapFrom(src => src.Justifications.OrderBy(j => (int)j.Reason)))
                .ForMember(dest => dest.TimeConsumedToCheck, opt => opt.MapFrom(src => src.TimeConsumedToCheck));

            CreateMap<PCEEvaluation, PCEEvaluationReturnDto>()
                .ForMember(dest => dest.ProductionLines, opt => opt.MapFrom(src => src.ProductionLines.OrderBy(pl => pl.LineName)))
                .ForMember(dest => dest.Justifications, opt => opt.MapFrom(src => src.Justifications.OrderBy(j => (int)j.Reason)))
                .ForMember(dest => dest.TimeConsumedToCheck, opt => opt.MapFrom(src => src.TimeConsumedToCheck))
                .ForMember(dest => dest.UploadedFiles, opt => opt.Ignore())
                .ForMember(dest => dest.WitnessForm, opt => opt.Ignore())
                .ForMember(dest => dest.SupportingEvidences, opt => opt.Ignore())
                .ForMember(dest => dest.ProductionProcessFlowDiagrams, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ProductionLine, ProductionLineReturnDto>()
                .ForMember(dest => dest.ProductionLineInputs, opt => opt.MapFrom(src => src.ProductionLineInputs.OrderBy(input => input.Type)))
                .ReverseMap();

            CreateMap<ProductionLine, ProductionLineUpdateDto>()
                .ForMember(dest => dest.ProductionLineInputs, opt => opt.MapFrom(src => src.ProductionLineInputs.OrderBy(input => input.Type)))
                .ReverseMap();
            CreateMap<ProductionLine, ProductionLinePostDto>().ReverseMap();
            CreateMap<ProductionLineReturnDto, ProductionLineUpdateDto>().ReverseMap();

            CreateMap<ProductionLineInput, ProductionLineInputReturnDto>().ReverseMap();
            CreateMap<ProductionLineInput, ProductionLineInputUpdateDto>().ReverseMap();
            CreateMap<ProductionLineInput, ProductionLineInputPostDto>().ReverseMap();
            CreateMap<ProductionLineInputReturnDto, ProductionLineInputUpdateDto>().ReverseMap();

            CreateMap<Justification, JustificationReturnDto>().ReverseMap();
            CreateMap<Justification, JustificationUpdateDto>().ReverseMap();
            CreateMap<Justification, JustificationPostDto>().ReverseMap();
            CreateMap<JustificationReturnDto, JustificationUpdateDto>().ReverseMap();
            ///////


            //Task Managment
            CreateMap<TaskManagment, TaskManagmentPostDto>().ReverseMap();
            CreateMap<TaskManagment, TaskManagmentUpdateDto>().ReverseMap();
            CreateMap<TaskManagment, TaskManagmentReturnDto>().ReverseMap();
            CreateMap<TaskManagmentReturnDto, TaskManagmentUpdateDto>().ReverseMap();
            CreateMap<TaskManagment, ShareTasksDto>().ReverseMap();
            // CreateMap<TaskManagment, ShareTasksDto>()
            //     .ForMember(dest => dest.SelectedRMs, opt => opt.Ignore());

            CreateMap<Notification, NotificationPostDto>().ReverseMap();
            CreateMap<Notification, NotificationReturnDto>().ReverseMap();
        
            CreateMap<TaskCommentPostDto, TaskComment>().ReverseMap();
            CreateMap<TaskComment, TaskCommentReturnDto>().ReverseMap();
            CreateMap<CommentHistory, CaseCorrectionHistoryRetunDto>();
        }

        string EnumToDisplayName<TEnum>(TEnum enumValue)
        {
            return (typeof(TEnum).GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute)?.Name ?? enumValue.ToString();
        }
    }
}
