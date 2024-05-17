namespace Photobox.Lib.Models;

public class PicturesModel(List<string> pictures)
{
    List<string> Pictures { get; set; } = pictures;
}
