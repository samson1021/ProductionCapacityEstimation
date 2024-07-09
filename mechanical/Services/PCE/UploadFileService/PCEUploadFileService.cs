

using AutoMapper;
using mechanical.Data;
using mechanical.Models.PCE.Dto.PCEUploadFileDto;
using mechanical.Models.PCE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.PCE.UploadFileService
{
    public class PCEUploadFileService: IPCEUploadFileService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;

        public PCEUploadFileService( CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }


        public async Task<Guid> CreateUploadFile(Guid userId, PCECreateFileDto file)
        {
            var uploadFile = new PCEUploadFile();
            uploadFile.Id = Guid.NewGuid();
            uploadFile.Name = file.File.FileName;
            uploadFile.ContentType = file.File.ContentType;
            uploadFile.Size = file.File.Length;
            uploadFile.Extension = Path.GetExtension(file.File.FileName);

            var uniqueFileName = uploadFile.Id.ToString() + uploadFile.Extension;
            var filePath = Path.Combine("PCEUploadFile", uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.File.CopyTo(fileStream);
            }
            uploadFile.Catagory = file.Catagory;
            uploadFile.Path = filePath;
            uploadFile.PCECaseId = file.CaseId;
            uploadFile.PlantCapacityEstimationId = file.PlantCapacityEstimationId;
            uploadFile.UploadDateTime = DateTime.Now;
            uploadFile.userId = userId;
            try
            {
                await _cbeContext.PCEUploadFiles.AddAsync(uploadFile);
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception
                Console.WriteLine("Error occurred: " + ex.Message);
                Console.WriteLine("Inner exception: " + ex.InnerException?.Message);
            }
            return uploadFile.Id;
        }



        public async Task<IEnumerable<PCEReturnFileDto>> GetUploadFileByCollateralId(Guid? CollateralId)
        {
            if (CollateralId == null) return null;
            var pceuploadFiles = await _cbeContext.PCEUploadFiles.Where(res => res.PlantCapacityEstimationId == CollateralId).ToListAsync();
            return _mapper.Map<IEnumerable<PCEReturnFileDto>>(pceuploadFiles);
        }




        public async Task<bool> DeleteFile(Guid Id)
        {
            var uploadFile = await _cbeContext.PCEUploadFiles.FindAsync(Id);
            if (uploadFile == null)
            {
                return false;
            }
            DeleteFile(Path.Combine("PCEUploadFile", uploadFile.Id.ToString() + uploadFile.Extension));
            _cbeContext.PCEUploadFiles.Remove(uploadFile);
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


        public async Task<ActionResult> DownloadFile(Guid Id)
        {
            var uploadFile = await _cbeContext.PCEUploadFiles.FindAsync(Id);
            var filePath = Path.Combine("PCEUploadFile", Id.ToString() + uploadFile.Extension);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = uploadFile.Name };
        }


        public async Task<(byte[], string, string)> ViewFile(Guid Id)
        {
            var uploadFile = await _cbeContext.PCEUploadFiles.FindAsync(Id);
            var filePath = Path.Combine("PCEUploadFile", Id.ToString() + uploadFile.Extension);

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



    }
}
