using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhotoboxWeb
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesDownloadController : ControllerBase
    {
        // GET: api/<PicturesDownloadController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET api/<PicturesDownloadController>/5
        [HttpGet("{imageName}")]
        public IActionResult Get(string imageName)
        {
            string filePath = Path.Combine(Program.PhotoBoxDirectory, imageName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/octet-stream", imageName); ;
        }

        // POST api/<PicturesDownloadController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PicturesDownloadController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PicturesDownloadController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
