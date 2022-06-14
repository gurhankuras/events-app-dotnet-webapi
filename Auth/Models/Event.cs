
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json.Converters;

namespace Auth.Models;

public class Event 
{
    public ObjectId Id { get; set; }
    public DateTime At { get; set; }
    public Guid CreatorId { get; set; }
   // [BsonObject]
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public GeoJsonPoint<GeoJson2DCoordinates> Location { get; set; }
    public EventAddress Address { get; set; }
    public IEnumerable<String> Categories { get; set; } = new List<String>();
    public PrivateEventLiveStreamInfo LiveStream { get; set; } = new PrivateEventLiveStreamInfo();
    public EventEnvironmentType Environment { get; set; } = EventEnvironmentType.Place;
// "No"
// "Yes"
// "Live"
// "Finished"
    //public string StreamingStatus { get; set; } = "No";
    //public string? LiveStreamURI { get; set; }

}

// Example enum strings
public enum EventEnvironmentType 
{
    [BsonRepresentation(BsonType.String)]
    Place, 
    [BsonRepresentation(BsonType.String)]
    Online, 
    [BsonRepresentation(BsonType.String)]
    Both,
}

public class PublicEventLiveStreamInfo 
{
    //public bool Permitted { get; set; } = false;
    //[JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? StartedAt { get; set; } = null;
    public bool? Finished { get; set; } = null;
    public string? Url { get; set; } = null;
}

public class PrivateEventLiveStreamInfo: PublicEventLiveStreamInfo 
{
    public string? StreamingKey { get; set; }
}



public class EventAddress {
    public string City { get; set; }
    public string District { get; set; }
    public string? AddressLine { get; set; }
}