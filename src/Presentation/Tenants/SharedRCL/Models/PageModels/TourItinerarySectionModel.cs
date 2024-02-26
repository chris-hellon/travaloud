using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public class TourItinerarySectionModel
{
	public TourItinerarySectionDto TourItinerarySectionDto { get; set; }
	public CarouselComponent Carousel { get; set; }

	public TourItinerarySectionModel()
	{
	}

	public TourItinerarySectionModel(TourItinerarySectionDto tourItinerarySectionDto, CarouselComponent carousel)
	{
		TourItinerarySectionDto = tourItinerarySectionDto;
		Carousel = carousel;
	}
}