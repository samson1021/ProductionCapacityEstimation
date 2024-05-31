using mechanical.Models.Dto.UploadFileDto;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Services.UploadFileService
{
    public interface IUploadFileService
    {
        Task<Guid> CreateUploadFile (Guid userId, CreateFileDto file);
        Task<ReturnFileDto> GetUploadFile(Guid? uploadFileId);
        Task<IEnumerable<ReturnFileDto>> GetUploadFileByCollateralId(Guid? CollateralId);
        Task<ActionResult>DownloadFile(Guid Id);
        Task<(byte[], string, string)> ViewFile(Guid Id);
        Task<bool> DeleteFile(Guid Id);
        Task<ReturnFileDto> UpdateFile(Guid Id, CreateFileDto file);
    }
}
