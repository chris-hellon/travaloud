namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class CarouselComponent
{
	public string Id { get; init; }  = string.Empty;
	public bool Rounded { get; init; }
	public List<string> Images { get; init; } = default!;
	public string TenantId { get; init; } = string.Empty;

	public CarouselComponent(string id, List<string> images, bool rounded = false)
	{
		Images = images;
		Id = id;
		Rounded = rounded;
	}

	public CarouselComponent()
	{
	}
}