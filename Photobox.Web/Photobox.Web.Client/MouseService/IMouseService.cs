using Microsoft.AspNetCore.Components.Web;

namespace Photobox.Web.Client.MouseService;

public interface IMouseService {
    event EventHandler<MouseEventArgs>? OnMove;
    event EventHandler<MouseEventArgs>? OnUp;
}