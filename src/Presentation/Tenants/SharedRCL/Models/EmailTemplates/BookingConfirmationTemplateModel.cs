using Travaloud.Application.Basket.Dto;
using Travaloud.Application.Catalog.Bookings.Dto;

namespace Travaloud.Tenants.SharedRCL.Models.EmailTemplates;

public class BookingConfirmationTemplateModel : EmailTemplateBaseModel
{
    public BasketModel? Basket { get; set; }
    public BookingDetailsDto? Booking { get; set; }
    public int? InvoiceId { get; set; }
    public string? ContactUrl { get; set; }
    public string? CustomerEmail { get; set; }

    public BookingConfirmationTemplateModel(
        string tenantName,
        string? primaryBackgroundColor,
        string? secondaryBackgroundColor,
        string? headerBackgroundColor,
        string? textColor,
        string? logoImageUrl,
        BasketModel basket,
        int? invoiceId, 
        string? contactUrl) : base(tenantName,
        primaryBackgroundColor,
        secondaryBackgroundColor,
        headerBackgroundColor,
        textColor,
        logoImageUrl)
    {
        Basket = basket;
        InvoiceId = invoiceId;
        ContactUrl = contactUrl;
    }
    
    public BookingConfirmationTemplateModel(
        string tenantName,
        string? primaryBackgroundColor,
        string? secondaryBackgroundColor,
        string? headerBackgroundColor,
        string? textColor,
        string? logoImageUrl,
        BookingDetailsDto booking,
        int? invoiceId, 
        string? contactUrl,
        string? customerEmail) : base(tenantName,
        primaryBackgroundColor,
        secondaryBackgroundColor,
        headerBackgroundColor,
        textColor,
        logoImageUrl)
    {
        Booking = booking;
        InvoiceId = invoiceId;
        ContactUrl = contactUrl;
        CustomerEmail = customerEmail;
    }
}