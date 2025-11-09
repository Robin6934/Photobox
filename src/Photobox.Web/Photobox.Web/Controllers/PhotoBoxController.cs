using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photobox.Lib;
using Photobox.Web.Database;
using Photobox.Web.Dtos.Requests;
using Photobox.Web.Dtos.Responses;
using Photobox.Web.Mapping;
using Photobox.Web.Models;
using Photobox.Web.Services;

namespace Photobox.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Identity.Bearer")]
public class PhotoBoxController(
    ILogger<PhotoBoxController> logger,
    PhotoBoxService photoBoxService,
    AppDbContext dbContext,
    EventService eventService
) : Controller
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="registerPhotoBox"></param>
    /// <param name="hardwareId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType<RegisterPhotoBoxResponse>(StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Register(
        RegisterPhotoboxRequest request,
        [FromHeader(Name = PhotoboxHeaders.HardwareId)] string hardwareId,
        CancellationToken cancellationToken
    )
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var user = await dbContext
            .Users.Include(e => e.PhotoBoxes)
            .SingleAsync(e => e.Id == userId, cancellationToken);

        if (user.PhotoBoxes.Any(e => e.HardwareId == hardwareId))
        {
            logger.LogInformation(
                "Photobox with id {PhotoBoxId} already exists for user {UserId}.",
                hardwareId,
                userId
            );

            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: $"Photobox with ID '{hardwareId}' is already registered for this user."
            );
        }

        var photobox = request.MapToPhotobox(user, hardwareId);

        await photoBoxService.CreateAsync(photobox, cancellationToken);

        await eventService.CreateNewEvent(user, photobox);

        logger.LogInformation(
            "Photobox with hardware id: {PhotoBoxId} created.",
            photobox.HardwareId
        );

        var response = photobox.MapToResponse();

        return CreatedAtAction(
            nameof(Register),
            new { hardwareId = photobox.HardwareId },
            response
        );
    }

    /// <summary>
    /// Checks if a photobox with the specified ID exists in the database.
    /// </summary>
    /// <param name="hardwareId">The ID of the photobox to check.</param>
    /// <param name="cancellationToken">The cancellationToken passed in by the runtime.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    /// <response code="200">Photobox exists in the database.</response>
    /// <response code="404">Photobox not found in the database.</response>
    [HttpGet]
    [ProducesResponseType<CheckPhotoboxResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckIfPhotoboxExists(
        [FromHeader(Name = PhotoboxHeaders.HardwareId)] string hardwareId,
        CancellationToken cancellationToken
    )
    {
        var response = new CheckPhotoboxResponse { PhotoboxId = hardwareId };

        var photoBox = await photoBoxService.GetFromHardwareIdAsync(hardwareId, cancellationToken);

        if (photoBox is null)
        {
            logger.LogInformation("Photobox with id {PhotoBoxId} was not found", hardwareId);
            response.Exists = false;
        }
        else
        {
            logger.LogInformation("Photobox with id {PhotoBoxId} was found.", hardwareId);
            response.Exists = true;
        }

        return Ok(response);
    }
}
