using Microsoft.AspNetCore.Mvc;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.FileUploadDto;
using mechanical.Models.PCE.Enum.PCEEnums.File;

namespace mechanical.Services.PCE.PCEService
{
    public interface IFileUploadService
    {
        // Task<Guid> CreateFile(Guid userId, Guid PCEId, FileCreateDto file, DocumentType Type);
        Task<List<FileUpload>> CreateFiles(Guid UserId, Guid PCEId, ICollection<FileCreateDto> Files, DocumentType Type, string folderName);
        // Task<IEnumerable<Guid>> CreateFiles(Guid userId, Guid PCEId, IEnumerable<FileCreateDto> files, DocumentType Type);
        Task<FileReturnDto> GetFile(Guid? FileId);
        Task<IEnumerable<FileReturnDto>> GetFileByPCEId(Guid? EntityId);
        Task<bool> DeleteFile(Guid Id);
        Task<FileReturnDto> UpdateFile(Guid Id, Guid PCEId, FileCreateDto file, DocumentType Type);
        Task<(byte[], string, string)> ViewFile(Guid Id);
        Task<ActionResult>DownloadFile(Guid Id);
    }
}
