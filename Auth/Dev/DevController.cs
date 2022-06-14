using Auth.Dto;
using Auth.Models;
using Auth.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nest;

[ApiController]
[Route("[controller]")]
public class DevController: ControllerBase
{
    private readonly IEventRepository _eventRepo;
    private readonly IMapper _mapper;
    private readonly IElasticClient _elasticClient;

    public DevController(IEventRepository eventRepo, IMapper mapper, IElasticClient elasticClient)
    {
        _eventRepo = eventRepo;
        _mapper = mapper;
        _elasticClient = elasticClient;
    }

    [HttpPost("bulk/events")]
    public async Task<ActionResult> BulkEvents([FromBody] BulkEventsRequest body) 
    {
        foreach (var eventDto in body.Events)
        {
            var e = _mapper.Map<Event>(eventDto);
            //e.CreatedAt = DateTime.UtcNow;
            await _eventRepo.Create(e);
            var a = await _elasticClient.IndexDocumentAsync<Event>(e);
            //_mapper.Map<CreatedEventResponse>(e);

            //var e = _mapper.Map<Event>(eventDto);
            //await _eventRepo.Create(e);
            //await _elasticClient.IndexDocumentAsync<Event>(e);
        }
        return Ok(200);
    }

    [HttpGet("search/events")]
    public async Task<ActionResult> SearchEvents([FromQuery] EventRequestParameters parameters) 
    {
        /*
         var response = await _elasticClient
         .SearchAsync<Event>(s => s
                .From(parameters.From)
                .Size(parameters.PageSize)
                .Query(q => q.Match(m => m.Field(f => f.Title).Query("gaming workshop")))
                .Sort(q => q
                            .Field(e => e.Field("_score").Descending())
                            .Field(e => e.Field(f => f.At).Descending())
                )
        );
        */

        var request = new SearchRequest 
        {
            From = parameters.From,
            Size = parameters.PageSize,
            Query = new MatchQuery { Field = "title", Query = parameters.Q },
            Sort =  new List<ISort>()
            {
                new FieldSort { Field = "_score", Order = SortOrder.Descending },
                new FieldSort { Field = "at", Order = SortOrder.Descending }
            }
        };

        var response = await _elasticClient.SearchAsync<Event>(request);
                
    

        return Ok(_mapper.Map<IReadOnlyCollection<CreatedEventResponse>>(response.Documents));
    }

}

public class BulkEventsRequest 
{
    public IEnumerable<CreateEventRequest> Events { get; set; }
}