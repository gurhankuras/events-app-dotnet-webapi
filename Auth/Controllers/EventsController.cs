
using Auth.AsyncServices;
using Auth.Dto;
using Auth.Models;
using Auth.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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



    [HttpPost]
    public async Task<object> CreateEvent([FromBody] CreateEventRequest req)
    {  
        var e = _mapper.Map<Event>(req);
        e.CreatedAt = DateTime.UtcNow;
        await _eventRepo.Create(e);
        var a = await _elasticClient.IndexDocumentAsync<Event>(e);
        return _mapper.Map<CreatedEventResponse>(e);
    }

    [HttpGet("search")]
    public async Task<IEnumerable<CreatedEventResponse>> Search([FromQuery] string title) 
    {
        var response = await _elasticClient.SearchAsync<Event>(s => s
        .Query(q => q.Match(m => m.Field(f => f.Title).Query(title))
        ));

        return _mapper.Map<IReadOnlyCollection<CreatedEventResponse>>(response.Documents);
    }

    [Authorize]
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
    
}