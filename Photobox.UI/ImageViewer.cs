using Photobox.LocalServer.RestApi.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Photobox.UI;
internal class ImageViewer(IPhotoboxApi photoboxApi) : IImageViewer
{
    private readonly IPhotoboxApi photoboxApi = photoboxApi;

    public async Task ShowImage(string imagePath)
    {
        var window = new ImageViewWindow(imagePath, photoboxApi);

        var tcs = new TaskCompletionSource<bool>();

        window.Closed += (o, e) => tcs.SetResult(true);

        window.Show();

        await tcs.Task;
    }
}
