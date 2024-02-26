using Microsoft.AspNetCore.Http;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.PageModels;

public class BasketModel
{
	public IList<BasketItemModel> Items { get; set; } = new List<BasketItemModel>();
	public decimal Total { get; set; } = 0M;
	public string PromoCode { get; set; }

	public BasketModel()
	{
	}

	public void AddItem(BasketItemModel item)
	{
		Items.Add(item);
	}

	public void AddItem(TourDetailsDto tour, TourDateDto? tourDate, int guestQuantity, ISession session)
	{
		if (tourDate == null) return;
		
		var item = this.Items.FirstOrDefault(x => x.Tour.Id == tour.Id);

		if (item == null)
		{
			this.AddItem(new BasketItemModel(tour, new List<BasketItemDateModel>()
			{
				new(tourDate, guestQuantity, tour.Name, tour.ImagePath)
			}));
		}
		else
		{
			var itemTourDate = item.TourDates.FirstOrDefault(x => x.TourDate.Id == tourDate.Id);

			if (itemTourDate != null)
				itemTourDate.GuestQuantity += guestQuantity;
			else
				item.TourDates.Add(new BasketItemDateModel(tourDate, guestQuantity, tour.Name, tour.ImagePath));
		}

		CalculateTotal();
		session.UpdateObjectInSession<BasketModel>("tourBookingBasket", this);
	}

	public void CalculateTotal()
	{
		Total = 0M;

		if (!Items.Any()) return;
		
		foreach (var item in Items)
		{
			if (!item.TourDates.Any()) continue;
			
			foreach (var tourDate in item.TourDates)
			{
				if (tourDate.TourDate.TourPrice != null)
					Total += tourDate.TourDate.TourPrice.Price * tourDate.GuestQuantity;
			}
		}
	}
}