using AutoMapper;
using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Security.Application;
using System.Net;
using AntiLdapInjection;

namespace mechanical.Services.UploadFileService
{
    public class UploadFileService : IUploadFileService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UploadFileService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> CreateUploadFile(Guid userId, CreateFileDto file)
        {
            var allowedExtensions = new[]
            {
                ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
                ".txt", ".rtf", ".odt", ".ods", ".odp"
            };

           var allowedContentTypes = new[]
           {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "application/vnd.ms-powerpoint",
                "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "text/plain",
                "application/rtf",
                "application/vnd.oasis.opendocument.text",
                "application/vnd.oasis.opendocument.spreadsheet",
                "application/vnd.oasis.opendocument.presentation"
            };

            // Get file extension and content type
            var fileExtension = Path.GetExtension(file.File.FileName).ToLowerInvariant();
            var contentType = file.File.ContentType.ToLowerInvariant();

            // Validate file extension and content type
            if (!allowedExtensions.Contains(fileExtension) ||
                !allowedContentTypes.Contains(contentType))
            {
                throw new InvalidOperationException(
                    $"File type '{fileExtension}' is not allowed. ");        
            }

            // Verify the actual file content matches the extension
            if (!IsValidDocumentFile(file.File))
            {
                throw new InvalidOperationException("File content doesn't match its extension");
            }
            var uploadFile = new UploadFile
            {
                Id = Guid.NewGuid(),
                Name = Path.GetFileNameWithoutExtension(file.File.FileName),
                ContentType = contentType,
                Size = file.File.Length,
                Extension = fileExtension,
                Category = file.Category,
                CaseId = file.CaseId,
                CollateralId = file.CollateralId,
                UploadDateTime = DateTime.UtcNow,
                userId = userId
            };

            var uniqueFileName = uploadFile.Id.ToString() + uploadFile.Extension;
            var sanitizedFileName = LdapEncoder.FilterEncode(uniqueFileName);
            var filePath = Path.Combine("UploadFile", sanitizedFileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.File.CopyToAsync(fileStream);
            }

            uploadFile.Path = filePath;

            await _cbeContext.UploadFiles.AddAsync(uploadFile);
            await _cbeContext.SaveChangesAsync();

            return uploadFile.Id;
            //Directory.CreateDirectory("UploadFile");

            //var uploadFile = new UploadFile();
            //uploadFile.Id = Guid.NewGuid();
            //uploadFile.Name = file.File.FileName;
            //uploadFile.ContentType = file.File.ContentType;
            //uploadFile.Size = file.File.Length;
            //uploadFile.Extension = Path.GetExtension(file.File.FileName);

            //var uniqueFileName = uploadFile.Id.ToString() + uploadFile.Extension;
            //var sanitizedFileName = LdapEncoder.FilterEncode(uniqueFileName);
            //var filePath = Path.Combine("UploadFile", sanitizedFileName);
            //using (var fileStream = new FileStream(filePath, FileMode.Create))
            //{
            //    file.File.CopyTo(fileStream);
            //}
            //uploadFile.Category = file.Category;
            //uploadFile.Path = filePath;
            //uploadFile.CaseId = file.CaseId;
            //uploadFile.CollateralId = file.CollateralId;
            //uploadFile.UploadDateTime = DateTime.UtcNow;
            //uploadFile.userId = userId;
            //await _cbeContext.UploadFiles.AddAsync(uploadFile);
            //await _cbeContext.SaveChangesAsync();
            //return uploadFile.Id;
        }
        private bool IsValidDocumentFile(IFormFile file)
        {
            try
            {
                // Read the first few bytes to check magic numbers
                using (var stream = file.OpenReadStream())
                {
                    var buffer = new byte[20];
                    stream.Read(buffer, 0, buffer.Length);

                    // Check for common document file signatures
                    if (buffer.Take(4).SequenceEqual(new byte[] { 0x25, 0x50, 0x44, 0x46 })) // PDF
                        return true;
                    if (buffer.Take(8).SequenceEqual(new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 })) // DOC, XLS, PPT
                        return true;
                    if (buffer.Take(4).SequenceEqual(new byte[] { 0x50, 0x4B, 0x03, 0x04 })) // DOCX, XLSX, PPTX
                        return true;

                    // Add more checks for other document types as needed
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
        public async Task<ReturnFileDto> GetUploadFile(Guid? Id)
        {
            if (Id == null) return null;

            var uploadFile = await _cbeContext.UploadFiles.FindAsync(Id);
            return _mapper.Map<ReturnFileDto>(uploadFile);

        }

        public async Task<ActionResult> DownloadFile(Guid Id)
        {
            //var uploadFile = await _cbeContext.UploadFiles.FindAsync(Id);
            //var filePath = Path.Combine("UploadFile", Id.ToString() + uploadFile.Extension);
            //byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            //return new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = uploadFile.Name };
            var (fileBytes, fileName, mimeType) = await ViewFile(Id);

            return new FileContentResult(fileBytes, mimeType) { FileDownloadName = fileName };
        }

        public async Task<(byte[], string, string)> ViewFile(Guid Id)
        {
            var uploadFile = await _cbeContext.UploadFiles.FindAsync(Id);
            var filePath = Path.Combine("UploadFile", Id.ToString() + uploadFile.Extension);
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found");
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
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
                case ".doc":
                case ".docx":
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".xls":
                case ".xlsx":
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".ppt":
                case ".pptx":
                    mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                default:
                    // Redirect to download for unsupported file types
                    mimeType = "application/octet-stream"; // Generic binary file type
                    break;
            }
            return (fileBytes, fileName, mimeType);
        }

        public async Task<IEnumerable<ReturnFileDto>> GetUploadFileByCollateralId(Guid? CollateralId)
        {
            if (CollateralId == null) return null;


            var evaluationId = await _cbeContext.PCEEvaluations
                            .Where(r => r.PCEId == CollateralId)
                            .Select(r => r.Id) // Select only the PCEId
                            .FirstOrDefaultAsync();

            if (evaluationId != Guid.Empty)
            {

                var uploadFiles = await _cbeContext.UploadFiles
                    .Where(res => res.CollateralId == CollateralId || res.CollateralId == evaluationId)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<ReturnFileDto>>(uploadFiles);

            }
            else
            {

                var uploadFiles = await _cbeContext.UploadFiles
                    .Where(res => res.CollateralId == CollateralId)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<ReturnFileDto>>(uploadFiles);

            }
        }

        public async Task<IEnumerable<ReturnFileDto>> GetMoUploadFile(Guid? CaseId)
        {
            if (CaseId == null) return null;

            var uploadFiles = await _cbeContext.UploadFiles
                .Where(res => res.CaseId == CaseId && res.CollateralId == null)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ReturnFileDto>>(uploadFiles);


        }

        public async Task<IEnumerable<ReturnPCEReportFileDto>> GetAllUploadFileByCaseId(Guid? CollateralId)
        {
            if (CollateralId == null) return null;

            var uploadFiles = await _cbeContext.UploadFiles.Where(res => res.CaseId == CollateralId).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnPCEReportFileDto>>(uploadFiles);
        }



        //public async Task<ReturnPCEReportFileDto> GetSignatureByEmployeeId(string? CollateralId)
        //{
        //    if (CollateralId == null) return null;

        //    var signaturefilename = _cbeContext.Signatures.Where(c => c.Emp_Id == CollateralId).Select(c => c.SignatureFileId).FirstOrDefault();

        //    var uploadFiles = await _cbeContext.UploadFiles.Where(res => res.CaseId == CollateralId).ToListAsync();
        //    return _mapper.Map<ReturnPCEReportFileDto>(uploadFiles);
        //}



        public async Task<ReturnFileDto> UpdateFile(Guid Id, CreateFileDto file)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uploadFile = await _cbeContext.UploadFiles.FindAsync(Id);
            if (uploadFile == null)
            {
                return null;
            }
            Directory.CreateDirectory("UploadFile");
            
            DeleteFile(Path.Combine("UploadFile", uploadFile.Id.ToString() + uploadFile.Extension));
            uploadFile.Name = file.File.FileName;
            uploadFile.ContentType = file.File.ContentType;
            uploadFile.Size = file.File.Length;
            uploadFile.Extension = Path.GetExtension(file.File.FileName);

            var uniqueFileName = uploadFile.Id.ToString() + uploadFile.Extension;
            var filePath = Path.Combine("UploadFile", uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.File.CopyTo(fileStream);
            }
            uploadFile.Category = file.Category;
            uploadFile.Path = filePath;
            uploadFile.CaseId = file.CaseId;
            uploadFile.CollateralId = file.CollateralId;
            uploadFile.UploadDateTime = DateTime.UtcNow;
            uploadFile.userId = Guid.Parse(httpContext.Session.GetString("userId"));

            _cbeContext.Update(uploadFile);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ReturnFileDto>(uploadFile);
        }

        public async Task<bool> DeleteFile(Guid Id)
        {
            var uploadFile = await _cbeContext.UploadFiles.FindAsync(Id);
            if (uploadFile == null)
            {
                return false;
            }
            DeleteFile(Path.Combine("UploadFile", uploadFile.Id.ToString() + uploadFile.Extension));
            _cbeContext.UploadFiles.Remove(uploadFile);
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
