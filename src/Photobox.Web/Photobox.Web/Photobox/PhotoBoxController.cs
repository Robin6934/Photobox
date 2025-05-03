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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="registerPhotoBox"></param>
    /// <param name="photoBoxId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> Register(
        RegisterPhotoBoxDto registerPhotoBox,
        [FromHeader(Name = "X-PhotoBox-Id")] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user = await dbContext
            .Users.Include(e => e.PhotoBoxes)
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("User with id {UserId} not found", userId);
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: $"User with ID '{userId}' was not found."
            );
        }

        if (user.PhotoBoxes.Any(e => e.PhotoboxId == photoBoxId))
        {
            logger.LogInformation(
                "Photobox with id {PhotoBoxId} already exists for user {UserId}.",
                photoBoxId,
                userId
            );
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: $"Photobox with ID '{photoBoxId}' is already registered for this user."
            );
        }

        var photobox = new PhotoBoxModel
        {
            PhotoboxId = photoBoxId,
            Name = registerPhotoBox.PhotoBoxName,
            ApplicationUser = user,
        };

        await dbContext.PhotoBoxModels.AddAsync(photobox, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Photobox with id: {PhotoBoxId} created.", photobox.PhotoboxId);

        return Ok(new { photobox.PhotoboxId, photobox.Name });
    }
    
    public async Task<IActionResult> GetGalleryCode(
        [FromHeader(Name = "X-PhotoBox-Id")] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        var photobox = await dbContext.PhotoBoxModels.FirstOrDefaultAsync(
            e => e.PhotoboxId == photoBoxId,
            cancellationToken
        );

        if (photobox is null)
        {
            return NotFound();
        }

        return Ok();
    }

    /// <summary>
    /// Checks if a photobox with the specified ID exists in the database.
    /// </summary>
    /// <param name="photoBoxId">The ID of the photobox to check.</param>
    /// <param name="cancellationToken">The cancellationToken passed in by the runtime.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    /// <response code="200">Photobox exists in the database.</response>
    /// <response code="404">Photobox not found in the database.</response>
    [HttpGet]
    [ProducesResponseType<CheckPhotoboxResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckIfPhotoboxExists(
        [FromHeader(Name = "X-PhotoBox-Id")] string photoBoxId,
        CancellationToken cancellationToken
    )
    {
        var response = new CheckPhotoboxResponse { PhotoboxId = photoBoxId };

        var photoBox = await dbContext.PhotoBoxModels.FirstOrDefaultAsync(
            p => p.PhotoboxId == photoBoxId,
            cancellationToken
        );

        if (photoBox is null)
        {
            logger.LogInformation("Photobox with id {PhotoBoxId} was not found", photoBoxId);
            response.Exists = false;
        }
        else
        {
            logger.LogInformation("Photobox with id {PhotoBoxId} was found.", photoBoxId);
            response.Exists = true;
        }

        return Ok(response);
    }
}
