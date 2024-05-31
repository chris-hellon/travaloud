using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Travaloud.Application.Common.Events;
using Travaloud.Application.Common.Interfaces;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Destinations;
using Travaloud.Domain.Catalog.Enquiries;
using Travaloud.Domain.Catalog.Events;
using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Catalog.Partners;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Services;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.PointOfSale;
using Travaloud.Domain.Stock;
using Travaloud.Infrastructure.Multitenancy;
using Travaloud.Infrastructure.Persistence.Configuration;

namespace Travaloud.Infrastructure.Persistence.Context;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(ITenantInfo currentTenant, DbContextOptions options, ICurrentUser currentUser, ISerializerService serializer, IOptions<DatabaseSettings> dbSettings, IEventPublisher events, IMultiTenantContextAccessor<TravaloudTenantInfo> multiTenantContextAccessor)
        : base(currentTenant, options, currentUser, serializer, dbSettings, events)
    {
        
    }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourCategory> TourCategories => Set<TourCategory>();
    public DbSet<TourDate> TourDates => Set<TourDate>();
    public DbSet<TourItinerary> TourItineraries => Set<TourItinerary>();
    public DbSet<TourCategoryLookup> TourCategoryLookups => Set<TourCategoryLookup>();
    public DbSet<TourDestinationLookup> TourDestinationLookups => Set<TourDestinationLookup>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyDestinationLookup> PropertyDestinationLookups => Set<PropertyDestinationLookup>();
    public DbSet<PropertyDirection> PropertyDirections => Set<PropertyDirection>();
    public DbSet<PropertyDirectionContent> PropertyDirectionContents => Set<PropertyDirectionContent>();
    public DbSet<PropertyFacility> PropertyFacilities => Set<PropertyFacility>();
    public DbSet<PropertyRoom> PropertyRooms => Set<PropertyRoom>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<TourPropertyLookup> TourPropertyLookup => Set<TourPropertyLookup>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();
    //public DbSet<BookingPayment> BookingPayments => Set<BookingPayment>();
    public DbSet<BookingItemRoom> BookingItemRooms => Set<BookingItemRoom>();
    public DbSet<BookingItemGuest> BookingItemGuests => Set<BookingItemGuest>();
    // public DbSet<BookingPackage> BookingPackages => Set<BookingPackage>();
    public DbSet<TourPrice> TourPrices => Set<TourPrice>();
    public DbSet<TourImage> TourImages => Set<TourImage>();
    public DbSet<TourItinerarySection> TourItinerarySections => Set<TourItinerarySection>();
    public DbSet<TourItinerarySectionImage> TourItinerarySectionImages => Set<TourItinerarySectionImage>();
    public DbSet<DestinationTourCategoryLookup> DestinationTourCategoryLookups => Set<DestinationTourCategoryLookup>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceField> ServiceFields => Set<ServiceField>();
    public DbSet<JobVacancy> JobVacancies => Set<JobVacancy>();
    public DbSet<JobVacancyResponse> JobVacancyResponses => Set<JobVacancyResponse>();
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageSorting> PageSortings => Set<PageSorting>();
    public DbSet<PageModal> PageModals => Set<PageModal>();
    public DbSet<PageModalLookup> PageModalLookups => Set<PageModalLookup>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<PartnerContact> PartnerContacts => Set<PartnerContact>();
    public DbSet<TourEnquiry> TourEnquiries => Set<TourEnquiry>();
    public DbSet<ServiceEnquiry> ServiceEnquiries => Set<ServiceEnquiry>();
    public DbSet<ServiceEnquiryField> ServiceEnquiryFields => Set<ServiceEnquiryField>();
    public DbSet<GeneralEnquiry> GeneralEnquiries => Set<GeneralEnquiry>();
    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<TravelGuide> TravelGuides => Set<TravelGuide>();
    public DbSet<TravelGuideGalleryImage> TravelGuideGalleryImages => Set<TravelGuideGalleryImage>();
    public DbSet<TourPickupLocation> TourPickupLocations => Set<TourPickupLocation>();
    
    // #region Stock 
    // public DbSet<Barcode> Barcodes => Set<Barcode>();
    // public DbSet<Category> Categories => Set<Category>();
    // public DbSet<CategoryType> CategoryTypes => Set<CategoryType>();
    // public DbSet<Order> Orders => Set<Order>();
    // public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
    // public DbSet<OrderSupplierNote> OrderSupplierNotes => Set<OrderSupplierNote>();
    // public DbSet<Product> Products => Set<Product>();
    // public DbSet<ProductBarcode> ProductBarcodes => Set<ProductBarcode>();
    // public DbSet<Reason> Reasons => Set<Reason>();
    // public DbSet<Report> Reports => Set<Report>();
    // public DbSet<ReportProduct> ReportProducts => Set<ReportProduct>();
    // public DbSet<Return> Returns => Set<Return>();
    // public DbSet<SupplierOffer> SupplierOffers => Set<SupplierOffer>();
    // public DbSet<SupplierOfferCondition> SupplierOfferConditions => Set<SupplierOfferCondition>();
    // public DbSet<SupplierOfferReward> SupplierOfferRewards => Set<SupplierOfferReward>();
    // public DbSet<SupplierProduct> SupplierProducts => Set<SupplierProduct>();
    // public DbSet<SupplierProperty> SupplierProperties => Set<SupplierProperty>();
    // public DbSet<Transfer> Transfers => Set<Transfer>();
    // public DbSet<Unit> Units => Set<Unit>();
    // public DbSet<Waste> Wastes => Set<Waste>();
    // #endregion
    //
    // #region Point Of Sale
    //
    // public DbSet<Deal> Deals => Set<Deal>();
    // public DbSet<DealPriceTier> DealPriceTiers => Set<DealPriceTier>();
    // public DbSet<FloorPlan> FloorPlans => Set<FloorPlan>();
    // public DbSet<FloorPlanTable> FloorPlanTables => Set<FloorPlanTable>();
    // public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    // public DbSet<MenuItemCategory> MenuItemCategories => Set<MenuItemCategory>();
    // public DbSet<MenuItemCondimentCategory> MenuItemCondimentCategories => Set<MenuItemCondimentCategory>();
    // public DbSet<MenuItemCondimentCategoryMenuItem> MenuItemCondimentCategoryMenuItems => Set<MenuItemCondimentCategoryMenuItem>();
    // public DbSet<MenuItemCondimentCategoryMenuItemModifier> MenuItemCondimentCategoryMenuItemModifiers => Set<MenuItemCondimentCategoryMenuItemModifier>();
    // public DbSet<MenuItemIngredient> MenuItemIngredients => Set<MenuItemIngredient>();
    // public DbSet<MenuItemModifier> MenuItemModifiers => Set<MenuItemModifier>();
    // public DbSet<MenuItemModifierPriceTier> MenuItemModifierPriceTiers => Set<MenuItemModifierPriceTier>();
    // public DbSet<MenuItemPriceTier> MenuItemPriceTiers => Set<MenuItemPriceTier>();
    // public DbSet<Modifier> Modifiers => Set<Modifier>();
    // public DbSet<ModifierCategory> ModifierCategories => Set<ModifierCategory>();
    // public DbSet<PinEntryDevice> PinEntryDevices => Set<PinEntryDevice>();
    // public DbSet<PLU> PLUs => Set<PLU>();
    // public DbSet<PriceTier> PriceTiers => Set<PriceTier>();
    // public DbSet<PropertyVersion> PropertyVersions => Set<PropertyVersion>();
    // public DbSet<Session> Sessions => Set<Session>();
    // public DbSet<TenderMedia> TenderMedias => Set<TenderMedia>();
    // public DbSet<TenderReason> TenderReasons => Set<TenderReason>();
    // public DbSet<Terminal> Terminals => Set<Terminal>();
    // public DbSet<Transaction> Transactions => Set<Transaction>();
    // public DbSet<TransactionAppliedDeal> TransactionAppliedDeals => Set<TransactionAppliedDeal>();
    // public DbSet<TransactionLineItem> TransactionLineItems => Set<TransactionLineItem>();
    // public DbSet<TransactionPayment> TransactionPayments => Set<TransactionPayment>();
    // public DbSet<VoidReason> VoidReasons => Set<VoidReason>();
    //
    // #endregion
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
        modelBuilder.Entity<Booking>().Property(u => u.InvoiceId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}