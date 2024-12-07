using Microsoft.AspNetCore.Mvc;
using Photobox.Web.Photobox.DTOs;

namespace Photobox.Web.Photobox;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhotoBoxController : Controller
{
    public IActionResult Create(CreatePhotoBoxDto createPhotoBox)
    {
        return Ok();
    }
}
