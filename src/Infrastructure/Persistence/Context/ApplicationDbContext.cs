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
using Travaloud.Domain.Catalog.JobVacancies;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Catalog.Partners;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Catalog.Services;
using Travaloud.Domain.Catalog.Tours;
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

    // private static readonly Dictionary<Type, string> EntitySchemaLookup = new()
    // {
    //     { typeof(Destination), SchemaNames.Destinations },
    //     { typeof(Tour), SchemaNames.Tours },
    //     { typeof(TourCategory), SchemaNames.Tours },
    //     { typeof(TourDate), SchemaNames.Tours },
    //     { typeof(TourItinerary), SchemaNames.Tours },
    //     { typeof(TourCategoryLookup), SchemaNames.Tours },
    //     { typeof(TourDestinationLookup), SchemaNames.Tours },
    //     { typeof(Property), SchemaNames.Properties },
    //     { typeof(PropertyDestinationLookup), SchemaNames.Properties },
    //     { typeof(PropertyDirection), SchemaNames.Properties },
    //     { typeof(PropertyDirectionContent), SchemaNames.Properties },
    //     { typeof(PropertyFacility), SchemaNames.Properties },
    //     { typeof(PropertyRoom), SchemaNames.Properties },
    //     { typeof(PropertyImage), SchemaNames.Properties },
    //     { typeof(Event), SchemaNames.Events },
    //     { typeof(TourPropertyLookup), SchemaNames.Tours },
    //     { typeof(Booking), SchemaNames.Bookings },
    //     { typeof(BookingItem), SchemaNames.Bookings },
    //     { typeof(BookingItemRoom), SchemaNames.Bookings },
    //     { typeof(TourPrice), SchemaNames.Tours },
    //     { typeof(TourImage), SchemaNames.Tours },
    //     { typeof(TourItinerarySection), SchemaNames.Tours },
    //     { typeof(TourItinerarySectionImage), SchemaNames.Tours },
    //     { typeof(DestinationTourCategoryLookup), SchemaNames.Destinations },
    //     { typeof(Service), SchemaNames.Services },
    //     { typeof(ServiceField), SchemaNames.Services },
    //     { typeof(JobVacancy), SchemaNames.JobVacancies },
    //     { typeof(JobVacancyResponse), SchemaNames.JobVacancies },
    //     { typeof(Page), SchemaNames.Pages },
    //     { typeof(PageSorting), SchemaNames.Pages },
    //     { typeof(PageModal), SchemaNames.Pages },
    //     { typeof(Partner), SchemaNames.Partners },
    //     { typeof(PartnerContact), SchemaNames.Partners },
    //     { typeof(TourEnquiry), SchemaNames.Enquiries },
    //     { typeof(ServiceEnquiry), SchemaNames.Enquiries },
    //     { typeof(ServiceEnquiryField), SchemaNames.Enquiries }
    // };
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        // {
        //     if (EntitySchemaLookup.TryGetValue(entityType.ClrType, out var schema))
        //     {
        //         modelBuilder.Entity(entityType.ClrType).ToTable(entityType.ClrType.Name, schema);
        //     }
        // }
        modelBuilder.HasDefaultSchema(SchemaNames.Catalog);
        modelBuilder.Entity<Booking>().Property(u => u.InvoiceId).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
    }
}