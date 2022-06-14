using Auth.Models;
using MongoDB.Bson;

namespace Auth.Dto;

public class EventByCategoryResponse 
{
    public string Id { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime At { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    //public DateTime CreatedAt { get; set; }
    public double Longitute { get; set; }
    public double Latitude { get; set; }
    public EventAddress Address { get; set; }
    public PublicEventLiveStreamInfo LiveStream { get; set; } = new PublicEventLiveStreamInfo();
    public EventEnvironmentType Environment { get; set; } = EventEnvironmentType.Place;
    public IEnumerable<String> Categories { get; set; }

}