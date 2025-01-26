using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photobox.Web.DbContext;
using Photobox.Web.Models;
using Photobox.Web.Photobox.DTOs;
using System.Security.Claims;

namespace Photobox.Web.Photobox;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhotoBoxController(
    ILogger<PhotoBoxController> logger,
    SignInManager<ApplicationUser> signInManager,
    AppDbContext dbContext) : Controller
{
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePhotoBoxDto createPhotoBox)
    {
        string userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var user = await dbContext.Users.FindAsync(userId);
        
        var photobox = new PhotoBoxModel()
        {
            PhotoboxId = createPhotoBox.PhotoBoxId,
            Name = createPhotoBox.PhotoBoxName,
            ApplicationUser = user
        };
        
        return Ok(user);
    }
    
    //https://github.com/dotnet/aspnetcore/blob/9388d498aae571b9575e8252ecc51b54b2b44e22/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs#L90
    [HttpPost]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login(LoginPhotoboxDto loginDto)
    {
        signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        
        var result = await signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
    
        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }
        
        return TypedResults.Empty;
    }
    
    
    /// <summary>
    /// Checks if a photobox with the specified ID exists in the database.
    /// </summary>
    /// <param name="photoBoxId">The ID of the photobox to check.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    /// <response code="200">Photobox exists in the database.</response>
    /// <response code="404">Photobox not found in the database.</response>
    [HttpGet("{photoBoxId}")]
    [Authorize(AuthenticationSchemes = "Identity.Bearer")]
    public async Task<IActionResult> CheckIfPhotoboxExists(string photoBoxId)
    {
        var photoBox = await dbContext.PhotoBoxModels.FirstOrDefaultAsync(p => p.PhotoboxId == photoBoxId);

        if (photoBox == null)
        {
            return NotFound();
        }

        return Ok();
    }

}
