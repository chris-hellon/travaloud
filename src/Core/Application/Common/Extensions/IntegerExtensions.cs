namespace Travaloud.Application.Common.Extensions;

public static class IntegerExtensions
{
    public static int ConvertToCents(this decimal dollars)
    {
        return (int)(dollars * 100);
    }
    
    public static decimal ConvertToDollars(this long? cents)
    {
        if (cents != null) return cents.Value / 100m;

        return 0;
    }
}