namespace RnD.DelegateRouter.Helpers;

public static class SegmentParsers
{
    public static (bool, object?) ParseBool(string segment)
        => (bool.TryParse(segment, out var result), result);

    public static (bool, object?) ParseInt(string segment)
        => (int.TryParse(segment, out var result), result);

    public static (bool, object?) ParseGuid(string segment)
        => (Guid.TryParse(segment, out var result), result);

    public static (bool, object?) ParseString(string segment)
        => (true, segment);

    public static (bool, object?) ParseDateTime(string segment)
        => (DateTime.TryParse(segment, out var result), result);

    public static (bool, object?) ParseFloat(string segment)
        => (float.TryParse(segment, out var result), result);

    public static (bool, object?) ParseDecimal(string segment)
        => (decimal.TryParse(segment, out var result), result);

    public static (bool, object?) ParseDouble(string segment)
        => (double.TryParse(segment, out var result), result);

    public static (bool, object?) ParseLong(string segment)
        => (long.TryParse(segment, out var result), result);

    public static (bool, object?) ParseTimeSpan(string segment)
        => (TimeSpan.TryParse(segment, out var result), result);
}
