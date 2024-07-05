using AutoMapper;
using mechanical.Data;
using mechanical.Models.PCE.Dto.ProductionUploadFileDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mechanical.Models.PCE.Entities;

namespace mechanical.Services.PCE.ProductionUploadFileService
{
    public class ProductionUploadFileService : IProductionUploadFileService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductionUploadFileService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Guid> CreateProductionUploadFile(Guid userId, CreateProductionFileDto file)
        {
            var productionUploadFile = new ProductionUploadFile();
            productionUploadFile.Id = Guid.NewGuid();
            productionUploadFile.Name = file.File.FileName;
            productionUploadFile.ContentType = file.File.ContentType;
            productionUploadFile.Size = file.File.Length;
            productionUploadFile.Extension = Path.GetExtension(file.File.FileName);

            var uniqueFileName = productionUploadFile.Id.ToString() + productionUploadFile.Extension;
            var filePath = Path.Combine("ProductionUploadFile", uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.File.CopyTo(fileStream);
            }
            productionUploadFile.Catagory = file.Catagory;
            productionUploadFile.Path = filePath;
            productionUploadFile.PCECaseId = file.PCECaseId;
            productionUploadFile.ProductionCapacityId = file.ProductionCapacityId;
            productionUploadFile.UploadDateTime = DateTime.Now;
            productionUploadFile.userId = userId;
            await _cbeContext.ProductionUploadFiles.AddAsync(productionUploadFile);
            await _cbeContext.SaveChangesAsync();
            return productionUploadFile.Id;
        }

        public async Task<bool> DeleteProductionFile(Guid Id)
        {
            var productionuploadFile = await _cbeContext.ProductionUploadFiles.FindAsync(Id);
            if (productionuploadFile == null)
            {
                return false;
            }
            DeleteFile(Path.Combine("ProductionUploadFile", productionuploadFile.Id.ToString() + productionuploadFile.Extension));
            _cbeContext.ProductionUploadFiles.Remove(productionuploadFile);
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
    

        public async Task<ActionResult> DownloadProductionFile(Guid Id)
        {
            var productionuploadFile = await _cbeContext.ProductionUploadFiles.FindAsync(Id);
            var filePath = Path.Combine("ProductionUploadFile", Id.ToString() + productionuploadFile.Extension);
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return new FileContentResult(fileBytes, "application/octet-stream") { FileDownloadName = productionuploadFile.Name };
        }

        public async Task<ReturnProductionFileDto> GetProductionUploadFile(Guid? Id)
        {
            if (Id == null) return null;

            var uploadFile = await _cbeContext.ProductionUploadFiles.FindAsync(Id);
            return _mapper.Map<ReturnProductionFileDto>(uploadFile);

        }

        public async Task<IEnumerable<ReturnProductionFileDto>> GetUploadFileByProductionCapacityId(Guid? ProductionCapacityId)
        {
            if (ProductionCapacityId == null) return null;

            var productionuploadFiles = await _cbeContext.ProductionUploadFiles.Where(res => res.ProductionCapacityId == ProductionCapacityId).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionFileDto>>(productionuploadFiles);

        }

        public async Task<ReturnProductionFileDto> UpdateProductionFile(Guid Id, CreateProductionFileDto file)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var productionuploadFile = await _cbeContext.ProductionUploadFiles.FindAsync(Id);
            if (productionuploadFile == null)
            {
                return null;
            }
            DeleteFile(Path.Combine("ProductionUploadFile", productionuploadFile.Id.ToString() + productionuploadFile.Extension));
            productionuploadFile.Name = file.File.FileName;
            productionuploadFile.ContentType = file.File.ContentType;
            productionuploadFile.Size = file.File.Length;
            productionuploadFile.Extension = Path.GetExtension(file.File.FileName);

            var uniqueFileName = productionuploadFile.Id.ToString() + productionuploadFile.Extension;
            var filePath = Path.Combine("ProductionUploadFile", uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.File.CopyTo(fileStream);
            }
            productionuploadFile.Catagory = file.Catagory;
            productionuploadFile.Path = filePath;
            productionuploadFile.PCECaseId = file.PCECaseId;
            productionuploadFile.ProductionCapacityId = file.ProductionCapacityId;
            productionuploadFile.UploadDateTime = DateTime.Now;
            productionuploadFile.userId = Guid.Parse(httpContext.Session.GetString("userId"));

            _cbeContext.Update(productionuploadFile);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ReturnProductionFileDto>(productionuploadFile);
        }

        public async Task<(byte[], string, string)> ViewProductionFile(Guid Id)
        {
            var productionuploadFile = await _cbeContext.ProductionUploadFiles.FindAsync(Id);
            var filePath = Path.Combine("ProductionUploadFile", Id.ToString() + productionuploadFile.Extension);

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
