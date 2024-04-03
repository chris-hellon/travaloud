namespace Travaloud.Application.Common.Extensions;

public static class IntegerExtensions
{
    public static int ConvertToCents(this decimal dollars)
    {
        return (int)(dollars * 100);
    }
}