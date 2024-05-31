namespace Travaloud.Domain.PointOfSale;

public abstract class PublishableEntity(
        bool? publishToUat, 
        bool? publishToPos, 
        bool? publishToApp, 
        DefaultIdType? publishToUatAmendId,
        DefaultIdType? publishToPosAmendId, 
        DefaultIdType? publishToAppAmendId)
    : AuditableEntity, IAggregateRoot
{
    public bool? PublishToUat { get; private set; } = publishToUat;
    
    public bool? PublishToPOS { get; private set; } = publishToPos;
    
    public bool? PublishToApp { get; private set; } = publishToApp;
    
    public DefaultIdType? PublishToUatAmendId { get; private set; } = publishToUatAmendId;
    
    public DefaultIdType? PublishToPOSAmendId { get; private set; } = publishToPosAmendId;
    
    public DefaultIdType? PublishToAppAmendId { get; private set; } = publishToAppAmendId;

    protected PublishableEntity Update(bool? publishToUat, bool? publishToPos, bool? publishToApp, DefaultIdType? publishToUatAmendId, DefaultIdType? publishToPosAmendId, DefaultIdType? publishToAppAmendId)
    {
        if (publishToUat is not null && PublishToUat != publishToUat)
            PublishToUat = publishToUat;

        if (publishToPos is not null && PublishToPOS != publishToPos)
            PublishToPOS = publishToPos;

        if (publishToApp is not null && PublishToApp != publishToApp)
            PublishToApp = publishToApp;

        if (publishToUatAmendId is not null && PublishToUatAmendId != publishToUatAmendId)
            PublishToUatAmendId = publishToUatAmendId.Value;

        if (publishToPosAmendId is not null && PublishToPOSAmendId != publishToPosAmendId)
            PublishToPOSAmendId = publishToPosAmendId.Value;

        if (publishToAppAmendId is not null && PublishToAppAmendId != publishToAppAmendId)
            PublishToAppAmendId = publishToAppAmendId.Value;

        return this;
    }
}