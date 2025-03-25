using Travaloud.Application.Catalog.Pages.Dto;

namespace Travaloud.Application.Common.Extensions;

public static class PageDetailsExtensions
{
    public static (string Title, string? SubTitle, string? SubSubTitle) GetTitleSubTitleAndSubSubTitle(
        this PageDetailsDto? pageDetails,
        string defaultTitle = "",
        string? defaultSubTitle = null,
        string? defaultSubSubTitle = null)
    {
        if (pageDetails == null)
        {
            return (defaultTitle, defaultSubTitle, defaultSubSubTitle);
        }

        if (string.IsNullOrEmpty(defaultTitle))
            defaultTitle = pageDetails.Title;

        defaultTitle = defaultTitle.ToUpper();
        
        var h2 = pageDetails.H2?.ToUpper();
        var h1 = pageDetails.H1?.ToUpper();
        var h3 = pageDetails.H3?.ToUpper();

        if (!string.IsNullOrEmpty(h2))
        {
            return (
                h2,
                !string.IsNullOrEmpty(h1) ? h1 : defaultSubTitle,
                !string.IsNullOrEmpty(h3) ? h3 : defaultSubSubTitle
            );
        }

        if (!string.IsNullOrEmpty(h1))
        {
            return (
                h1,
                !string.IsNullOrEmpty(h3) ? h3 : defaultSubTitle,
                defaultSubSubTitle
            );
        }

        if (!string.IsNullOrEmpty(h3))
        {
            return (
                h3,
                defaultSubTitle,
                defaultSubSubTitle
            );
        }

        return (defaultTitle, defaultSubTitle, defaultSubSubTitle);
    }
}