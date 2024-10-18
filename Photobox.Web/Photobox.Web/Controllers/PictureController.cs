using Microsoft.AspNetCore.Mvc;

namespace Photobox.Web.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class PictureController : Controller
{
    [HttpPost]
    public IActionResult UploadPicture(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        // Define the path where you want to save the uploaded file
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        // Ensure the directory exists
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Create a unique file name to avoid overwriting
        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save the file
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            formFile.CopyTo(fileStream);
        }

        return Ok(new { filePath });
    }

    [HttpGet]
    public IActionResult Test(string filePath)
    {
        byte[] b = System.IO.File.ReadAllBytes(filePath);
        return File(b, "image/jpeg");
    }

}
