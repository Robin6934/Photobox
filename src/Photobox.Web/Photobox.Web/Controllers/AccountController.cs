using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Photobox.Web.Models;

namespace Photobox.Web.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(AuthenticationSchemes = "Identity.Bearer")]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    ILogger<AccountController> logger
) : Controller
{
    [HttpPost]
    public async Task<IActionResult> DownloadPersonalData()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();

        var userId = await userManager.GetUserIdAsync(user);
        logger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

        var personalData = new Dictionary<string, string>();
        var personalDataProps = typeof(ApplicationUser)
            .GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
        foreach (var p in personalDataProps)
        {
            personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
        }

        var logins = await userManager.GetLoginsAsync(user);
        foreach (var l in logins)
        {
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        }

        personalData.Add(
            "Authenticator Key",
            (await userManager.GetAuthenticatorKeyAsync(user)) ?? "null"
        );

        var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);
        return File(fileBytes, "application/json", "PersonalData.json");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return NotFound();

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            return BadRequest("Incorrect password.");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

        var userId = await userManager.GetUserIdAsync(user);
        logger.LogInformation("User with ID '{UserId}' deleted their account.", userId);

        return Ok();
    }
}

public record DeleteAccountRequest(string Password);
