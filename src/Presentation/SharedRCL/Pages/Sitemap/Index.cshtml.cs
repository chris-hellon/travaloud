using System.Text;
using System.Xml.Serialization;
using AspNetCore.SEOHelper.Sitemap;

namespace Travaloud.Tenants.SharedRCL.Pages.Sitemap;

public class IndexModel : TravaloudBasePageModel
{
    public async Task<ContentResult> OnGetAsync()
    {
        await base.OnGetDataAsync();

        var sb = new StringBuilder();
        sb.Append(GenerateSitemap());

        return new ContentResult
        {
            ContentType = "application/xml",
            Content = sb.ToString(),
            StatusCode = 200
        };
    }

    private string GenerateSitemap()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var sitemapNodes = new List<SitemapNode>()
        {
            new() { LastModified = DateTime.UtcNow, Priority = 0.5, Url = baseUrl, Frequency = SitemapFrequency.Monthly },
        };

        switch (TenantId)
        {
            case "fuse":
                sitemapNodes.AddRange(new List<SitemapNode>() {
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/hostels", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/tours", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/our-story", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/get-in-touch", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/hostel-booking", Frequency = SitemapFrequency.Yearly }
                });

                if (Properties != null)
                    sitemapNodes.AddRange(Properties.Select(property => new SitemapNode
                    {
                        LastModified = DateTime.UtcNow, Priority = 0.1,
                        Url = $"{baseUrl}/hostels/{property.FriendlyUrl}", Frequency = SitemapFrequency.Monthly
                    }));

                if (Tours != null)
                    sitemapNodes.AddRange(Tours.Select(tour => new SitemapNode
                    {
                        LastModified = DateTime.UtcNow, Priority = 0.1, Url = $"{baseUrl}/tours/{tour.FriendlyUrl}",
                        Frequency = SitemapFrequency.Monthly
                    }));
                break;
            case "vbh":
                sitemapNodes.AddRange(new List<SitemapNode>() {
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/hostels", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/tours", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/services", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/contact", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/hostel-booking", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/about-us", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/about-us/city-guides", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/about-us/community", Frequency = SitemapFrequency.Yearly },
                    new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/about-us/join-our-crew", Frequency = SitemapFrequency.Yearly },
                });

                if (Properties != null)
                    sitemapNodes.AddRange(Properties.Select(property => new SitemapNode
                    {
                        LastModified = DateTime.UtcNow, Priority = 0.1,
                        Url = $"{baseUrl}/hostels/{property.FriendlyUrl}", Frequency = SitemapFrequency.Monthly
                    }));

                if (Tours != null)
                    sitemapNodes.AddRange(Tours.Select(tour => new SitemapNode
                    {
                        LastModified = DateTime.UtcNow, Priority = 0.1, Url = $"{baseUrl}/tours/{tour.FriendlyUrl}",
                        Frequency = SitemapFrequency.Monthly
                    }));

                if (Services != null)
                    sitemapNodes.AddRange(Services.Select(service => new SitemapNode
                    {
                        LastModified = DateTime.UtcNow, Priority = 0.1,
                        Url = $"{baseUrl}/service/{service.FriendlyUrl}", Frequency = SitemapFrequency.Monthly
                    }));
                break;
            case "uncut":
                if (Tours != null)
                    sitemapNodes.AddRange(Tours.Select(tour => new SitemapNode
                    {
                        LastModified = DateTime.UtcNow, Priority = 0.1, Url = $"{baseUrl}/tours/{tour.FriendlyUrl}",
                        Frequency = SitemapFrequency.Monthly
                    }));

                if (Services != null)
                    sitemapNodes.AddRange(Services.Select(service => new SitemapNode
                    {
                        LastModified = DateTime.UtcNow, Priority = 0.1,
                        Url = $"{baseUrl}/service/{service.FriendlyUrl}", Frequency = SitemapFrequency.Monthly
                    }));
                break;
        }

        sitemapNodes.AddRange(new List<SitemapNode>() {
            new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/terms-and-conditions", Frequency = SitemapFrequency.Yearly },
            new() { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/cookie-policy", Frequency = SitemapFrequency.Yearly },
        });

        if (TenantId == "fuse")
        {
            sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.1, Url = baseUrl + "/privacy-policy", Frequency = SitemapFrequency.Yearly });
        }

        var serializer = new XmlSerializer(typeof(List<SitemapNode>));

        var stringwriter = new StringWriter();
        serializer.Serialize(stringwriter, sitemapNodes);

        return stringwriter.ToString();
    }
}