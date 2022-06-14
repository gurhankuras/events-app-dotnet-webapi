
using Auth.AsyncServices;
using Auth.Dto;
using Auth.Models;
using Auth.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;


[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _eventRepo;
    private readonly IEventService _eventService;
    private readonly IMapper _mapper;
    private readonly IMongoClient _mongoClient;
    private readonly IElasticClient _elasticClient;
    private readonly IMessageBusClient _busClient;

    public EventsController(
        IEventRepository eventRepo, 
        IMapper mapper, 
        IMongoClient mongoClient, 
        IElasticClient elasticClient,
        IMessageBusClient busClient,
        IEventService eventService) 
    {
        _eventRepo = eventRepo;
        _mapper = mapper;
        _mongoClient = mongoClient;
        _elasticClient = elasticClient;
        _busClient = busClient;
        _eventService = eventService;
    }


    [Authorize]
    [HttpPost]
    public async Task<object> CreateEvent([FromBody] CreateEventRequest req)
    {  
        var id = User.FindFirst("id")?.Value;

        if (id == null) 
        {
            return Unauthorized();
        }
        var e = _mapper.Map<Event>(req);
        e.CreatorId = new Guid(id);
        e.CreatedAt = DateTime.UtcNow;
        //e.CreatedAt = DateTime.UtcNow;
        await _eventRepo.Create(e);
        var a = await _elasticClient.IndexDocumentAsync<Event>(e);
        return StatusCode(201, _mapper.Map<CreatedEventResponse>(e));
    }

    [HttpPost("test")]
    public async Task<object> Test([FromBody] dynamic req) {
        Console.WriteLine(DateTime.UtcNow.ToString("o"));
        Console.Write(req);
        return req;
    }

    [HttpGet("search")]
    public async Task<IEnumerable<CreatedEventResponse>> Search([FromQuery] string title) 
    {
        var response = await _elasticClient.SearchAsync<Event>(s => s
        .Query(q => q.Match(m => m.Field(f => f.Title).Query(title))
        ));

        return _mapper.Map<IReadOnlyCollection<CreatedEventResponse>>(response.Documents);
    }

    //[Authorize]
    [HttpGet]
    public async Task<IEnumerable<NearEvent>> GetNearEvents([FromQuery] GetNearEventsQuery query)
    {  
        var location = new Coordinates 
        {
            Latitude = query.Latitute,
            Longitute = query.Longitude
        };
        var events = await _eventService.GetNearEventsAsync(location);
        return events;
    }

    [HttpGet("category")]
    public async Task<IEnumerable<EventByCategoryResponse>> GetByCategory([FromQuery] string category) 
    {
        var categoricEvents = await _eventService.GetByCategory(category);
        return categoricEvents;
    }
    
}