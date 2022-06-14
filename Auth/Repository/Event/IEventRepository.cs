using Auth.Dto;
using Auth.Models;

namespace Auth.Services;

public interface IEventRepository 
{
    Task Create(Event e);
    Task<IEnumerable<NearEventDto>> GetNearEvents(Coordinates location);
    Task<IEnumerable<EventByCategoryResponse>> GetByCategory(string category);
    Task<Event?> GetById(string id);
    Task SetStreaming(string id, PrivateEventLiveStreamInfo streamInfo);

}


public class Coordinates {
    public double Longitute { get; set; }
    public double Latitude { get; set; }
}