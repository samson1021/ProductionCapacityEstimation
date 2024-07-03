using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.SignatureDto;
using mechanical.Models.Dto.SignatureOfEmployeInfo;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;
using mechanical.Services.UploadFileService;

namespace mechanical.Services.EmployeeInformationService
{
    public class EmployeeService: IEmployeeService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IUploadFileService _uploadFileService;
        public EmployeeService(CbeContext cbeContext, IMapper mapper, IUploadFileService uploadFileService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _uploadFileService = uploadFileService;
        }
        public async Task<EmployeeInfoes> GetEmployeeInfos(string EmployeeId)
        {
            var addSignature = _mapper.Map<EmployeeInfoes>(EmployeeId);

            addSignature.emp_ID = "50168";
            addSignature.pr_Number = "location";
            addSignature.position = "50168";
            addSignature.supervisor_name="gech";

            await _cbeContext.Employees.AddAsync(addSignature);
            await _cbeContext.SaveChangesAsync();
            return addSignature;
        }

   
    }
}
