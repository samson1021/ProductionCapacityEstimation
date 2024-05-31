using AutoMapper;
using Signature_management.Model.Dto.AcknowledgementDto;
using Signature_management.Model.Dto.SignatureDto;
using Signature_management.Model.Entities;

namespace Signature_management.Mapper
{
    public class MappingProfile: Profile
    {
        public MappingProfile() { 
        
         CreateMap<Acknowledgement, AcknowledgementPostDto>();
         //CreateMap<Acknowledgement, AcknowledgementReturnDto>();
            CreateMap<Signatures, SignaturePostDto>();
        }
    }
}
