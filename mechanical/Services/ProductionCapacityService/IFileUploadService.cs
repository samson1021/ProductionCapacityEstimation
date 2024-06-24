using Microsoft.AspNetCore.Mvc;
using mechanical.Models.Dto.ProductionCapacityDto.FileUploadDto;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.File;

namespace mechanical.Services.ProductionCapacityService
{
    public interface IFileUploadService
    {
        Task<Guid> CreateFile(Guid userId, Guid PCEId, FileCreateDto file, DocumentType Type);
        Task<IEnumerable<Guid>> CreateFiles(Guid userId, Guid PCEId, IEnumerable<FileCreateDto> files, DocumentType Type);
        Task<FileReturnDto> GetFile(Guid? FileId);
        Task<IEnumerable<FileReturnDto>> GetFileByPCEId(Guid? EntityId);
        Task<bool> DeleteFile(Guid Id);
        Task<FileReturnDto> UpdateFile(Guid Id, Guid PCEId, FileCreateDto file, DocumentType Type);
        Task<(byte[], string, string)> ViewFile(Guid Id);
        Task<ActionResult>DownloadFile(Guid Id);
    }
}
