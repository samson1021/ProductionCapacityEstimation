using mechanical.Models.Dto.ProductionUploadFileDto;
using mechanical.Models.Dto.UploadFileDto;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Services.ProductionUploadFileService
{
    public interface IProductionUploadFileService
    {
        Task<Guid> CreateProductionUploadFile(Guid userId, CreateProductionFileDto file);
        Task<ReturnProductionFileDto> GetProductionUploadFile(Guid? uploadFileId);
        Task<IEnumerable<ReturnProductionFileDto>> GetUploadFileByProductionCapacityId(Guid? ProductionCapacityId);
        Task<ActionResult> DownloadProductionFile(Guid Id);
        Task<(byte[], string, string)> ViewProductionFile(Guid Id);
        Task<bool> DeleteProductionFile(Guid Id);
        Task<ReturnProductionFileDto> UpdateProductionFile(Guid Id, CreateProductionFileDto file);
    }
}
