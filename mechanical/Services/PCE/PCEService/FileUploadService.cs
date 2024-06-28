using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

using mechanical.Data;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.FileUploadDto;
using mechanical.Models.PCE.Enum.PCEEnums.File;

namespace mechanical.Services.PCE.PCEService
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
  

        public async Task<List<FileUpload>> CreateFiles(Guid UserId, Guid PCEId, ICollection<FileCreateDto> Files, DocumentType Type, string folderName)
        {
            Console.WriteLine("file upload service create files...............................................................");
            var savedFiles = new List<FileUpload>();

            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "UploadFile", folderName);
            Directory.CreateDirectory(uploadPath);
            Console.WriteLine("Files count: " + Files.Count); // Add this line
        
            if (Files != null && Files.Count > 0)
            {
                foreach (var fileDto in Files)
                {
            Console.WriteLine("#################################dto");
            Console.WriteLine(fileDto != null);
            Console.WriteLine("#################################");
                    if (fileDto.File.Length > 0)
                    {
                        var fileId = Guid.NewGuid();
                        string filePath = Path.Combine(uploadPath, fileId.ToString() + "_" + fileDto.File.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileDto.File.CopyToAsync(stream);
                        }

                        var file = new FileUpload
                        {
                            Id = fileId,
                            Name = fileDto.File.FileName,
                            Type = Type,
                            ContentType = fileDto.File.ContentType,
                            Extension = Path.GetExtension(fileDto.File.FileName),
                            Size = fileDto.File.Length,
                            PCEId = PCEId,
                            UploadAt = DateTime.UtcNow, 
                            UploadedBy = UserId,
                            Path = filePath
                        };
                        savedFiles.Add(file);
                    }
                }

                // Adding all file uploads to the database context in one batch
                await _cbeContext.FileUploads.AddRangeAsync(savedFiles);
                // await _cbeContext.AddRangeAsync(savedFiles);
                await _cbeContext.SaveChangesAsync();
            }
            Console.WriteLine("#################################");
            Console.WriteLine(savedFiles != null);
            Console.WriteLine("#################################");
            return savedFiles;
        }

        // public async Task<List<FileUpload>> CreateFiles(Guid UserId, Guid PCEId, ICollection<FileCreateDto> Files, DocumentType Type, string folderName)
        // // private async Task<List<FileUpload>> SaveFilesAsync(ICollection<FileCreateDto> Files)
        // {
         
        //     Console.WriteLine("file upload service create files...............................................................");
        //     var savedFiles = new List<FileUpload>();

        //     string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);
        //     Directory.CreateDirectory(uploadPath);

        //     if (Files != null)
        //     {
        //         Console.WriteLine(Files);
        //         foreach (var file in Files)
        //         {
        //             Console.WriteLine("inside file upload service create files...............................................................");
        //             if (file.File.Length > 0)
        //             {
        //                 string filePath = Path.Combine(uploadPath, Guid.NewGuid().ToString() + "_" + file.File.FileName);
        //                 using (var stream = new FileStream(filePath, FileMode.Create))
        //                 {
        //                     await file.File.CopyToAsync(stream);
        //                 }

        //                 savedFiles.Add(new FileUpload
        //                 {
        //                     Id = Guid.NewGuid(),
        //                     Name = file.File.FileName,
        //                     ContentType = file.File.ContentType,
        //                     Size = file.File.Length,
        //                     Path = filePath,
        //                     Type = file.Type,
        //                     PCEId = PCEId,
        //                     UploadAt = DateTime.UtcNow,
        //                     UploadedBy = UserId
        //                 });
        //             }
        //         }
        //     }

        //     return savedFiles;
        // }



        // public async Task<Guid> CreateFile(Guid UserId, Guid PCEId, FileCreateDto FileDto, DocumentType Type)
        // {
        //     var fileId = Guid.NewGuid();
        //     Console.WriteLine("create file...............................................................");
        //     using (var memoryStream = new MemoryStream())
        //     {
        //         await FileDto.File.CopyToAsync(memoryStream);
        //         var file = new FileUpload
        //         {                   
        //             Id = fileId,
        //             Name = FileDto.File.FileName,
        //             Type = Type,
        //             ContentType = FileDto.File.ContentType,
        //             Extension = Path.GetExtension(FileDto.File.FileName),
        //             Size = FileDto.File.Length,   
        //             PCEId = PCEId,
        //             UploadAt = DateTime.Now,
        //             UploadedBy = UserId
        //         };

        //         file.Path = Path.Combine("file", file.Id.ToString() + file.Extension);
        //         _cbeContext.FileUploads.AddAsync(file);
        //         await _cbeContext.SaveChangesAsync();
        //     }

        //     return fileId;
        // }
        
        // public async Task<IEnumerable<Guid>> CreateFiles(Guid UserId, Guid PCEId, IEnumerable<FileCreateDto> Files, DocumentType Type)
        // {
        //     var fileIds = new List<Guid>();

        //     Console.WriteLine("file upload service create files...............................................................");
        //     if (Files != null)
        //     {
        //         foreach (var file in Files)
        //         {
        //             var fileId = await CreateFile(UserId, PCEId, file, Type);
        //             fileIds.Add(fileId);
        //         }

        //     }
        //     return fileIds;
        // }

        // public async Task<Guid> CreateFile(Guid UserId, Guid PCEId, FileCreateDto File, String Type)
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
        //     file.UploadedBy = UserId;
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
