namespace Travaloud.Application.Common.Interfaces;

public class Dto : IDto
{
    public int? SortOrder { get; set; }
}

public interface IDto
{
    int? SortOrder { get; set; }
}