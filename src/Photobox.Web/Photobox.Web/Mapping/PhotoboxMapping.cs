using Photobox.Web.Models;
using Photobox.Web.Requests;
using Photobox.Web.Responses;

namespace Photobox.Web.Mapping;

public static class PhotoboxMapping
{
    public static PhotoBox MapToPhotobox(
        this RegisterPhotoboxRequest request,
        ApplicationUser user,
        string hardwareId
    )
    {
        return new PhotoBox
        {
            Id = Guid.CreateVersion7(),
            ApplicationUser = user,
            Name = request.PhotoBoxName,
            HardwareId = hardwareId,
        };
    }

    public static RegisterPhotoBoxResponse MapToResponse(this PhotoBox photobox)
    {
        return new RegisterPhotoBoxResponse
        {
            HardwareId = photobox.HardwareId,
            PhotoBoxName = photobox.Name,
        };
    }
}
