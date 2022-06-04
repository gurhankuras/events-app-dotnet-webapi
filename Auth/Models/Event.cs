
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Auth.Models;

public class Event 
{
    public ObjectId Id { get; set; }
    public DateTime At { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public GeoJsonPoint<GeoJson2DCoordinates> Location { get; set; }
    public EventAddress Address { get; set; }
}

public class EventAddress {
    public string City { get; set; }
    public string District { get; set; }
    public string? AddressLine { get; set; }
}