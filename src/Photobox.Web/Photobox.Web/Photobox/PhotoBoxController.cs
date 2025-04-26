using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using Photobox.Lib.Helper;
using Photobox.Web.DbContext;
using Photobox.Web.Models;
using Photobox.Web.Photobox.DTOs;

namespace Photobox.Web.Photobox;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Identity.Bearer")]
public class PhotoBoxController(
    ILogger<PhotoBoxController> logger,
    SignInManager<ApplicationUser> signInManager,
    AppDbContext dbContext
) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePhotoBoxDto createPhotoBox,
        [FromHeader(Name = "X-PhotoBox-Id"), OpenApiIgnore] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user = await dbContext.Users.FindAsync([userId], cancellationToken);

        if (user is null)
        {
            logger.LogInformation("User with id {UserId} not found", userId);
            return Unauthorized();
        }

        var photobox = new PhotoBoxModel
        {
            PhotoboxId = photoBoxId,
            GalleryId = PhotoboxHelper.GenerateGalleryId(),
            Name = createPhotoBox.PhotoBoxName,
            ApplicationUser = user,
        };

        logger.LogInformation("Photobox {PhotoBoxId} created.", photobox.PhotoboxId);

        return Ok(user);
    }

    public async Task<IActionResult> GetGalleryCode(
        [FromHeader(Name = "X-PhotoBox-Id"), OpenApiIgnore] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        return null;
    }

    /// <summary>
    /// Checks if a photobox with the specified ID exists in the database.
    /// </summary>
    /// <param name="photoBoxId">The ID of the photobox to check.</param>
    /// <param name="cancellationToken">The cancellationToken passed in by the runtime.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    /// <response code="200">Photobox exists in the database.</response>
    /// <response code="404">Photobox not found in the database.</response>
    [HttpGet("{photoBoxId}")]
    public async Task<IActionResult> CheckIfPhotoboxExists(
        [FromHeader(Name = "X-PhotoBox-Id"), OpenApiIgnore] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        var photoBox = await dbContext.PhotoBoxModels.FirstOrDefaultAsync(
            p => p.PhotoboxId == photoBoxId,
            cancellationToken
        );

        if (photoBox is null)
        {
            return NotFound();
        }

        return Ok();
    }
}
