using Signature_management.Data;

namespace Signature_management.Service.SignatureService
{
    public class SignatureService:ISignatureService
    {
        public readonly MyDbContext _cbeContext;

        public SignatureService(MyDbContext context)
        {
            _cbeContext = context;
        }
        public string GetSignature(string EmployeeId)
        {
            string signatureBase64data = _cbeContext.signatures.Where(ca => ca.Emp_Id == EmployeeId).Select(ca => ca.SignatureBase64String).FirstOrDefault();


            return signatureBase64data;
        }
    }
}
