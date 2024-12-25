using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Photobox.Web.Photobox.DTOs;

namespace Photobox.Web.Photobox;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhotoBoxController : Controller
{
    [HttpGet]
    public IActionResult Register(CreatePhotoBoxDto createPhotoBox)
    {


        return Ok();
    }
}
