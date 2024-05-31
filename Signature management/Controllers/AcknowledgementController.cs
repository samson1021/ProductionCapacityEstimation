using Microsoft.AspNetCore.Mvc;
using Signature_management.Data;
using Signature_management.Model.Dto.AcknowledgementDto;
using Signature_management.Service.AcknowledgementService;

namespace Signature_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcknowledgementController : ControllerBase
    {
        private readonly IAcknowledgementService _acknowledgementService;
        private readonly MyDbContext _myDbContext;


        public AcknowledgementController(IAcknowledgementService acknowledgementService, MyDbContext myDbContext)
        {
            _acknowledgementService = acknowledgementService;
            _myDbContext = myDbContext;
        }




        // GET: api/<AcknowledgementController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AcknowledgementController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST
       //[ Route("api/AcknowledgementController")]
        [HttpPost]
        public IActionResult Post( AcknowledgementPostDto acknowledgementPostDto)
        {
           var ReturnData= _acknowledgementService.CreateAcknowledgementLetter(acknowledgementPostDto);

            return Ok(ReturnData);

        }

        // PUT api/<AcknowledgementController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AcknowledgementController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
