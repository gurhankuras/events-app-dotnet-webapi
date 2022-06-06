using Auth.Models;
namespace Auth.Dto;
public class CreateEventRequest 
{
    public DateTime At { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }

    public double Longitute { get; set; }
    public double Latitude { get; set; }
    public CreateEventRequestAddress Address { get; set; }
}

public class CreateEventRequestAddress {
    public string City { get; set; }
    public string District { get; set; }
    public string? AddressLine { get; set; }
}
