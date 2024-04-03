using HtmlAgilityPack;

namespace Travaloud.Infrastructure.Common.Extensions;

public static class ListExtensions
{
    private static readonly Random Rng = new();

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    public static List<string> ExtractLiElements(this string htmlString)
    {
        var liElements = new List<string>();

        // Create an HTML document
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlString);

        // Find all <ul> elements
        var ulElements = htmlDocument.DocumentNode.SelectNodes("//ul");

        if (ulElements == null) return liElements;
        
        // Iterate through each <ul> element
        liElements.AddRange(from liNodes in ulElements.Select(ulElement => ulElement.SelectNodes(".//li")).OfType<HtmlNodeCollection>() from liNode in liNodes select liNode.InnerHtml);

        return liElements;
    }
}
