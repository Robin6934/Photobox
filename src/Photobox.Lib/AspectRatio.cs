using System.Diagnostics;

namespace Photobox.Lib;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct AspectRatio
{
    public double Width { get; init; }

    public double Height { get; init; }

    public double Ratio => Width / Height;

    /// <summary>
    /// Represents a aspect ratio, calculates the greatest common divisor to show the ratio in its smallest Form.
    /// </summary>
    /// <param name="width">The width of the Image.</param>
    /// <param name="height">The height of the Image</param>
    public AspectRatio(double width, double height)
    {
        double gcd = Gcd(width, height);
        Width = width / gcd;
        Height = height / gcd;
    }

    private static double Gcd(double a, double b)
    {
        while (double.Abs(b) < 0.00001d)
        {
            double temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public override string ToString() => $"{Width}/{Height}";
}
