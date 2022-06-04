
using Auth.Models;
using MongoDB.Bson;

public class CreatedEventResponse 
{
    public string Id { get; set; }
    public DateTime At { get; set; }
    public string Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public double Longitute { get; set; }
    public double Latitude { get; set; }
    public EventAddress Address { get; set; }
}