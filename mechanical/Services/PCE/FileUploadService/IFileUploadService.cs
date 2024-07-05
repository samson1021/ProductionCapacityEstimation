using Microsoft.AspNetCore.Mvc;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.File;
using mechanical.Models.PCE.Dto.FileUploadDto;

namespace mechanical.Services.PCE.FileUploadService
{
    public interface IFileUploadService
    {
        // Task<Guid> CreateFile(Guid userId, Guid PCEEId, FileCreateDto file, Category Type);
        Task<List<FileUpload>> CreateFiles(Guid UserId, Guid PCEEId, ICollection<IFormFile> Files, Category Type);
        // Task<List<FileUpload>> CreateFiles(Guid UserId, Guid PCEEId, ICollection<FileCreateDto> Files, Category Type);
        // Task<IEnumerable<Guid>> CreateFiles(Guid userId, Guid PCEEId, IEnumerable<FileCreateDto> files, Category Type);
        Task<FileReturnDto> GetFile(Guid? FileId);
        Task<IEnumerable<FileReturnDto>> GetFileByPCEEId(Guid? EntityId);
        Task<bool> DeleteFile(Guid Id);
        Task<FileReturnDto> UpdateFile(Guid Id, Guid PCEEId, FileCreateDto file, Category Type);
        Task<(byte[], string, string)> ViewFile(Guid Id);
        Task<ActionResult>DownloadFile(Guid Id);
    }
}
