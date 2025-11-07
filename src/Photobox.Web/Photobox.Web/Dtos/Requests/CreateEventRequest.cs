namespace Photobox.Web.Dtos.Requests;

public record CreateEventRequest
{
    public required string Name { get; set; }

    public required DateTime StartDate { get; set; }

    public required string PhotoboxHardwareId { get; set; }
}
