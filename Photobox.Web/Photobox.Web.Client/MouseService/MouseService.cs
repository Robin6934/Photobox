using Microsoft.AspNetCore.Components.Web;

namespace Photobox.Web.Client.MouseService;

// use MouseService to fire events
public class MouseService : IMouseService {
    public event EventHandler<MouseEventArgs>? OnMove;
    public event EventHandler<MouseEventArgs>? OnUp;
    
    public void FireMove(object obj, MouseEventArgs evt) 
        => OnMove?.Invoke(obj, evt);
    
    public void FireUp(object obj, MouseEventArgs evt)
        => OnUp?.Invoke(obj, evt);
}