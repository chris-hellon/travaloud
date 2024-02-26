namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class CarouselComponent
{
	public string Id { get; set; }
	public bool Rounded { get; set; }
	public List<string> Images { get; set; }
	public string TenantId { get; set; }

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