using Microsoft.AspNetCore.Mvc;
using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;

namespace mechanical.Services.UploadFileService
{
    public interface IUploadFileService
    {
        // Task<List<UploadFile>> CreateFiles(Guid UserId, ICollection<CreateFileDto> Files);
        Task<Guid> CreateUploadFile (Guid userId, CreateFileDto file);
        Task<ReturnFileDto> GetUploadFile(Guid? uploadFileId);
        Task<IEnumerable<ReturnFileDto>> GetUploadFileByCollateralId(Guid? CollateralId);
        Task<IEnumerable<ReturnPCEReportFileDto>> GetAllUploadFileByCaseId(Guid? CollateralId);
        //Task<ReturnPCEReportFileDto> GetSignatureByEmployeeId(string? CollateralId);

        Task<ActionResult> DownloadFile(Guid Id);
        Task<(byte[], string, string)> ViewFile(Guid Id);
        Task<bool> DeleteFile(Guid Id);
        Task<ReturnFileDto> UpdateFile(Guid Id, CreateFileDto file);
    }
}
