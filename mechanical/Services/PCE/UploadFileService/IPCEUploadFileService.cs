
using mechanical.Models.PCE.Dto.PCEUploadFileDto;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Services.PCE.UploadFileService
{
    public interface IPCEUploadFileService
    {
        Task<Guid> CreateUploadFile(Guid userId, PCECreateFileDto file);
        Task<IEnumerable<PCEReturnFileDto>> GetUploadFileByCollateralId(Guid? CollateralId);
        Task<bool> DeleteFile(Guid Id);
        Task<ActionResult> DownloadFile(Guid Id);

        Task<(byte[], string, string)> ViewFile(Guid Id);


    }
}
