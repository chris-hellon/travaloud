using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

namespace Travaloud.Infrastructure.Persistence.Configuration;

public class DestinationConfig : IEntityTypeConfiguration<Destination>
{
    public void Configure(EntityTypeBuilder<Destination> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Name).HasColumnType("nvarchar(1024)");
        builder.Property(x => x.Description).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ShortDescription).HasColumnType("nvarchar(3000)");
        builder.Property(x => x.Directions).HasColumnType("nvarchar(max)");
        builder.Property(x => x.GoogleMapsKey).HasColumnType("nvarchar(300)");

        builder.Property(p => p.ImagePath).HasMaxLength(2048);
        builder.Property(p => p.ThumbnailImagePath).HasMaxLength(2048);
    }
}

public class PropertiesConfig : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.IsMultiTenant();

        builder.Property(p => p.ImagePath).HasMaxLength(2048);
        builder.Property(p => p.ThumbnailImagePath).HasMaxLength(2048);
        builder.Property(p => p.VideoPath).HasMaxLength(2048);
        builder.Property(p => p.MobileVideoPath).HasMaxLength(2048);
    }
}

public class BookingsConfig : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.IsMultiTenant();
    }
}

public class BookingPackagesConfig : IEntityTypeConfiguration<BookingPackage>
{
    public void Configure(EntityTypeBuilder<BookingPackage> builder)
    {
        builder.IsMultiTenant();
    }
}


public class TourConfig : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Name).HasColumnType("nvarchar(1024)").HasMaxLength(1024);
        builder.Property(x => x.Description).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ShortDescription).HasColumnType("nvarchar(3000)").HasMaxLength(3000);
        builder.Property(x => x.Address).HasColumnType("nvarchar(300)").HasMaxLength(300);
        builder.Property(x => x.TelephoneNumber).HasColumnType("nvarchar(50)").HasMaxLength(50);
        builder.Property(x => x.WhatsIncluded).HasColumnType("nvarchar(max)");
        builder.Property(x => x.WhatsNotIncluded).HasColumnType("nvarchar(max)");
        builder.Property(x => x.AdditionalInformation).HasColumnType("nvarchar(max)");

        builder.Property(p => p.ImagePath).HasMaxLength(2048);
        builder.Property(p => p.ThumbnailImagePath).HasMaxLength(2048);
        builder.Property(p => p.VideoPath).HasMaxLength(2048);
        builder.Property(p => p.MobileVideoPath).HasMaxLength(2048);
    }
}

public class TourCategoryConfig : IEntityTypeConfiguration<TourCategory>
{
    public void Configure(EntityTypeBuilder<TourCategory> builder)
    {
        builder.IsMultiTenant();

        builder.Property(b => b.Name).HasColumnType("nvarchar(1024)").HasMaxLength(1024);
        builder.Property(x => x.Description).HasColumnType("nvarchar(max)");
        builder.Property(x => x.ShortDescription).HasColumnType("nvarchar(3000)").HasMaxLength(3000);

        builder.Property(p => p.ImagePath).HasMaxLength(2048);
        builder.Property(p => p.ThumbnailImagePath).HasMaxLength(2048);
    }
}

public class TourDateConfig : IEntityTypeConfiguration<TourDate>
{
    public void Configure(EntityTypeBuilder<TourDate> builder)
    {
        builder.IsMultiTenant();
    }
}

public class TourItineraryConfig : IEntityTypeConfiguration<TourItinerary>
{
    public void Configure(EntityTypeBuilder<TourItinerary> builder)
    {
        builder.Property(b => b.Header).HasColumnType("nvarchar(100)").HasMaxLength(100);
        builder.Property(x => x.Title).HasColumnType("nvarchar(200)").HasMaxLength(200);
        builder.Property(x => x.Description).HasColumnType("nvarchar(max)");

        builder.IsMultiTenant();
    }
}

public class TourItinerarySectionConfig : IEntityTypeConfiguration<TourItinerarySection>
{
    public void Configure(EntityTypeBuilder<TourItinerarySection> builder)
    {
        builder.IsMultiTenant();

        builder.Property(p => p.Title).HasMaxLength(300);
        builder.Property(p => p.SubTitle).HasMaxLength(300);
    }
}

public class TourItinerarySectionImageConfig : IEntityTypeConfiguration<TourItinerarySectionImage>
{
    public void Configure(EntityTypeBuilder<TourItinerarySectionImage> builder)
    {
        builder.IsMultiTenant();

        builder.Property(p => p.ImagePath).HasMaxLength(2048);
        builder.Property(p => p.ThumbnailImagePath).HasMaxLength(2048);
    }
}

public class TourCategoryLookupConfig : IEntityTypeConfiguration<TourCategoryLookup>
{
    public void Configure(EntityTypeBuilder<TourCategoryLookup> builder)
    {
        builder.IsMultiTenant();
    }
}

public class TourDestinationLookupConfig : IEntityTypeConfiguration<TourDestinationLookup>
{
    public void Configure(EntityTypeBuilder<TourDestinationLookup> builder)
    {
        builder.IsMultiTenant();
    }
}

public class DestinationTourCategoryLookupConfig : IEntityTypeConfiguration<DestinationTourCategoryLookup>
{
    public void Configure(EntityTypeBuilder<DestinationTourCategoryLookup> builder)
    {
        builder.IsMultiTenant();
    }
}

public class SercicesConfig : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.IsMultiTenant();
    }
}

public class SerciceFieldsConfig : IEntityTypeConfiguration<ServiceField>
{
    public void Configure(EntityTypeBuilder<ServiceField> builder)
    {
        builder.IsMultiTenant();
    }
}

public class JobVacanciesLookupConfig : IEntityTypeConfiguration<JobVacancy>
{
    public void Configure(EntityTypeBuilder<JobVacancy> builder)
    {
        builder.IsMultiTenant();
    }
}

public class JobVacancyResponsesConfig : IEntityTypeConfiguration<JobVacancyResponse>
{
    public void Configure(EntityTypeBuilder<JobVacancyResponse> builder)
    {
        builder.Property(p => p.Email).HasMaxLength(256);
        builder.Property(p => p.FirstName).HasMaxLength(256);
        builder.Property(p => p.LastName).HasMaxLength(256);
        builder.Property(p => p.HowCanWeCollaborate).HasMaxLength(2000);
        builder.Property(p => p.EstimatedDates).HasMaxLength(500);
        builder.Property(p => p.DestinationsVisited).HasMaxLength(500);
        builder.Property(p => p.Instagram).HasMaxLength(256);
        builder.Property(p => p.TikTok).HasMaxLength(256);
        builder.Property(p => p.YouTube).HasMaxLength(256);
        builder.Property(p => p.Portfolio).HasMaxLength(256);
        builder.Property(p => p.Equipment).HasMaxLength(500);
    }
}

public class PageLookupConfig : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.IsMultiTenant();
    }
}

public class PageSortingLookupConfig : IEntityTypeConfiguration<PageSorting>
{
    public void Configure(EntityTypeBuilder<PageSorting> builder)
    {
        builder.IsMultiTenant();
    }
}

public class PageModalsConfig : IEntityTypeConfiguration<PageModal>
{
    public void Configure(EntityTypeBuilder<PageModal> builder)
    {
        builder.IsMultiTenant();
    }
}

public class PageModalLookupsConfig : IEntityTypeConfiguration<PageModalLookup>
{
    public void Configure(EntityTypeBuilder<PageModalLookup> builder)
    {

    }
}


public class PartnersConfig : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.IsMultiTenant();
    }
}

public class PartnerContactsConfig : IEntityTypeConfiguration<PartnerContact>
{
    public void Configure(EntityTypeBuilder<PartnerContact> builder)
    {
        builder.IsMultiTenant();
    }
}

public class EventsConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.IsMultiTenant();
    }
}

public class TourEnquiriesConfig : IEntityTypeConfiguration<TourEnquiry>
{
    public void Configure(EntityTypeBuilder<TourEnquiry> builder)
    {
        builder.IsMultiTenant();
    }
}

public class ServiceEnquiriesConfig : IEntityTypeConfiguration<ServiceEnquiry>
{
    public void Configure(EntityTypeBuilder<ServiceEnquiry> builder)
    {
        builder.IsMultiTenant();
    }
}

public class GeneralEnquiriesConfig : IEntityTypeConfiguration<GeneralEnquiry>
{
    public void Configure(EntityTypeBuilder<GeneralEnquiry> builder)
    {
        builder.IsMultiTenant();
    }
}

public class GalleriesConfig : IEntityTypeConfiguration<Gallery>
{
    public void Configure(EntityTypeBuilder<Gallery> builder)
    {
        builder.IsMultiTenant();
    }
}

public class TravelGuidesConfig : IEntityTypeConfiguration<TravelGuide>
{
    public void Configure(EntityTypeBuilder<TravelGuide> builder)
    {
        builder.IsMultiTenant();
    }
}