using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Photobox.Web.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class PictureController : Controller
{
    public IActionResult UploadPicture()
    {
        return null;
    }
}
