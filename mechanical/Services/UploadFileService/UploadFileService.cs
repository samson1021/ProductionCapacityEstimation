using AutoMapper;
using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.Dto.UploadFileDto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
            var uploadFile = new UploadFile();
            uploadFile.Id = Guid.NewGuid();
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
            uploadFile.UploadDateTime = DateTime.Now;
            uploadFile.userId = userId;
            await _cbeContext.UploadFiles.AddAsync(uploadFile);
            await _cbeContext.SaveChangesAsync();
            return uploadFile.Id;
        }
        public async Task<ReturnFileDto> GetUploadFile(Guid? Id)
        {
            if(Id == null) return null;

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
            uploadFile.UploadDateTime = DateTime.Now;
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
