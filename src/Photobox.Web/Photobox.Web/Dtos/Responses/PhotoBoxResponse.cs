namespace Photobox.Web.Dtos.Responses;

public class PhotoBoxResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string HardwareId { get; set; }
    public string? CurrentEventCode { get; set; }
    public string? CurrentEventName { get; set; }
}
