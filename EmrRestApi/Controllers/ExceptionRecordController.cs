using Microsoft.AspNetCore.Mvc;
using ShareResource.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmrRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionRecordController : ControllerBase
    {
        // GET: api/<ExceptionRecordController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ExceptionRecordController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ExceptionRecordController>
        [HttpPost]
        public void Post([FromBody] List<ExceptionRecord> exceptionRecord)
        {
            exceptionRecord.ForEach(x =>
            {
                Console.WriteLine($"{x.Exception}");
            });
        }

        // PUT api/<ExceptionRecordController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExceptionRecordController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
