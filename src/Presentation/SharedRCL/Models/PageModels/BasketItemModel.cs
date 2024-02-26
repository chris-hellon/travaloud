using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public class BasketItemModel
{
	public TourDetailsDto Tour { get; set; }
	public IList<BasketItemDateModel> TourDates { get; set; }

	public BasketItemModel()
	{
	}

	public BasketItemModel(TourDetailsDto tour, IList<BasketItemDateModel> tourDates)
	{
		Tour = tour;
		TourDates = tourDates;
	}
}

public class BasketItemDateModel
{
	public Guid Id { get; set; }
	public TourDateDto TourDate { get; set; }
	public string TourName { get; set; }
	public string TourImagePath { get; set; }
	public int GuestQuantity { get; set; }
	public bool IsConfirmationPage { get; set; } = false;

	public BasketItemDateModel(TourDateDto tourDate, int guestQuantity, string tourName, string tourImagePath)
	{
		TourDate = tourDate;
		Id = tourDate.Id;
		GuestQuantity = guestQuantity;
		TourName = tourName;
		TourImagePath = tourImagePath;
	}
}