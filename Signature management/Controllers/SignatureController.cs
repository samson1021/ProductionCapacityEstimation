
using Microsoft.AspNetCore.Mvc;
using Signature_management.Data;
using Signature_management.Service.AcknowledgementService;
using Signature_management.Service.SignatureService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Signature_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly ISignatureService _signatureService;
        private readonly MyDbContext _myDbContext;        
         public SignatureController(ISignatureService signatureService, MyDbContext myDbContext)
        {
            _signatureService = signatureService;
            _myDbContext = myDbContext;
        }

        // GET: api/<SignatureController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SignatureController>/5
        [HttpGet("{id}")]
        public IActionResult GetSignature(string id)
        {
            string signature = "";
            if (id!=null)
            {
                 signature= _signatureService.GetSignature(id);
            }
            return Ok(signature);
        }

        // POST api/<SignatureController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SignatureController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SignatureController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
