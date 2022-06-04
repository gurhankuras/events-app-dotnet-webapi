using Auth.Models;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

public class NearEventDto 
{
    public ObjectId Id { get; set; }
    public DateTime At { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }

    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public GeoJsonPoint<GeoJson2DCoordinates> Location { get; set; }

    public EventAddress Address { get; set; }
 
    //public double Longitute { get; set; }
    //public double Latitude { get; set; }
    public double Distance { get; set; }
}