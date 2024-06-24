using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using mechanical.Data;
using mechanical.Models.Entities.ProductionCapacity;
using mechanical.Models.Dto.ProductionCapacityDto.FileUploadDto;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.File;

namespace mechanical.Services.ProductionCapacityService
{
    public class FileUploadService : IFileUploadService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FileUploadService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
  
        public async Task<Guid> CreateFile(Guid userId, Guid PCEId, FileCreateDto FileDto, DocumentType Type)
        {
            var fileId = Guid.NewGuid();

            using (var memoryStream = new MemoryStream())
            {
                await FileDto.File.CopyToAsync(memoryStream);
                var file = new FileUpload
                {                   
                    Id = fileId,
                    Name = FileDto.File.FileName,
                    Type = Type,
                    ContentType = FileDto.File.ContentType,
                    Extension = Path.GetExtension(FileDto.File.FileName),
                    Size = FileDto.File.Length,   
                    PCEId = PCEId,
                    UploadAt = DateTime.Now,
                    UploadedBy = userId
                };

                file.Path = Path.Combine("file", file.Id.ToString() + file.Extension);
                _cbeContext.FileUploads.AddAsync(file);
                await _cbeContext.SaveChangesAsync();
            }

            return fileId;
        }
        
        public async Task<IEnumerable<Guid>> CreateFiles(Guid userId, Guid PCEId, IEnumerable<FileCreateDto> Files, DocumentType Type)
        {
            var fileIds = new List<Guid>();

            if (Files != null)
            {
                foreach (var file in Files)
                {
                    var fileId = await CreateFile(userId, PCEId, file, Type);
                    fileIds.Add(fileId);
                }

            }
            return fileIds;
        }

        // public async Task<Guid> CreateFile(Guid userId, Guid PCEId, FileCreateDto File, String Type)
        // {
        //     var file = new FileUpload();
        //     file.Id = Guid.NewGuid();
        //     file.Name = File.File.FileName;
        //     file.Type = Type;
        //     file.ContentType = File.File.ContentType;
        //     file.Size = File.File.Length;
        //     file.Extension = Path.GetExtension(File.File.FileName);

        //     var uniqueFileName = file.Id.ToString() + file.Extension;
        //     var filePath = Path.Combine("file", uniqueFileName);
        //     // using (var fileStream = new FileStream(filePath, FileMode.Create))
        //     using (var fileStream = new FileStream(filePath, FileMode.Create))
        //     {
        //         File.File.CopyToAsync(fileStream);
        //     }
        //     file.Path = filePath;
        //     file.PCEId = PCEId;
        //     file.UploadAt = DateTime.Now;
        //     file.UploadedBy = userId;
        //     await _cbeContext.FileUploads.AddAsync(file);
        //     await _cbeContext.SaveChangesAsync();
        //     return file.Id;
        // }
        public async Task<FileReturnDto> GetFile(Guid? Id)
        {
            if(Id == null) return null;

            var file = await _cbeContext.FileUploads.FindAsync(Id);
            return _mapper.Map<FileReturnDto>(file);

        }
        public async Task <ActionResult>DownloadFile(Guid Id)
        {
            var file = await _cbeContext.FileUploads.FindAsync(Id);
            var filePath = Path.Combine("file", Id.ToString() + file.Extension);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return  new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = file.Name };
        }
        public async Task<(byte[] , string, string)> ViewFile(Guid Id)
        {
            var file = await _cbeContext.FileUploads.FindAsync(Id);
            var filePath = Path.Combine("file", Id.ToString() + file.Extension);

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            string mimeType;
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".pdf":
                    mimeType = "application/pdf";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimeType = "image/jpeg";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                default:
                    throw new ArgumentException("Unsupported file type");
            }
            return (fileBytes, fileName, mimeType);
        }

        public async Task<IEnumerable<FileReturnDto>> GetFileByPCEId(Guid? PCEId)
        {
            if (PCEId == null) return null;

            var files = await _cbeContext.FileUploads.Where(res=>res.PCEId == PCEId).ToListAsync();
            return _mapper.Map<IEnumerable<FileReturnDto>>(files);
        }
        public async Task<FileReturnDto> UpdateFile(Guid Id, Guid PCEId, FileCreateDto File, DocumentType Type)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var file = await _cbeContext.FileUploads.FindAsync(Id);
            if (file == null)
            {
                return null;
            }
            DeleteFile(Path.Combine("file", file.Id.ToString() + file.Extension));
            file.Name = File.File.FileName;
            file.Type = Type;
            file.ContentType = File.File.ContentType;
            file.Size = File.File.Length;
            file.Extension = Path.GetExtension(File.File.FileName);

            var uniqueFileName = file.Id.ToString() + file.Extension;
            var filePath = Path.Combine("file", uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                File.File.CopyToAsync(fileStream);
            }
            file.Path = filePath;
            file.PCEId = PCEId;
            file.UploadAt = DateTime.Now;
            file.UploadedBy = Guid.Parse(httpContext.Session.GetString("userId"));

            _cbeContext.Update(file);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<FileReturnDto>(file);
        }

        public async Task<bool> DeleteFile(Guid Id)
        {
            var file = await _cbeContext.FileUploads.FindAsync(Id);
            if (file == null)
            {
               return false;
            }
            DeleteFile(Path.Combine("file", file.Id.ToString() + file.Extension));
            _cbeContext.FileUploads.Remove(file);
            await _cbeContext.SaveChangesAsync();
            return true;
        }
        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
