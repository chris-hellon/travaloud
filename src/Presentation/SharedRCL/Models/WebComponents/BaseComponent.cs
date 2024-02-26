namespace Travaloud.Tenants.SharedRCL.Models.WebComponents;

public class BaseComponent
{
    public int? MdClass { get; set; }
    public int? LgClass { get; set; }
    public int? XsClass { get; set; }
    public int? MarginBottom { get; set; }
    public int? MarginBottomLg { get; set; }
    public int? MarginTop { get; set; }
    public int? MarginLeft { get; set; }
    public int? MarginRight { get; set; }
    public int? PaddingBottom { get; set; }
    public int? PaddingTop { get; set; }
    public int? PaddingLeft { get; set; }
    public int? PaddingRight { get; set; }
    public int? PaddingBottomLg { get; set; }
    public int? PaddingTopLg { get; set; }
    public int? PaddingLeftLg { get; set; }
    public int? PaddingRightLg { get; set; }

    public string? ParentCssClass { get; set; }

    public string CssClass
    {
        get
        {
            var cssClass = "";

            if (XsClass != null)
                cssClass += $"col-xs-{XsClass.Value} ";

            if (MdClass != null)
                cssClass += $"col-md-{MdClass.Value} ";

            if (LgClass != null)
                cssClass += $"col-lg-{LgClass.Value} ";

            if (MarginBottom != null)
                cssClass += $"mb-{MarginBottom.Value} ";

            if(MarginBottomLg != null)
                cssClass += $"mb-lg-{MarginBottomLg.Value} ";

            if (MarginTop != null)
                cssClass += $"mt-{MarginTop.Value} ";

            if(MarginLeft != null)
                cssClass += $"ms-{MarginLeft.Value} ";

            if (MarginRight != null)
                cssClass += $"me-{MarginRight.Value} ";

            if (PaddingBottom != null)
                cssClass += $"pb-{PaddingBottom.Value} ";

            if (PaddingTop != null)
                cssClass += $"pt-{PaddingTop.Value} ";

            if (PaddingLeft != null)
                cssClass += $"ps-{PaddingLeft.Value} ";

            if (PaddingRight != null)
                cssClass += $"pe-{PaddingRight.Value} ";

            if(PaddingBottomLg != null)
                cssClass += $"pb-lg-{PaddingBottomLg.Value} ";

            if (PaddingTopLg != null)
                cssClass += $"pt-lg-{PaddingTopLg.Value} ";

            if (PaddingLeftLg != null)
                cssClass += $"ps-lg-{PaddingLeftLg.Value} ";

            if (PaddingRightLg != null)
                cssClass += $"pe-lg-{PaddingRightLg.Value} ";

            cssClass = cssClass.Trim();

            return cssClass;
        }
    }

    public BaseComponent()
    {
        MdClass = 6;
        LgClass = 6;
        XsClass = 12;
    }

    public BaseComponent(int? mdClass, int? lgClass, int? xsClass, int? marginBottom = null, int? marginTop = null, int? marginLeft = null, int? marginRight = null, int? paddingBottom = null, int? paddingTop = null, int? paddingLeft = null, int? paddingRight = null, int? marginBottomLg = null)
    {
        MdClass = mdClass;
        LgClass = lgClass;
        XsClass = xsClass;
        MarginBottom = marginBottom;
        MarginTop = marginTop;
        MarginLeft = marginLeft;
        MarginRight = marginRight;
        PaddingBottom = paddingBottom;
        PaddingTop = paddingTop;
        PaddingLeft = paddingLeft;
        PaddingRight = paddingRight;
        MarginBottomLg = marginBottomLg;
    }

    public BaseComponent(string? parentCssClass = null)
    {
        ParentCssClass = parentCssClass;
    }
}