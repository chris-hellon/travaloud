using Travaloud.Application.Catalog.Images.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class FullImageCarouselComponent
{
	public BookNowComponent BookNowComponent { get; private set; } = null;

	public List<ImageDto> CarouselImages { get; private set; }

	public bool FullCoverMask { get; private set; }
	
	public bool FullCoverOneTitle { get; set; }
	
	public string? Title { get; set; }
	public string? SubTitle { get; set; }

	public FullImageCarouselComponent()
	{

	}

	public FullImageCarouselComponent(List<ImageDto> carouselImages, BookNowComponent? bookNowComponent = null, bool fullCoverMask = false)
	{
		BookNowComponent = bookNowComponent;
		CarouselImages = carouselImages;
		FullCoverMask = fullCoverMask;
	}
}