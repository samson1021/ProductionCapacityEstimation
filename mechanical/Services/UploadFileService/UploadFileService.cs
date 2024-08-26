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
            uploadFile.Catagory = file.Catagory;
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
        public async Task <ActionResult>DownloadFile(Guid Id)
        {
            var uploadFile = await _cbeContext.UploadFiles.FindAsync(Id);
            var filePath = Path.Combine("UploadFile", Id.ToString() + uploadFile.Extension);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return  new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = uploadFile.Name };
        }
        public async Task<(byte[] , string, string)> ViewFile(Guid Id)
        {
            var uploadFile = await _cbeContext.UploadFiles.FindAsync(Id);
            var filePath = Path.Combine("UploadFile", Id.ToString() + uploadFile.Extension);

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

        public async Task<IEnumerable<ReturnFileDto>> GetUploadFileByCollateralId(Guid? CollateralId)
        {
            if (CollateralId == null) return null;

            var uploadFiles = await _cbeContext.UploadFiles.Where(res=>res.CollateralId == CollateralId).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnFileDto>>(uploadFiles);
        }



        public async Task<IEnumerable<ReturnPCEReportFileDto>> GetAllUploadFileByCaseId(Guid? CollateralId)
        {
            if (CollateralId == null) return null;

            var uploadFiles = await _cbeContext.UploadFiles.Where(res => res.CaseId == CollateralId).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnPCEReportFileDto>>(uploadFiles);
        }




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
            uploadFile.Catagory = file.Catagory;
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
