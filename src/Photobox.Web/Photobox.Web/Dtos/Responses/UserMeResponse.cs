namespace Photobox.Web.Dtos.Responses;

public record UserMeResponse(Guid Id, string? Email, string? UserName, bool EmailConfirmed);
