using System.Data;
using Travaloud.Application.Catalog.Bookings.Specification;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Application.Common.Utils;
using Travaloud.Application.Identity.Users;
using Travaloud.Domain.Catalog.Bookings;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Bookings.Commands;

public class UpdateBookingRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal TotalAmount { get; set; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public int ItemQuantity { get; set; } = default!;
    public bool IsPaid { get; set; } = default!;
    public DateTime BookingDate { get; set; } = default!;
    public string? GuestId { get; set; }
    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
    public int ConcurrencyVersion { get; set; }
    public string? StripeSessionId { get; set; }
    public bool? SendPaymentLink { get; set; }
    public string? AdditionalNotes { get; set; }
    public bool WaiverSigned { get; set; }
    public string? BookingSource { get; set; }
    public bool DoNotUpdateAmount { get; set; }
    public decimal? AmountOutstanding { get; set; }
    public bool? Cancelled {get; set; }
    public bool StaffMemberRequired { get; set; } = true;

    public IList<UpdateBookingItemRequest> Items { get; set; } = [];
    public UserDetailsDto? Guest { get; set; }
    public UserDetailsDto? StaffMember { get; set; }
}

public class UpdateBookingRequestHandler : IRequestHandler<UpdateBookingRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Booking> _bookingRepository;
    private readonly IRepositoryFactory<TourDate> _tourDateRepository;
    private readonly IStringLocalizer<UpdateBookingRequestHandler> _localizer;
    private readonly ICurrentUser _currentUser;
    
    public UpdateBookingRequestHandler(IRepositoryFactory<Booking> bookingRepository,
        IRepositoryFactory<TourDate> tourDateRepository,
        IStringLocalizer<UpdateBookingRequestHandler> localizer, ICurrentUser currentUser)
    {
        _bookingRepository = bookingRepository;
        _tourDateRepository = tourDateRepository;
        _localizer = localizer;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(UpdateBookingRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.GuestId))
            throw new CustomException("A Guest must be selected.");
        
        var booking = await _bookingRepository.SingleOrDefaultAsync(new BookingByIdSpec(request.Id), cancellationToken);

        _ = booking ?? throw new NotFoundException(string.Format(_localizer["booking.notfound"], request.Id));

        if (booking.ConcurrencyVersion != request.ConcurrencyVersion)
        {
            // Handle concurrency conflict scenario
            throw new DBConcurrencyException("Booking has been updated by another user.");
        }
        
        if (request.StaffMember == null && request.StaffMemberRequired)
            throw new CustomException("Please Select a Staff Member");
        
        var createdBy = request.StaffMemberRequired ? DefaultIdType.Parse(request.StaffMember.Id) : booking.CreatedBy;

        // Update the booking properties
        var updatedBooking = booking.Update(
            request.Description,
            request.TotalAmount,
            request.CurrencyCode,
            request.ItemQuantity,
            request.IsPaid,
            request.BookingDate,
            request.GuestId,
            request.StripeSessionId,
            request.WaiverSigned,
            request.GuestName,
            request.GuestEmail,
            request.AdditionalNotes,
            request.BookingSource,
            null,
            null,
            createdBy,
            request.DoNotUpdateAmount,
            request.AmountOutstanding,
            null, 
            request.Cancelled);
        
        var userId = _currentUser.GetUserId();
        
        await updatedBooking.ProcessBookingItems(booking, request.Items, userId, _tourDateRepository, cancellationToken);
        
        booking.ConcurrencyVersion++;

        // Save the changes to the repository
        await _bookingRepository.UpdateAsync(booking, cancellationToken);

        return booking.Id;
    }

}