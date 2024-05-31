using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Enums;

namespace Travaloud.Domain.PointOfSale;

public abstract class Deal(
    string name,
    string posName,
    string appName,
    string? appDescription,
    DateTime? dealAvailableFrom,
    DateTime? dealAvailableTo,
    decimal? percentageDiscount,
    int? customerLimit,
    bool? loyaltyRequired,
    string triggersQueryJson,
    string rewardsQueryJson,
    bool? publishToUat,
    bool? publishToPos,
    bool? publishToApp,
    DefaultIdType? publishToUatAmendId,
    DefaultIdType? publishToPosAmendId,
    DefaultIdType? publishToAppAmendId,
    DealType dealType,
    DefaultIdType? propertyId)
    : PublishableEntity(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId,
        publishToAppAmendId)
{
    [MaxLength(128)] 
    public string Name { get; private set; } = name;

    [MaxLength(128)] 
    public string POSName { get; private set; } = posName;

    [MaxLength(128)] 
    public string AppName { get; private set; } = appName;

    [MaxLength(256)] 
    public string? AppDescription { get; private set; } = appDescription;

    public DateTime? DealAvailableFrom { get; private set; } = dealAvailableFrom;

    public DateTime? DealAvailableTo { get; private set; } = dealAvailableTo;

    [Precision(3, 0)] 
    public decimal? PercentageDiscount { get; private set; } = percentageDiscount;

    public int? CustomerLimit { get; private set; } = customerLimit;

    public bool? LoyaltyRequired { get; private set; } = loyaltyRequired;

    public string TriggersQueryJson { get; private set; } = triggersQueryJson;

    public string RewardsQueryJson { get; private set; } = rewardsQueryJson;

    public DealType DealType { get; private set; } = dealType;

    public DefaultIdType? PropertyId { get; private set; } = propertyId;

    public virtual Property Property { get; set; } = default!;
    public virtual IEnumerable<DealPriceTier>? PriceTiers { get; set; } = default!;

    public Deal Update(string? name, string? posName, string? appName, string? appDescription,
        DateTime? dealAvailableFrom, DateTime? dealAvailableTo, decimal? percentageDiscount, int? customerLimit,
        bool? loyaltyRequired, string? triggersQueryJson, string? rewardsQueryJson, bool? publishToUat,
        bool? publishToPos, bool? publishToApp, DefaultIdType? publishToUatAmendId, DefaultIdType? publishToPosAmendId,
        DefaultIdType? publishToAppAmendId, DealType? dealType)
    {
        if (name is not null && Name != name)
            Name = name;

        if (posName is not null && POSName != posName)
            POSName = posName;

        if (appName is not null && AppName != appName)
            AppName = appName;

        if (appDescription is not null && AppDescription != appDescription)
            AppDescription = appDescription;

        if (dealAvailableFrom is not null && DealAvailableFrom != dealAvailableFrom)
            DealAvailableFrom = dealAvailableFrom.Value;

        if (dealAvailableTo is not null && DealAvailableTo != dealAvailableTo)
            DealAvailableTo = dealAvailableTo.Value;

        if (percentageDiscount is not null && PercentageDiscount != percentageDiscount)
            PercentageDiscount = percentageDiscount.Value;

        if (customerLimit is not null && CustomerLimit != customerLimit)
            CustomerLimit = customerLimit.Value;

        if (loyaltyRequired is not null && LoyaltyRequired != loyaltyRequired)
            LoyaltyRequired = loyaltyRequired;

        if (triggersQueryJson is not null && TriggersQueryJson != triggersQueryJson)
            TriggersQueryJson = triggersQueryJson;

        if (rewardsQueryJson is not null && RewardsQueryJson != rewardsQueryJson)
            RewardsQueryJson = rewardsQueryJson;

        base.Update(publishToUat, publishToPos, publishToApp, publishToUatAmendId, publishToPosAmendId,
            publishToAppAmendId);

        if (dealType is not null && DealType != dealType)
            DealType = dealType.Value;

        return this;
    }
}