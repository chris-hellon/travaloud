using Travaloud.Domain.Catalog.Bookings;

namespace Travaloud.Domain.PointOfSale;

public abstract class FloorPlanTable(
        DefaultIdType floorPlanId, 
        int tableNumber, 
        int seatCapacity, 
        int? partySize, 
        decimal width,
        decimal height, 
        decimal x, 
        decimal y, 
        DefaultIdType? bookingPackageId, 
        string description)
    : AuditableEntity, IAggregateRoot
{
    public DefaultIdType FloorPlanId { get; private set; } = floorPlanId;
    
    public int TableNumber { get; private set; } = tableNumber;
    
    public int SeatCapacity { get; private set; } = seatCapacity;
    
    public int? PartySize { get; private set; } = partySize;
    
    public decimal Width { get; private set; } = width;
    
    public decimal Height { get; private set; } = height;
    
    public decimal X { get; private set; } = x;
    
    public decimal Y { get; private set; } = y;
    
    public DefaultIdType? BookingPackageId { get; private set; } = bookingPackageId;
    
    public string Description { get; private set; } = description;

    public virtual FloorPlan FloorPlan { get; set; } = default!;
    public virtual BookingPackage? BookingPackage { get; set; } = default!;
    
    public FloorPlanTable Update(
        int? tableNumber, 
        int? seatCapacity, 
        int? partySize, 
        decimal? width,
        decimal? height, 
        decimal? x, 
        decimal? y, 
        DefaultIdType? bookingPackageId, 
        string? description)
    {
        if (tableNumber.HasValue && TableNumber != tableNumber)
            TableNumber = tableNumber.Value;

        if (seatCapacity.HasValue && SeatCapacity != seatCapacity)
            SeatCapacity = seatCapacity.Value;

        if (partySize.HasValue && PartySize != partySize)
            PartySize = partySize.Value;

        if (width.HasValue && Width != width)
            Width = width.Value;

        if (height.HasValue && Height != height)
            Height = height.Value;

        if (x.HasValue && X != x)
            X = x.Value;

        if (y.HasValue && Y != y)
            Y = y.Value;

        if (bookingPackageId.HasValue && BookingPackageId != bookingPackageId)
            BookingPackageId = bookingPackageId;

        if (description is not null && Description != description)
            Description = description;

        return this;
    }

}