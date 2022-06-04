using Auth.Models;

namespace Auth.Services;

public interface IEventRepository 
{
    Task Create(Event e);
    Task<IEnumerable<NearEventDto>> GetNearEvents(Coordinates location);
}


public class Coordinates {
    public double Longitute { get; set; }
    public double Latitude { get; set; }
}