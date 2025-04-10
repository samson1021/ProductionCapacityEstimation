using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.SignatureDto;
using mechanical.Models.Dto.SignatureOfEmployeInfo;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace mechanical.Services.SignatureService
{
    public class SignatureService : ISignatureService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IUploadFileService _uploadFileService;
        public SignatureService(CbeContext cbeContext, IMapper mapper, IUploadFileService uploadFileService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _uploadFileService = uploadFileService;
        }

        public async Task<Signatures> CreateSignature(Guid userId, SignatureDto signatureDto)
        {
            var userIdss = _cbeContext.CreateUsers.Where(c => c.Id == userId).Select(c => c.emp_ID).FirstOrDefault();

            var isSignatureExist = await _cbeContext.Signatures.Where(c => c.Emp_Id == userIdss).Select(c => c.Emp_Id).FirstOrDefaultAsync();
             if (isSignatureExist == null)
            {


                var addSignature = _mapper.Map<Signatures>(signatureDto);
                addSignature.Id = Guid.NewGuid();

                var signatureFee = new CreateFileDto()
                {
                    File = signatureDto.File ?? throw new ArgumentNullException(nameof(signatureDto.File)),
                    CaseId = addSignature.Id,
                    Catagory = "Signature"
                };
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    signatureDto.File.CopyTo(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                // Convert the byte array to a Base64 string
                addSignature.SignatureBase64String = Convert.ToBase64String(fileBytes);
                addSignature.Emp_Id = signatureDto.Emp_Id;
                addSignature.SignatureFileId = await _uploadFileService.CreateUploadFile(userId, signatureFee);
                addSignature.CreateUserId = userId;
                addSignature.CreatedBy = userIdss;
                addSignature.CreatedDate = DateTime.Now;

                await _cbeContext.Signatures.AddAsync(addSignature);
                await _cbeContext.SaveChangesAsync();
                return addSignature;
            }
            else
            {
                return null;
            }
        }
        public async Task<IEnumerable<ReturnSignatureDto>> Getsignature(string Id)
        {
            var userId = _cbeContext.CreateUsers.Where(c => c.Id.ToString() == Id).Select(c => c.emp_ID).FirstOrDefault();
            var signatureresponse = await _cbeContext.Signatures.Where(c => c.Emp_Id == userId).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnSignatureDto>>(signatureresponse);
        }
        public async Task<EmployeeInfoes> GetEmployeeName(string emp_Id)
        {
            var employeeInfo = await _cbeContext.Employees.FirstOrDefaultAsync(c => c.emp_ID == emp_Id);
            return _mapper.Map<EmployeeInfoes>(employeeInfo);
        }

    }
}