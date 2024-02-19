using Travaloud.Application.Catalog.Events.Commands;
using Travaloud.Application.Catalog.Events.DTO;
using Travaloud.Application.Catalog.Events.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IEventsService : ITransientService
{
    /// <summary>
    /// Search events using available filters.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PaginationResponse<EventDto>> SearchAsync(SearchEventsRequest request);

    /// <summary>
    /// Get service details.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<EventDto> GetAsync(DefaultIdType id);

    /// <summary>
    /// Create a new event.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<DefaultIdType> CreateAsync(CreateEventRequest request);

    /// <summary>
    /// Update a event.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateEventRequest request);

    /// <summary>
    /// Delete an event.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}