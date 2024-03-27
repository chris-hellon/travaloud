using MediatR;
using Travaloud.Application.Catalog.Bookings.Commands;
using Travaloud.Application.Catalog.Bookings.Dto;
using Travaloud.Application.Catalog.Bookings.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.Models;
using Travaloud.Application.Identity.Users;
using Travaloud.Shared.Authorization;

namespace Travaloud.Infrastructure.Catalog.Services;

public class BookingsService : BaseService, IBookingsService
{
    private readonly IUserService _userService;

    public BookingsService(IUserService userService, ISender mediator) : base(mediator)
    {
        _userService = userService;
    }

    public Task<PaginationResponse<BookingDto>> SearchAsync(SearchBookingsRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<BookingDetailsDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetBookingRequest(id));
    }

    public Task<IEnumerable<BookingDto>> GetGuestBookingsAsync(GetGuestBookingsRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<DefaultIdType> CreateAsync(CreateBookingRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateBookingRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> FlagBookingAsPaidAsync(DefaultIdType id, FlagBookingAsPaidRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteBookingRequest(id));
    }
    
    public Task<DefaultIdType> DeleteItemAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteBookingItemRequest(id));
    }
    
    public async Task<FileResponse> ExportAsync(ExportBookingsRequest filter)
    {
        filter.Guests = await _userService.GetListAsync(TravaloudRoles.Guest);
        
        var response = await Mediator.Send(filter);
        
        return new FileResponse(response);
    }
}