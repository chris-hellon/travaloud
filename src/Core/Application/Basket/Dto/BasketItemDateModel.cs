using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Basket.Dto;

public class BasketItemDateModel
{
    public DefaultIdType Id { get; set; }
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