using SimpleMvcSitemap;
using SimpleMvcSitemap.News;
using SimpleMvcSitemap.Videos;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.TravelGuides.Queries;

namespace Travaloud.Tenants.SharedRCL.Pages.Sitemap;

public class IndexModel : TravaloudBasePageModel
{
    private readonly ITravelGuidesService _travelGuidesService;

    public IndexModel(ITravelGuidesService travelGuidesService)
    {
        _travelGuidesService = travelGuidesService;
    }

    public async Task<ActionResult> OnGetAsync()
    {
        await base.OnGetDataAsync();

        var sitemapNodes = await GenerateSitemap();
        
        return new SitemapProvider().CreateSitemap(new SitemapModel(sitemapNodes));
    }

    private async Task<List<SitemapNode>> GenerateSitemap()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var sortedPropertiesList = Properties?.OrderByDescending(x => x.LastModifiedOn ?? x.CreatedOn).ToList();
        var sortedToursList = Tours?.OrderByDescending(x => x.LastModifiedOn).ToList();
        var lastModifiedTour = sortedToursList?.FirstOrDefault();
        var lastTourModfiedOn = (lastModifiedTour.LastModifiedOn ?? lastModifiedTour.CreatedOn);
        var lastTourModfiedOnFormatted = new DateTime(lastTourModfiedOn.Year, lastTourModfiedOn.Month, lastTourModfiedOn.Day, lastTourModfiedOn.Hour, lastTourModfiedOn.Minute, lastTourModfiedOn.Second, DateTimeKind.Local);

        var lastModifiedProperty = sortedPropertiesList?.First();
        var lastPropertyModfiedOn = (lastModifiedProperty.LastModifiedOn ?? lastModifiedProperty.CreatedOn);
        var lastPropertyModfiedOnFormatted = new DateTime(lastPropertyModfiedOn.Year, lastPropertyModfiedOn.Month, lastPropertyModfiedOn.Day, lastPropertyModfiedOn.Hour, lastPropertyModfiedOn.Minute, lastPropertyModfiedOn.Second, DateTimeKind.Local);
        
        var sitemapNodes = new List<SitemapNode>()
        {
            new (baseUrl) {  LastModificationDate = lastTourModfiedOnFormatted, Priority = 0.5M, ChangeFrequency = ChangeFrequency.Monthly, Videos =
                [
                    new SitemapVideo("Home Page Video", "",
                        "https://travaloudcdn.azureedge.net/fuse/assets/images/POOLPARTY_23-03-22-12-min.jpg?w=300",
                        "https://travaloud.azureedge.net/fuse/assets/images/0b47297a-3910-4e83-8f83-a94e35996c09.mp4")
                ]
            },
        };
        
        switch (TenantId)
        {
            case "fuse":
                sitemapNodes.Add(new SitemapNode( baseUrl + "/hostels") { LastModificationDate = lastPropertyModfiedOnFormatted, Priority = 0.1M, ChangeFrequency = ChangeFrequency.Yearly });

                if (Properties != null)
                    sitemapNodes.AddRange(Properties.Select(property => new SitemapNode($"{baseUrl}/hostels/{property.FriendlyUrl}")
                    {
                        LastModificationDate = new DateTime((property.LastModifiedOn ?? property.CreatedOn).Year,
                            (property.LastModifiedOn ?? property.CreatedOn).Month,
                            (property.LastModifiedOn ?? property.CreatedOn).Day,
                            (property.LastModifiedOn ?? property.CreatedOn).Hour,
                            (property.LastModifiedOn ?? property.CreatedOn).Minute,
                            (property.LastModifiedOn ?? property.CreatedOn).Second,
                            DateTimeKind.Local), 
                        Priority = 0.1M,
                        ChangeFrequency = ChangeFrequency.Yearly,
                        Videos = !string.IsNullOrEmpty(property.VideoPath) ?
                            [
                                new SitemapVideo($"{property.Name} Video.", $"{property.Name} Hostel Welcome Video.", property.ImagePath,
                                    property.VideoPath)
                            ]
                            : []
                    }));
                
                sitemapNodes.Add(new SitemapNode(baseUrl + "/tours") { LastModificationDate = lastPropertyModfiedOnFormatted, Priority = 0.1M, ChangeFrequency = ChangeFrequency.Monthly });

                if (Tours != null)
                    sitemapNodes.AddRange(Tours.Select(tour => new SitemapNode($"{baseUrl}/tours/{tour.FriendlyUrl}")
                    {
                        LastModificationDate = new DateTime((tour.LastModifiedOn ?? tour.CreatedOn).Year,
                            (tour.LastModifiedOn ?? tour.CreatedOn).Month,
                            (tour.LastModifiedOn ?? tour.CreatedOn).Day,
                            (tour.LastModifiedOn ?? tour.CreatedOn).Hour,
                            (tour.LastModifiedOn ?? tour.CreatedOn).Minute,
                            (tour.LastModifiedOn ?? tour.CreatedOn).Second,
                            DateTimeKind.Local), 
                        Priority = 0.1M,
                        ChangeFrequency = ChangeFrequency.Monthly
                    }));

                sitemapNodes.AddRange(new List<SitemapNode>() {
                    new(baseUrl + "/our-story") { LastModificationDate = new DateTime(2024, 02, 12, 10, 23, 00, DateTimeKind.Local), Priority = 0.1M, ChangeFrequency = ChangeFrequency.Yearly },
                    new(baseUrl + "/events") { LastModificationDate = new DateTime(2024, 05, 07, 14, 24, 00, DateTimeKind.Local), Priority = 0.1M, ChangeFrequency = ChangeFrequency.Monthly },
                    new(baseUrl + "/get-in-touch") { LastModificationDate = new DateTime(2023, 10, 07, 12, 33, 00, DateTimeKind.Local), Priority = 0.1M, ChangeFrequency = ChangeFrequency.Yearly },
                });
                
                sitemapNodes.Add(new SitemapNode(baseUrl + "/travel-guides") { LastModificationDate = new DateTime(2024, 04, 05, 13, 24, 06, DateTimeKind.Local), Priority = 0.1M, ChangeFrequency = ChangeFrequency.Weekly });
                
                var travelGuides = await _travelGuidesService.SearchAsync(new SearchTravelGuidesRequest()
                {
                    PageNumber = 1,
                    PageSize = 99999
                });

                if (travelGuides.Data.Count != 0)
                    sitemapNodes.AddRange(travelGuides.Data.Select(travelGuide => new SitemapNode($"{baseUrl}/travel-guides/{travelGuide.UrlFriendlyTitle}")
                    {
                        LastModificationDate =  new DateTime((travelGuide.LastModifiedOn ?? travelGuide.CreatedOn).Year,
                            (travelGuide.LastModifiedOn ?? travelGuide.CreatedOn).Month,
                            (travelGuide.LastModifiedOn ?? travelGuide.CreatedOn).Day,
                            (travelGuide.LastModifiedOn ?? travelGuide.CreatedOn).Hour,
                            (travelGuide.LastModifiedOn ?? travelGuide.CreatedOn).Minute,
                            (travelGuide.LastModifiedOn ?? travelGuide.CreatedOn).Second,
                            DateTimeKind.Local), 
                        Priority = 0.1M,
                        ChangeFrequency = ChangeFrequency.Yearly,
                        News = new SitemapNews(new NewsPublication("Fuse Hostels & Travel", "en"), new DateTime(travelGuide.CreatedOn.Year,
                            travelGuide.CreatedOn.Month,
                            travelGuide.CreatedOn.Day,
                            travelGuide.CreatedOn.Hour,
                            travelGuide.CreatedOn.Minute,
                            travelGuide.CreatedOn.Second,
                            DateTimeKind.Local), travelGuide.Title)
                        {
                            
                        }
                    }));
                
                sitemapNodes.AddRange(new List<SitemapNode>() {
                    new(baseUrl + "/join-our-crew") { LastModificationDate = new DateTime(2024, 02, 12, 09, 52, 45, DateTimeKind.Local), Priority = 0.1M, ChangeFrequency = ChangeFrequency.Monthly },
                    new(baseUrl + "/hostel-booking") { LastModificationDate = new DateTime(2024, 03, 01, 11, 45, 22, DateTimeKind.Local), Priority = 0.1M, ChangeFrequency = ChangeFrequency.Yearly },
                    new(baseUrl + "/terms-and-conditions") { LastModificationDate = new DateTime(2023, 06, 01, 14, 12, 12, DateTimeKind.Local), Priority = 0.1M, ChangeFrequency = ChangeFrequency.Yearly },
                    new(baseUrl + "/cookie-policy") { LastModificationDate = new DateTime(2023, 06, 01, 13, 58, 00, DateTimeKind.Local), Priority = 0.1M,  ChangeFrequency = ChangeFrequency.Yearly },
                });
                break;
        }

        return sitemapNodes;
    }
}