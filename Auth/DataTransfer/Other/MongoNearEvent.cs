using Auth.Models;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

public class NearEventDto 
{
    public ObjectId Id { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime At { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }

    public string Description { get; set; }
    //public DateTime CreatedAt { get; set; }
    public GeoJsonPoint<GeoJson2DCoordinates> Location { get; set; }

    public EventAddress Address { get; set; }
    
    public IEnumerable<String> Categories { get; set; } = new List<String>();
    public PrivateEventLiveStreamInfo LiveStream { get; set; } = new PrivateEventLiveStreamInfo();
    public EventEnvironmentType Environment { get; set; } = EventEnvironmentType.Place;
 
    //public string StreamingStatus { get; set; } = "No";

    //public double Longitute { get; set; }
    //public double Latitude { get; set; }
    public double Distance { get; set; }
}