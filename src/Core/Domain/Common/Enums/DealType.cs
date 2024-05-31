using System.ComponentModel;

namespace Travaloud.Domain.Common.Enums;

public enum DealType
{
    [Description("Price Discount")]
    PriceDiscount,

    [Description("Product Giveaway")]
    ProductGiveaway,

    [Description("Percentage Discount")]
    PercentageDiscount,
}