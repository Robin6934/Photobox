using Photobox.Web.Dtos.Requests;
using Photobox.Web.Dtos.Responses;
using Photobox.Web.Models;

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
