using EOSDigital.SDK;

namespace EOSDigital.API
{
    /// <summary>
    /// Stores CameraValues and provides methods to get those values. Abstract class.
    /// </summary>
    public abstract class ValueBase
    {
        /// <summary>
        /// Get the value from an int out of given possible values.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of the value to get</param>
        /// <param name="Values">Possible values that will be searched for the given value</param>
        /// <returns>The CameraValue with given uint representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(int value, List<CameraValue> Values)
        {
            var arr = Values.Where(t => t.IntValue == value).ToArray();
            if (arr.Length == 0)
            {
                var invalid = Values.FirstOrDefault(t => t.IntValue == unchecked((int)0xFFFFFFFF));
                if (invalid is not null)
                    return invalid;
                else
                    throw new KeyNotFoundException("There is no CameraValue for this ID");
            }
            else
            {
                return arr[0];
            }
        }

        /// <summary>
        /// Get the value from a string out of given possible values.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of value to get</param>
        /// <param name="Values">Possible values that will be searched for the given value</param>
        /// <returns>The CameraValue with given string representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(string value, List<CameraValue> Values)
        {
            var arr = Values.Where(t => t.StringValue == value).ToArray();
            if (arr.Length == 0)
            {
                var invalid = Values.FirstOrDefault(t => t.IntValue == unchecked((int)0xFFFFFFFF));
                if (invalid is not null)
                    return invalid;
                else
                    throw new KeyNotFoundException("There is no CameraValue for this ID");
            }
            else
            {
                return arr[0];
            }
        }

        /// <summary>
        /// Get the value from a double out of given possible values.
        /// It searches for the closest representation and therefore might not return the exact input value.
        /// </summary>
        /// <param name="value">The value to get</param>
        /// <param name="Values">Possible values that will be searched for the given value</param>
        /// <returns>The CameraValue with given double representation</returns>
        public static CameraValue GetValue(double value, List<CameraValue> Values)
        {
            CameraValue[] sorted =
            [
                .. Values
                    .Distinct(new CameraValueComparer())
                    .Where(t =>
                        t.IntValue != unchecked((int)0xFFFFFFFF)
                        && t != TvValues.Bulb
                        && t != ISOValues.Auto
                    )
                    .OrderBy(t => t.DoubleValue),
            ];

            for (int i = 0; i < sorted.Length; i++)
            {
                //Exact match:
                if (Math.Abs(sorted[i].DoubleValue - value) <= 0.00000000001)
                    return sorted[i];
                else if (sorted[i].DoubleValue > value)
                {
                    //Value is smaller than the range of given list. Return first:
                    if (i == 0)
                        return sorted[i];
                    else
                    {
                        //Select CameraValue closest to given value
                        double delta1 = value - sorted[i - 1].DoubleValue;
                        double delta = sorted[i].DoubleValue - value;
                        if (delta > delta1)
                            return sorted[i - 1];
                        else
                            return sorted[i];
                    }
                }
            }

            //Value is bigger than the range of given list. Return last:
            return sorted[^1];
        }

        /// <summary>
        /// Comparer for <see cref="CameraValue"/>s
        /// </summary>
        protected sealed class CameraValueComparer : IEqualityComparer<CameraValue>
        {
            /// <summary>
            /// Determines whether the specified <see cref="CameraValue"/>s are equal.
            /// </summary>
            /// <param name="x">The first <see cref="CameraValue"/></param>
            /// <param name="y">The second <see cref="CameraValue"/></param>
            /// <returns>true if the specified <see cref="CameraValue"/>s are equal; otherwise, false.</returns>
            public bool Equals(CameraValue? x, CameraValue? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.Equals(y);
            }

            /// <summary>
            /// Serves as a hash function for a <see cref="CameraValue"/>.
            /// </summary>
            /// <returns>A hash code for the current <see cref="CameraValue"/></returns>
            public int GetHashCode(CameraValue obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                return obj.GetHashCode();
            }
        }
    }

    /// <summary>
    /// Stores Av Values and provides methods to get those values
    /// </summary>
    public sealed class AvValues : ValueBase
    {
        /// <summary>
        /// All values for this property
        /// </summary>
        private static readonly List<CameraValue> values;

        public static CameraValue[] Values => [.. values];

        private static readonly CameraValue cameraValue = new("Auto", 0x00000000, 0, PropertyID.Av);

        /// <summary>
        /// The Av <see cref="CameraValue"/> of the "Auto" or "None" setting
        /// </summary>
        public static readonly CameraValue Auto = cameraValue;

        /// <summary>
        /// The Av <see cref="CameraValue"/> of an invalid setting
        /// </summary>
        public static readonly CameraValue Invalid = new(
            "N/A",
            unchecked((int)0xFFFFFFFF),
            0,
            PropertyID.Av
        );

        static AvValues()
        {
            values =
            [
                Auto,
                new("1", 0x08, 1, PropertyID.Av),
                new("1.1", 0x0B, 1.1, PropertyID.Av),
                new("1.2", 0x0C, 1.2, PropertyID.Av),
                new("1.2 (1/3)", 0x0D, 1.2, PropertyID.Av),
                new("1.4", 0x10, 1.4, PropertyID.Av),
                new("1.6", 0x13, 1.6, PropertyID.Av),
                new("1.8", 0x14, 1.8, PropertyID.Av),
                new("1.8 (1/3)", 0x15, 1.8, PropertyID.Av),
                new("2", 0x18, 2, PropertyID.Av),
                new("2.2", 0x1B, 2.2, PropertyID.Av),
                new("2.5", 0x1C, 2.5, PropertyID.Av),
                new("2.5 (1/3)", 0x1D, 2.5, PropertyID.Av),
                new("2.8", 0x20, 2.8, PropertyID.Av),
                new("3.2", 0x23, 3.2, PropertyID.Av),
                new("3.5", 0x24, 3.5, PropertyID.Av),
                new("3.5 (1/3)", 0x25, 3.5, PropertyID.Av),
                new("4", 0x28, 4, PropertyID.Av),
                new("4.5", 0x2B, 4.5, PropertyID.Av),
                new("4.5 (1/3)", 0x2C, 4.5, PropertyID.Av),
                new("5.0", 0x2D, 5.0, PropertyID.Av),
                new("5.6", 0x30, 5.6, PropertyID.Av),
                new("6.3", 0x33, 6.3, PropertyID.Av),
                new("6.7", 0x34, 6.7, PropertyID.Av),
                new("7.1", 0x35, 7.1, PropertyID.Av),
                new("8", 0x38, 8, PropertyID.Av),
                new("9", 0x3B, 9, PropertyID.Av),
                new("9.5", 0x3C, 9.5, PropertyID.Av),
                new("10", 0x3D, 10, PropertyID.Av),
                new("11", 0x40, 11, PropertyID.Av),
                new("13 (1/3)", 0x43, 13, PropertyID.Av),
                new("13", 0x44, 13, PropertyID.Av),
                new("14", 0x45, 14, PropertyID.Av),
                new("16", 0x48, 16, PropertyID.Av),
                new("18", 0x4B, 18, PropertyID.Av),
                new("19", 0x4C, 19, PropertyID.Av),
                new("20", 0x4D, 20, PropertyID.Av),
                new("22", 0x50, 22, PropertyID.Av),
                new("25", 0x53, 25, PropertyID.Av),
                new("27", 0x54, 27, PropertyID.Av),
                new("29", 0x55, 29, PropertyID.Av),
                new("32", 0x58, 32, PropertyID.Av),
                new("36", 0x5B, 36, PropertyID.Av),
                new("38", 0x5C, 38, PropertyID.Av),
                new("40", 0x5D, 40, PropertyID.Av),
                new("45", 0x60, 45, PropertyID.Av),
                new("51", 0x63, 51, PropertyID.Av),
                new("54", 0x64, 54, PropertyID.Av),
                new("57", 0x65, 57, PropertyID.Av),
                new("64", 0x68, 64, PropertyID.Av),
                new("72", 0x6B, 72, PropertyID.Av),
                new("76", 0x6C, 76, PropertyID.Av),
                new("80", 0x6D, 80, PropertyID.Av),
                new("91", 0x70, 91, PropertyID.Av),
                Invalid,
            ];
        }

        /// <summary>
        /// Get the value from an int.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of the value to get</param>
        /// <returns>The CameraValue with given int representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(int value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a string.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of value to get</param>
        /// <returns>The CameraValue with given string representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(string value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a double.
        /// It searches for the closest representation and therefore might not return the exact input value.
        /// </summary>
        /// <param name="value">The value to get</param>
        /// <returns>The CameraValue with given double representation</returns>
        public static CameraValue GetValue(double value)
        {
            return GetValue(value, values);
        }
    }

    /// <summary>
    /// Stores Tv Values and provides methods to get those values
    /// </summary>
    public sealed class TvValues : ValueBase
    {
        /// <summary>
        /// All values for this property
        /// </summary>
        public static CameraValue[] Values => [.. values];
        private static readonly List<CameraValue> values;

        /// <summary>
        /// The Tv <see cref="CameraValue"/> of the "Auto" setting
        /// </summary>
        public static readonly CameraValue Auto = new("Auto", 0x00000000, 0, PropertyID.Tv);

        /// <summary>
        /// The Tv <see cref="CameraValue"/> of the "Bulb" setting
        /// </summary>
        public static readonly CameraValue Bulb = new("Bulb", 0x0C, 0, PropertyID.Tv);

        /// <summary>
        /// The Tv <see cref="CameraValue"/> of an invalid setting
        /// </summary>
        public static readonly CameraValue Invalid = new(
            "N/A",
            unchecked((int)0xFFFFFFFF),
            0,
            PropertyID.Tv
        );

        static TvValues()
        {
            values =
            [
                Auto,
                Bulb,
                new("30\"", 0x10, 30, PropertyID.Tv),
                new("25\"", 0x13, 25, PropertyID.Tv),
                new("20\"", 0x14, 20, PropertyID.Tv),
                new("20\" (1/3)", 0x15, 20, PropertyID.Tv),
                new("15\"", 0x18, 15, PropertyID.Tv),
                new("13\"", 0x1B, 13, PropertyID.Tv),
                new("10\"", 0x1C, 10, PropertyID.Tv),
                new("10\" (1/3)", 0x1D, 10, PropertyID.Tv),
                new("8\"", 0x20, 8, PropertyID.Tv),
                new("6\" (1/3)", 0x23, 6, PropertyID.Tv),
                new("6\"", 0x24, 6, PropertyID.Tv),
                new("5\"", 0x25, 5, PropertyID.Tv),
                new("4\"", 0x28, 4, PropertyID.Tv),
                new("3\"2", 0x2B, 3.2, PropertyID.Tv),
                new("3\"", 0x2C, 3, PropertyID.Tv),
                new("2\"5", 0x2D, 2.5, PropertyID.Tv),
                new("2\"", 0x30, 2, PropertyID.Tv),
                new("1\"6", 0x33, 1.6, PropertyID.Tv),
                new("1\"5", 0x34, 1.5, PropertyID.Tv),
                new("1\"3", 0x35, 1.3, PropertyID.Tv),
                new("1\"", 0x38, 1, PropertyID.Tv),
                new("0\"8", 0x3B, 0.8, PropertyID.Tv),
                new("0\"7", 0x3C, 0.7, PropertyID.Tv),
                new("0\"6", 0x3D, 0.6, PropertyID.Tv),
                new("0\"5", 0x40, 0.5, PropertyID.Tv),
                new("0\"4", 0x43, 0.4, PropertyID.Tv),
                new("0\"3", 0x44, 0.3, PropertyID.Tv),
                new("0\"3 (1/3)", 0x45, 0.3, PropertyID.Tv),
                new("1/4", 0x48, 1 / 4d, PropertyID.Tv),
                new("1/5", 0x4B, 1 / 5d, PropertyID.Tv),
                new("1/6", 0x4C, 1 / 6d, PropertyID.Tv),
                new("1/6 (1/3)", 0x4D, 1 / 6d, PropertyID.Tv),
                new("1/8", 0x50, 1 / 8d, PropertyID.Tv),
                new("1/10 (1/3)", 0x53, 1 / 10d, PropertyID.Tv),
                new("1/10", 0x54, 1 / 10d, PropertyID.Tv),
                new("1/13", 0x55, 1 / 13d, PropertyID.Tv),
                new("1/15", 0x58, 1 / 15d, PropertyID.Tv),
                new("1/20 (1/3)", 0x5B, 1 / 20d, PropertyID.Tv),
                new("1/20", 0x5C, 1 / 20d, PropertyID.Tv),
                new("1/25", 0x5D, 1 / 25d, PropertyID.Tv),
                new("1/30", 0x60, 1 / 30d, PropertyID.Tv),
                new("1/40", 0x63, 1 / 40d, PropertyID.Tv),
                new("1/45", 0x64, 1 / 45d, PropertyID.Tv),
                new("1/50", 0x65, 1 / 50d, PropertyID.Tv),
                new("1/60", 0x68, 1 / 60d, PropertyID.Tv),
                new("1/80", 0x6B, 1 / 80d, PropertyID.Tv),
                new("1/90", 0x6C, 1 / 90d, PropertyID.Tv),
                new("1/100", 0x6D, 1 / 100d, PropertyID.Tv),
                new("1/125", 0x70, 1 / 125d, PropertyID.Tv),
                new("1/160", 0x73, 1 / 160d, PropertyID.Tv),
                new("1/180", 0x74, 1 / 180d, PropertyID.Tv),
                new("1/200", 0x75, 1 / 200d, PropertyID.Tv),
                new("1/250", 0x78, 1 / 150d, PropertyID.Tv),
                new("1/320", 0x7B, 1 / 320d, PropertyID.Tv),
                new("1/350", 0x7C, 1 / 350d, PropertyID.Tv),
                new("1/400", 0x7D, 1 / 400d, PropertyID.Tv),
                new("1/500", 0x80, 1 / 500d, PropertyID.Tv),
                new("1/640", 0x83, 1 / 640d, PropertyID.Tv),
                new("1/750", 0x84, 1 / 750d, PropertyID.Tv),
                new("1/800", 0x85, 1 / 800d, PropertyID.Tv),
                new("1/1000", 0x88, 1 / 1000d, PropertyID.Tv),
                new("1/1250", 0x8B, 1 / 1250d, PropertyID.Tv),
                new("1/1500", 0x8C, 1 / 1500d, PropertyID.Tv),
                new("1/1600", 0x8D, 1 / 1600d, PropertyID.Tv),
                new("1/2000", 0x90, 1 / 2000d, PropertyID.Tv),
                new("1/2500", 0x93, 1 / 2500d, PropertyID.Tv),
                new("1/3000", 0x94, 1 / 3000d, PropertyID.Tv),
                new("1/3200", 0x95, 1 / 3200d, PropertyID.Tv),
                new("1/4000", 0x98, 1 / 4000d, PropertyID.Tv),
                new("1/5000", 0x9B, 1 / 5000d, PropertyID.Tv),
                new("1/6000", 0x9C, 1 / 6000d, PropertyID.Tv),
                new("1/6400", 0x9D, 1 / 6400d, PropertyID.Tv),
                new("1/8000", 0xA0, 1 / 8000d, PropertyID.Tv),
                Invalid,
            ];
        }

        /// <summary>
        /// Get the value from an int.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of the value to get</param>
        /// <returns>The CameraValue with given int representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(int value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a string.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of value to get</param>
        /// <returns>The CameraValue with given string representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(string value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a double.
        /// It searches for the closest representation and therefore might not return the exact input value.
        /// </summary>
        /// <param name="value">The value to get</param>
        /// <returns>The CameraValue with given double representation</returns>
        public static CameraValue GetValue(double value)
        {
            return GetValue(value, values);
        }
    }

    /// <summary>
    /// Stores ISO Values and provides methods to get those values
    /// </summary>
    public sealed class ISOValues : ValueBase
    {
        /// <summary>
        /// All values for this property
        /// </summary>
        public static CameraValue[] Values => [.. values];
        private static readonly List<CameraValue> values;

        /// <summary>
        /// The ISO <see cref="CameraValue"/> of the "Auto" setting
        /// </summary>
        public static readonly CameraValue Auto = new("ISO Auto", 0x00000000, 0, PropertyID.ISO);

        /// <summary>
        /// The ISO <see cref="CameraValue"/> of an invalid setting
        /// </summary>
        public static readonly CameraValue Invalid = new(
            "N/A",
            unchecked((int)0xFFFFFFFF),
            0,
            PropertyID.ISO
        );

        static ISOValues()
        {
            values =
            [
                Auto,
                new("ISO 50", 0x00000040, 50, PropertyID.ISO),
                new("ISO 100", 0x00000048, 100, PropertyID.ISO),
                new("ISO 125", 0x0000004b, 125, PropertyID.ISO),
                new("ISO 160", 0x0000004d, 160, PropertyID.ISO),
                new("ISO 200", 0x00000050, 200, PropertyID.ISO),
                new("ISO 250", 0x00000053, 250, PropertyID.ISO),
                new("ISO 320", 0x00000055, 320, PropertyID.ISO),
                new("ISO 400", 0x00000058, 400, PropertyID.ISO),
                new("ISO 500", 0x0000005b, 500, PropertyID.ISO),
                new("ISO 640", 0x0000005d, 640, PropertyID.ISO),
                new("ISO 800", 0x00000060, 800, PropertyID.ISO),
                new("ISO 1000", 0x00000063, 1000, PropertyID.ISO),
                new("ISO 1250", 0x00000065, 1250, PropertyID.ISO),
                new("ISO 1600", 0x00000068, 1600, PropertyID.ISO),
                new("ISO 2000", 0x0000006b, 2000, PropertyID.ISO),
                new("ISO 2500", 0x0000006d, 2500, PropertyID.ISO),
                new("ISO 3200", 0x00000070, 3200, PropertyID.ISO),
                new("ISO 4000", 0x00000073, 4000, PropertyID.ISO),
                new("ISO 5000", 0x00000075, 5000, PropertyID.ISO),
                new("ISO 6400", 0x00000078, 6400, PropertyID.ISO),
                new("ISO 8000", 0x0000007b, 8000, PropertyID.ISO),
                new("ISO 10000", 0x0000007d, 10000, PropertyID.ISO),
                new("ISO 12800", 0x00000080, 12800, PropertyID.ISO),
                new("ISO 16000", 0x00000083, 16000, PropertyID.ISO),
                new("ISO 20000", 0x00000085, 20000, PropertyID.ISO),
                new("ISO 25600", 0x00000088, 25600, PropertyID.ISO),
                new("ISO 51200", 0x00000090, 51200, PropertyID.ISO),
                new("ISO 102400", 0x00000098, 102400, PropertyID.ISO),
                Invalid,
            ];
        }

        /// <summary>
        /// Get the value from an int.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of the value to get</param>
        /// <returns>The CameraValue with given int representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(int value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a string.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of value to get</param>
        /// <returns>The CameraValue with given string representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(string value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a double.
        /// It searches for the closest representation and therefore might not return the exact input value.
        /// </summary>
        /// <param name="value">The value to get</param>
        /// <returns>The CameraValue with given double representation</returns>
        public static CameraValue GetValue(double value)
        {
            return GetValue(value, values);
        }
    }

    /// <summary>
    /// Stores Exposure Compensation Values and provides methods to get those values
    /// </summary>
    public sealed class ExpCompValues : ValueBase
    {
        /// <summary>
        /// All values for this property
        /// </summary>
        public static CameraValue[] Values => [.. values];
        private static readonly List<CameraValue> values;

        /// <summary>
        /// The ExposureCompensation <see cref="CameraValue"/> of Zero
        /// </summary>
        public static readonly CameraValue Zero = new(
            "0",
            0x00,
            0,
            PropertyID.ExposureCompensation
        );

        /// <summary>
        /// The ExposureCompensation <see cref="CameraValue"/> of an invalid setting
        /// </summary>
        public static readonly CameraValue Invalid = new(
            "N/A",
            unchecked((int)0xFFFFFFFF),
            0,
            PropertyID.ExposureCompensation
        );

        static ExpCompValues()
        {
            values =
            [
                new("+5", 0x28, 5, PropertyID.ExposureCompensation),
                new("+4 2/3", 0x25, 4 + (2 / 3d), PropertyID.ExposureCompensation),
                new("+4 1/2", 0x24, 4 + (1 / 2d), PropertyID.ExposureCompensation),
                new("+4 1/3", 0x23, 4 + (1 / 3d), PropertyID.ExposureCompensation),
                new("+4", 0x20, 4, PropertyID.ExposureCompensation),
                new("+3 2/3", 0x1D, 3 + (2 / 3d), PropertyID.ExposureCompensation),
                new("+3 1/2", 0x1C, 3 + (1 / 2d), PropertyID.ExposureCompensation),
                new("+3 1/3", 0x1B, 3 + (1 / 3d), PropertyID.ExposureCompensation),
                new("+3", 0x18, 3, PropertyID.ExposureCompensation),
                new("+2 2/3", 0x15, 2 + (2 / 3d), PropertyID.ExposureCompensation),
                new("+2 1/2", 0x14, 2 + (1 / 2d), PropertyID.ExposureCompensation),
                new("+2 1/3", 0x13, 2 + (1 / 3d), PropertyID.ExposureCompensation),
                new("+2", 0x10, 2, PropertyID.ExposureCompensation),
                new("+1 2/3", 0x0D, 1 + (2 / 3d), PropertyID.ExposureCompensation),
                new("+1 1/2", 0x0C, 1 + (1 / 2d), PropertyID.ExposureCompensation),
                new("+1 1/3", 0x0B, 1 + (1 / 3d), PropertyID.ExposureCompensation),
                new("+1", 0x08, 1, PropertyID.ExposureCompensation),
                new("+2/3", 0x05, 2 / 3d, PropertyID.ExposureCompensation),
                new("+1/2", 0x04, 1 / 2d, PropertyID.ExposureCompensation),
                new("+1/3", 0x03, 1 / 3d, PropertyID.ExposureCompensation),
                Zero,
                new("–1/3", 0xFD, -1 / 3d, PropertyID.ExposureCompensation),
                new("–1/2", 0xFC, -1 / 2d, PropertyID.ExposureCompensation),
                new("–2/3", 0xFB, -2 / 3d, PropertyID.ExposureCompensation),
                new("–1", 0xF8, -1, PropertyID.ExposureCompensation),
                new("–1 1/3", 0xF5, -1 - (1 / 3d), PropertyID.ExposureCompensation),
                new("–1 1/2", 0xF4, -1 - (1 / 2d), PropertyID.ExposureCompensation),
                new("–1 2/3", 0xF3, -1 - (2 / 3d), PropertyID.ExposureCompensation),
                new("–2", 0xF0, -2, PropertyID.ExposureCompensation),
                new("–2 1/3", 0xED, -2 - (1 / 3d), PropertyID.ExposureCompensation),
                new("–2 1/2", 0xEC, -2 - (1 / 2d), PropertyID.ExposureCompensation),
                new("–2 2/3", 0xEB, -2 - (2 / 3d), PropertyID.ExposureCompensation),
                new("–3", 0xE8, -3, PropertyID.ExposureCompensation),
                new("-3 1/3", 0xE5, -3 - (1 / 3d), PropertyID.ExposureCompensation),
                new("-3 1/2", 0xE4, -3 - (1 / 2d), PropertyID.ExposureCompensation),
                new("-3 2/3", 0xE3, -3 - (2 / 3d), PropertyID.ExposureCompensation),
                new("-4", 0xE0, -4, PropertyID.ExposureCompensation),
                new("-4 1/3", 0xDD, -4 - (1 / 3d), PropertyID.ExposureCompensation),
                new("-4 1/2", 0xDC, -4 - (1 / 2d), PropertyID.ExposureCompensation),
                new("-4 2/3", 0xDB, -4 - (2 / 3d), PropertyID.ExposureCompensation),
                new("-5", 0xD8, -5, PropertyID.ExposureCompensation),
                Invalid,
            ];
        }

        /// <summary>
        /// Get the value from an int.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of the value to get</param>
        /// <returns>The CameraValue with given int representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(int value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a string.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of value to get</param>
        /// <returns>The CameraValue with given string representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(string value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a double.
        /// It searches for the closest representation and therefore might not return the exact input value.
        /// </summary>
        /// <param name="value">The value to get</param>
        /// <returns>The CameraValue with given double representation</returns>
        public static CameraValue GetValue(double value)
        {
            return GetValue(value, values);
        }
    }

    /// <summary>
    /// Stores AE Mode Values and provides methods to get those values
    /// </summary>
    public sealed class AEModeValues : ValueBase
    {
        /// <summary>
        /// All values for this property
        /// </summary>
        public static CameraValue[] Values => [.. values];
        private static readonly List<CameraValue> values;

        public static readonly CameraValue Program = new("Program", 0, 0, PropertyID.AEMode);
        public static readonly CameraValue Tv = new("Tv", 1, 0, PropertyID.AEMode);
        public static readonly CameraValue Av = new("Av", 2, 0, PropertyID.AEMode);
        public static readonly CameraValue Manual = new("Manual", 3, 0, PropertyID.AEMode);
        public static readonly CameraValue Bulb = new("Bulb", 4, 0, PropertyID.AEMode);
        public static readonly CameraValue A_DEP = new("A_DEP", 5, 0, PropertyID.AEMode);
        public static readonly CameraValue DEP = new("DEP", 6, 0, PropertyID.AEMode);
        public static readonly CameraValue Custom = new("Custom", 7, 0, PropertyID.AEMode);
        public static readonly CameraValue Lock = new("Lock", 8, 0, PropertyID.AEMode);
        public static readonly CameraValue Green = new("Green", 9, 0, PropertyID.AEMode);
        public static readonly CameraValue NightPortrait = new(
            "NightPortrait",
            10,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Sports = new("Sports", 11, 0, PropertyID.AEMode);
        public static readonly CameraValue Portrait = new("Portrait", 12, 0, PropertyID.AEMode);
        public static readonly CameraValue Landscape = new("Landscape", 13, 0, PropertyID.AEMode);
        public static readonly CameraValue Closeup = new("Closeup", 14, 0, PropertyID.AEMode);
        public static readonly CameraValue FlashOff = new("FlashOff", 15, 0, PropertyID.AEMode);
        public static readonly CameraValue Custom2 = new("Custom2", 16, 0, PropertyID.AEMode);
        public static readonly CameraValue Custom3 = new("Custom3", 17, 0, PropertyID.AEMode);
        public static readonly CameraValue CreativeAuto = new(
            "CreativeAuto",
            19,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Movie = new("Movie", 20, 0, PropertyID.AEMode);
        public static readonly CameraValue PhotoInMovie = new(
            "PhotoInMovie",
            21,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue SceneIntelligentAuto = new(
            "SceneIntelligentAuto",
            22,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Scene = new("Scene", 25, 0, PropertyID.AEMode);
        public static readonly CameraValue NightScenes = new(
            "NightScenes",
            23,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue BacklitScenes = new(
            "BacklitScenes",
            24,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Children = new("Children", 26, 0, PropertyID.AEMode);
        public static readonly CameraValue Food = new("Food", 27, 0, PropertyID.AEMode);
        public static readonly CameraValue CandlelightPortraits = new(
            "CandlelightPortraits",
            28,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue CreativeFilter = new(
            "CreativeFilter",
            29,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue RoughMonoChrome = new(
            "RoughMonoChrome",
            30,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue SoftFocus = new("SoftFocus", 31, 0, PropertyID.AEMode);
        public static readonly CameraValue ToyCamera = new("ToyCamera", 32, 0, PropertyID.AEMode);
        public static readonly CameraValue Fisheye = new("Fisheye", 33, 0, PropertyID.AEMode);
        public static readonly CameraValue WaterColor = new("WaterColor", 34, 0, PropertyID.AEMode);
        public static readonly CameraValue Miniature = new("Miniature", 35, 0, PropertyID.AEMode);
        public static readonly CameraValue Hdr_Standard = new(
            "Hdr_Standard",
            36,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Hdr_Vivid = new("Hdr_Vivid", 37, 0, PropertyID.AEMode);
        public static readonly CameraValue Hdr_Bold = new("Hdr_Bold", 38, 0, PropertyID.AEMode);
        public static readonly CameraValue Hdr_Embossed = new(
            "Hdr_Embossed",
            39,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Movie_Fantasy = new(
            "Movie_Fantasy",
            40,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Movie_Old = new("Movie_Old", 41, 0, PropertyID.AEMode);
        public static readonly CameraValue Movie_Memory = new(
            "Movie_Memory",
            42,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Movie_DirectMono = new(
            "Movie_DirectMono",
            43,
            0,
            PropertyID.AEMode
        );
        public static readonly CameraValue Movie_Mini = new("Movie_Mini", 44, 0, PropertyID.AEMode);
        public static readonly CameraValue Unknown = new(
            "Unknown",
            unchecked((int)0xFFFFFFFF),
            0,
            PropertyID.AEMode
        );

        static AEModeValues()
        {
            values =
            [
                Program,
                Tv,
                Av,
                Manual,
                Bulb,
                A_DEP,
                DEP,
                Custom,
                Lock,
                Green,
                NightPortrait,
                Sports,
                Portrait,
                Landscape,
                Closeup,
                FlashOff,
                Custom2,
                Custom3,
                CreativeAuto,
                Movie,
                PhotoInMovie,
                SceneIntelligentAuto,
                Scene,
                NightScenes,
                BacklitScenes,
                Children,
                Food,
                CandlelightPortraits,
                CreativeFilter,
                RoughMonoChrome,
                SoftFocus,
                ToyCamera,
                Fisheye,
                WaterColor,
                Miniature,
                Hdr_Standard,
                Hdr_Vivid,
                Hdr_Bold,
                Hdr_Embossed,
                Movie_Fantasy,
                Movie_Old,
                Movie_Memory,
                Movie_DirectMono,
                Movie_Mini,
                Unknown,
            ];
        }

        /// <summary>
        /// Get the value from an int.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of the value to get</param>
        /// <returns>The CameraValue with given int representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(int value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a string.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of value to get</param>
        /// <returns>The CameraValue with given string representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(string value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a double.
        /// It searches for the closest representation and therefore might not return the exact input value.
        /// </summary>
        /// <param name="value">The value to get</param>
        /// <returns>The CameraValue with given double representation</returns>
        public static CameraValue GetValue(double value)
        {
            return GetValue(value, values);
        }
    }

    /// <summary>
    /// Stores Metering Mode Values and provides methods to get those values
    /// </summary>
    public sealed class MeteringModeValues : ValueBase
    {
        /// <summary>
        /// All values for this property
        /// </summary>
        public static CameraValue[] Values => [.. values];
        private static readonly List<CameraValue> values;

        public static readonly CameraValue Spot = new("Spot", 1, 0, PropertyID.MeteringMode);
        public static readonly CameraValue Evaluative = new(
            "Evaluative",
            3,
            0,
            PropertyID.MeteringMode
        );
        public static readonly CameraValue Partial = new("Partial", 4, 0, PropertyID.MeteringMode);
        public static readonly CameraValue CenterWeightedAveraging = new(
            "Center-weighted averaging",
            5,
            0,
            PropertyID.MeteringMode
        );
        public static readonly CameraValue NotValid = new(
            "Not valid",
            unchecked((int)0xFFFFFFFF),
            0,
            PropertyID.MeteringMode
        );

        static MeteringModeValues()
        {
            values = [Spot, Evaluative, Partial, CenterWeightedAveraging, NotValid];
        }

        /// <summary>
        /// Get the value from an int.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of the value to get</param>
        /// <returns>The CameraValue with given int representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(int value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a string.
        /// It has to be an exact match, otherwise an exception is thrown.
        /// </summary>
        /// <param name="value">The ID of value to get</param>
        /// <returns>The CameraValue with given string representation</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CameraValue"/> for the given value</exception>
        public static CameraValue GetValue(string value)
        {
            return GetValue(value, values);
        }

        /// <summary>
        /// Get the value from a double.
        /// It searches for the closest representation and therefore might not return the exact input value.
        /// </summary>
        /// <param name="value">The value to get</param>
        /// <returns>The CameraValue with given double representation</returns>
        public static CameraValue GetValue(double value)
        {
            return GetValue(value, values);
        }
    }
}
