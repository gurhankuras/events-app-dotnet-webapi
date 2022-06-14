using Newtonsoft.Json.Converters;

public class DartDateTimeConverter : IsoDateTimeConverter
{
    public DartDateTimeConverter()
    {
        DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFK";
    }
}