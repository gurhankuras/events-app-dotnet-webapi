
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
[Route("live")]
public class LiveStreamController : ControllerBase
{
    private readonly IEventRepository _eventRepo;
    private readonly IEventService _eventService;
    private readonly IMapper _mapper;
    //private readonly IMongoClient _mongoClient;
    private readonly IElasticClient _elasticClient;
    private readonly IMessageBusClient _busClient;

    public LiveStreamController(
        IEventRepository eventRepo, 
        IMapper mapper, 
        //IMongoClient mongoClient, 
        IElasticClient elasticClient,
        IMessageBusClient busClient,
        IEventService eventService) 
    {
        _eventRepo = eventRepo;
        _mapper = mapper;
        //_mongoClient = mongoClient;
        _elasticClient = elasticClient;
        _busClient = busClient;
        _eventService = eventService;
    }

    [HttpGet]
    public DateTime Demo() {
          return DateTime.UtcNow;
    }



    [HttpPost("events")]
    public async Task<ActionResult<object>> Stream([FromBody] StartLiveStreamQuery query)
    {
     /*
       var id = User.FindFirst("id")?.Value;

       if (id == null) 
       {
            return Unauthorized();
       }
       */

       var evt = await _eventService.GetById(query.EventId);
       
       if (evt == null) 
       {
            return NotFound();
       }

       if (evt.Environment == EventEnvironmentType.Place) 
       {
            return BadRequest();
       }

       if (evt.CreatorId.ToString() != query.UserId 
            || evt.LiveStream.StreamingKey == null 
            || evt.LiveStream.StreamingKey != query.Key) 
       {
            return Unauthorized();
       }

       await _eventService.SetStreaming(query.EventId, new PrivateEventLiveStreamInfo {
            Finished = false,
            StartedAt = DateTime.UtcNow,
            Url = "",
            StreamingKey = evt.LiveStream.StreamingKey
       });

       return Ok();
    }
}

public class StartLiveStreamQuery
{
    public string UserId { get; set; }
    public string EventId { get; set; }
    public string Key { get; set; }
}