using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

using mechanical.Data;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.File;
using mechanical.Models.PCE.Dto.FileUploadDto;

namespace mechanical.Services.PCE.FileUploadService
{
    public class FileUploadService : IFileUploadService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadService(IWebHostEnvironment webHostEnvironment, CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }
  

        public async Task<List<FileUpload>> CreateFiles(Guid UserId, Guid PCEEId, ICollection<IFormFile> Files, Category Category)
        // public async Task<List<FileUpload>> CreateFiles(Guid UserId, Guid PCEEId, ICollection<FileCreateDto> Files, Category Category)
        {
            var savedFiles = new List<FileUpload>();
            var folderName = "";
  
            switch (Category)
            {
                case Category.SupportingEvidence:            
                    folderName = "SupportingEvidences"; 
                    break;
                case Category.ProductionProcessFlowDiagram:
                    folderName = "FlowDiagrams";
                    break;
                default:
                    folderName = "Others";
                    break;
            }

            string uploadPath = Path.Combine("UploadFile", folderName);
            // string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "UploadFile", folderName);
            Directory.CreateDirectory(uploadPath);
        
            if (Files != null && Files.Count > 0)
            {
                foreach (var fileDto in Files)
                {
                    if (fileDto.Length > 0)
                    {
                        var fileId = Guid.NewGuid();
                        string filePath = Path.Combine(uploadPath, fileId.ToString() + "_" + fileDto.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileDto.CopyToAsync(stream);
                        }

                        var file = new FileUpload
                        {
                            Id = fileId,
                            Name = fileDto.FileName,
                            Category = Category,
                            ContentType = fileDto.ContentType,
                            Extension = Path.GetExtension(fileDto.FileName),
                            Size = fileDto.Length,
                            PCEEId = PCEEId,
                            UploadedAt = DateTime.UtcNow, 
                            UploadedBy = UserId,
                            Path = filePath
                        };
                        savedFiles.Add(file);
                    }
                }

                await _cbeContext.FileUploads.AddRangeAsync(savedFiles);
                // await _cbeContext.AddRangeAsync(savedFiles);
                await _cbeContext.SaveChangesAsync();
            }
            return savedFiles;
        }

        public async Task<FileReturnDto> GetFile(Guid? Id)
        {
            if(Id == null) return null;

            var file = await _cbeContext.FileUploads.FindAsync(Id);
            return _mapper.Map<FileReturnDto>(file);

        }
        public async Task <ActionResult>DownloadFile(Guid Id)
        {
            var file = await _cbeContext.FileUploads.FindAsync(Id);
            var filePath = file.Path;
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return  new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = file.Name };
        }
        public async Task<(byte[] , string, string)> ViewFile(Guid Id)
        {
            var file = await _cbeContext.FileUploads.FindAsync(Id);
            var filePath = file.Path;

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            string mimeCategory;
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".pdf":
                    mimeCategory = "application/pdf";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimeCategory = "image/jpeg";
                    break;
                case ".png":
                    mimeCategory = "image/png";
                    break;
                case ".gif":
                    mimeCategory = "image/gif";
                    break;
                default:
                    throw new ArgumentException("Unsupported file Category");
            }
            return (fileBytes, fileName, mimeCategory);
        }

        public async Task<IEnumerable<FileReturnDto>> GetFileByPCEEId(Guid? PCEEId)
        {
            if (PCEEId == null) return null;

            var files = await _cbeContext.FileUploads.Where(res=>res.PCEEId == PCEEId).ToListAsync();
            return _mapper.Map<IEnumerable<FileReturnDto>>(files);
        }
        public async Task<FileReturnDto> UpdateFile(Guid Id, Guid PCEEId, FileCreateDto File, Category Category)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var file = await _cbeContext.FileUploads.FindAsync(Id);
            if (file == null)
            {
                return null;
            }
            DeleteFile(file.Path);
            file.Name = File.File.FileName;
            file.Category = Category;
            file.ContentType = File.File.ContentType;
            file.Size = File.File.Length;
            file.Extension = Path.GetExtension(File.File.FileName);
            
            var folderName = "";
            switch (file.Category)
            {
                case Category.SupportingEvidence:            
                    folderName = "SupportingEvidences"; 
                    break;
                case Category.ProductionProcessFlowDiagram:
                    folderName = "FlowDiagrams";
                    break;
                default:
                    folderName = "Others";
                    break;
            }
            string filePath = Path.Combine("UploadFile", folderName, file.Id.ToString() + "_" + file.Name);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                File.File.CopyToAsync(fileStream);
            }
            file.Path = filePath;
            file.PCEEId = PCEEId;
            file.UploadedAt = DateTime.Now;
            file.UploadedBy = Guid.Parse(httpContext.Session.GetString("UserId"));

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
