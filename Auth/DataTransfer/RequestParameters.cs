

public class RequestParameters 
{
    const int maxSize = 50;
    private int _size = 10;
    public int PageSize { 
        get {
            return _size;
        }
        set {
            _size = Math.Clamp(value, 1, maxSize);
        } 
    }
    public int PageNumber { get; set; } = 1;

    public int From {
        get {
            return (Math.Max(1, PageNumber) - 1) * PageSize;
        }
    }
}


public class EventRequestParameters: RequestParameters
{
    public string Q { get; set; } = "";
}