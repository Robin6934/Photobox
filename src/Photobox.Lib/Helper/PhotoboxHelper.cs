using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using DeviceId;
using MethodTimer;
using Photobox.Lib.RestApi;

namespace Photobox.Lib.Helper;

public static class PhotoboxHelper
{
    [field: AllowNull, MaybeNull]
    public static string PhotoboxId
    {
        get
        {
            if (string.IsNullOrEmpty(field))
            {
                field = new DeviceIdBuilder()
                    .AddMacAddress()
                    .OnWindows(windows => windows.AddProcessorId().AddMotherboardSerialNumber())
                    .ToString();
            }

            return field;
        }
    }

    private static readonly char[] _allowedChars =
    {
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9',
    };

    public static string GenerateGalleryId()
    {
        ReadOnlySpan<char> span = _allowedChars.AsSpan();

        return new string(RandomNumberGenerator.GetItems(span, 6));
    }
}
