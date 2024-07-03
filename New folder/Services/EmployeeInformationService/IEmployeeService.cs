using mechanical.Models.Dto.SignatureDto;
using mechanical.Models.Dto.SignatureOfEmployeInfo;
using mechanical.Models.Entities;

namespace mechanical.Services.EmployeeInformationService
{
    public interface IEmployeeService
    {
        Task<EmployeeInfoes> GetEmployeeInfos(string EmployeeId);

    }
}
