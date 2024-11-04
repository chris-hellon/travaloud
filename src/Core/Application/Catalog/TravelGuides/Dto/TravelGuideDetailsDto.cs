using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Travaloud.Application.Catalog.TravelGuides.Dto;

public class TravelGuideDetailsDto
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string SubTitle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string DescriptionFormatted => CleanHtmlContent(Description);
    public string? ImagePath { get; set; }
    public DateTime CreatedOn { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? UrlFriendlyTitle { get; set; } = default!;
    
    public IEnumerable<TravelGuideGalleryImageDto> TravelGuideGalleryImages { get; set; } = default!;

    public static string CleanHtmlContent(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input.Replace("’", "'");
        
        // Step 1: Remove HTML tags
        string plainText = Regex.Replace(input, "<.*?>", string.Empty);
        
        // Step 2: Decode HTML entities
        plainText = HttpUtility.HtmlDecode(plainText);
        
        // Step 3: Normalize whitespace
        plainText = Regex.Replace(plainText, @"\s+", " "); // Replace multiple spaces with a single space
        
        // Step 4: Decode numeric HTML entities (hex and decimal)
        plainText = DecodeNumericHtmlEntities(plainText);
        
        return plainText.Trim();
    }

    private static string DecodeNumericHtmlEntities(string input)
    {
        // Decode decimal numeric entities (e.g., &#39;)
        input = Regex.Replace(input, @"&#(\d+);", match => 
        {
            int code = int.Parse(match.Groups[1].Value);
            return char.ConvertFromUtf32(code);
        });

        // Decode hexadecimal numeric entities (e.g., &#x27;)
        input = Regex.Replace(input, @"&#x([0-9A-Fa-f]+);", match => 
        {
            int code = Convert.ToInt32(match.Groups[1].Value, 16);
            return char.ConvertFromUtf32(code);
        });

        Regex reg = new Regex("[*'\",_&#^@]");
        input = reg.Replace(input, string.Empty);

        Regex reg1 = new Regex("[ ]");
        input = reg.Replace(input, "-");
        
        return input;
    }
}