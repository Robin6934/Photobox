using Microsoft.AspNetCore.Mvc;

namespace Photobox.Web.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class PictureController : Controller
{
    /// <summary>
    /// Uploads a picture to the server.
    /// </summary>
    /// <param name="formFile">The picture file to upload.</param>
    /// <returns>Returns a 200 OK response if successful or a 400 Bad Request if no file is provided.</returns>
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

        return Ok();
    }

    /// <summary>
    /// Retrieves a list of all uploaded picture file paths.
    /// </summary>
    /// <returns>A list of file paths for all uploaded pictures.</returns>
    [HttpGet]
    public List<string> ListPictures()
    {
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        return Directory.GetFiles(uploadsFolder).ToList();
    }

    /// <summary>
    /// Retrieves a picture from the server based on the provided file path.
    /// </summary>
    /// <param name="filePath">The full file path of the picture to retrieve.</param>
    /// <returns>Returns a FileResult with the picture as a byte array and a content type of image/jpeg.</returns>
    [HttpGet]
    public FileResult Test(string filePath)
    {
        byte[] b = System.IO.File.ReadAllBytes(filePath);
        return File(b, "image/jpeg");
    }
}
