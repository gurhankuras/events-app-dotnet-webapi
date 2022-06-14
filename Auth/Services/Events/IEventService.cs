using Auth.Dto;
using Auth.Models;
using Auth.Services;
using AutoMapper;

public interface IEventService 
{
    Task<IEnumerable<NearEvent>> GetNearEventsAsync(Coordinates location);
    Task<IEnumerable<EventByCategoryResponse>> GetByCategory(string category);
    Task<Event?> GetById(string id);
    Task SetStreaming(string id, PrivateEventLiveStreamInfo streamInfo);

}

public class EventService : IEventService
{
    private readonly IEventRepository _repository;
    private readonly IMapper _mapper;

    public EventService(IEventRepository repository, IMapper mapper)
    {
        _repository = repository;
        this._mapper = mapper;
    }

    public async Task<IEnumerable<EventByCategoryResponse>> GetByCategory(string category)
    {
        return await _repository.GetByCategory(category);
    }

    public async Task<Event?> GetById(string id) 
    {
        return await _repository.GetById(id);
    }

    public async Task<IEnumerable<NearEvent>> GetNearEventsAsync(Coordinates location)
    {
        if (location is null)
        {
            throw new ArgumentNullException(nameof(location));
        }

        var events = await _repository.GetNearEvents(location);
        return _mapper.Map<IEnumerable<NearEvent>>(events);
    }

    public async Task SetStreaming(string id, PrivateEventLiveStreamInfo streamInfo)
    {
        await _repository.SetStreaming(id, streamInfo);
    }
}