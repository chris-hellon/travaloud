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
    public DbSet<BookingItemRoom> BookingItemRooms => Set<BookingItemRoom>();
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
    public DbSet<BookingItemGuest> BookingItemGuests => Set<BookingItemGuest>();
    public DbSet<TourPickupLocation> TourPickupLocations => Set<TourPickupLocation>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
        modelBuilder.Entity<Booking>().Property(u => u.InvoiceId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}