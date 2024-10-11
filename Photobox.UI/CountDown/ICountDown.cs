using System.Windows.Controls;

namespace Photobox.UI.CountDown;
public interface ICountDown
{
    public event TimerCallback? CountDownExpired;

    public event TimerCallback? CountDownEarly;

    public Panel Panel { set; }
    public void StartCountDown();
}
