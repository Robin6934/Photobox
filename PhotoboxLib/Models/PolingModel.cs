namespace Photobox.Lib.Models;

public class PolingModel(bool triggerPicture, string printPictureName)
{
    public bool TriggerPicture { get; init; } = triggerPicture;

    public string PrintPictureName { get; init; } = printPictureName;
}
