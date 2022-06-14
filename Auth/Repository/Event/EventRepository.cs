
using Auth.Dto;
using Auth.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Auth.Services;

public class InterestedUser 
{
    public ObjectId Id { get; set; }
    public string ImageUrl { get; set; } = "";
}

public class InterestedUserObjectDto {
    public IEnumerable<InterestedUser> InterestedUsers { get; set; }
}

class MongoEventRepository : IEventRepository
{
    private IMongoDatabase db;
    private readonly IMongoClient _client;
    private readonly IMapper _mapper;
    public MongoEventRepository(IMongoClient client, IMapper mapper)
    {
        db = client.GetDatabase("eventsapp");
        _client = client;
        _mapper = mapper;    
    }

    public async Task Create(Event e)
    {
        await Events.InsertOneAsync(e);
    }

     private IMongoCollection<Event> Events { 
        get {
            return db.GetCollection<Event>("events");
        } 
    }

    public async Task<IEnumerable<EventByCategoryResponse>> GetByCategory(string category) 
    {
        var filterBuilder = Builders<Event>.Filter;
        
        var filter = filterBuilder.AnyEq("Categories", category);
        var cursor = await Events.FindAsync(filter);
        var events = cursor.ToList();

        return _mapper.Map<IEnumerable<EventByCategoryResponse>>(events);
    }

    public async Task<IEnumerable<Event>> GetByCreationDate() 
    {
       
        
        var filter = Builders<Event>.Filter.Empty;
        var sort = Builders<Event>.Sort.Descending(e => e.CreatedAt);
        var options = new FindOptions<Event> { Sort = sort };
        var eventsByCreatedDate = await Events.Find(filter).SortByDescending(e => e.CreatedAt).Limit(5).ToListAsync();

        return eventsByCreatedDate;
    }


    //public async Task<IEnumerable<Event>> GetInterestedUsers(string eventId) 
    //{
        /*
        var filter = Builders<Event>.Filter.Eq(e => e.Id, new ObjectId(eventId));
        var projection = Builders<Event>.Projection
                            .Exclude(e => e.Id) 
                            .Slice(e => e.Categories, 0, 5); 
        var options = new FindOptions<Event> { Projection = projection };
        var eventsByCreatedDate = Events.Find<>(filter).Project<{}>(projection).FirstOrDefault();

        return eventsByCreatedDate;
        */
        // { $project: { name: 1, threeFavorites: { $slice: [ "$favorites", 3 ] } } }
        //var pipeline = new List<BsonDocument>();
        //var projectPipeline = {}
        //var pipeline = new List<BsonDocument>();
        //var projectionStr =  @"{ $project: { name: 1, threeFavorites: { $slice: [ ""$favorites"", 3 ] } } }";
        //var unwindStr = @"{$unwind: ""$InterestedUsers""}";
        //var projection = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(projectionStr);
        /*
        var matchPipeline = new BsonDocument 
        {
            {
                "$match", new BsonDocument{
                    {"_id", eventId}
                }
            }
        };

        var unwind = new BsonDocument 
        {
            {
                "$unwind", "$InterestedUsers"
            }
        };
        */

/*
        var unwind = new BsonDocument 
        {
            {
                "$unwind", "$InterestedUsers"
            }
        };
*/
        // { $group: { _id: null, myCount: { $sum: 1 } } }
       /*
        var group = new BsonDocument
        {
            {
                "$group", new BsonDocument {
                    {"_id", null},
                    {"Count", new BsonDocument 
                        {
                            {"$sum", 1}
                        }
                    }
                }
            }
        };

        var projection = new BsonDocument 
        {
            {
                "$project", new BsonDocument {
                    {"_id", 0}
                }
            }
        };
        var project = Builders<Event>.Projection
                            .Exclude(e => e.Id) 
                            .Slice(e => e.Categories, 0, 5);
        var filter = Builders<Event>.Filter.Eq(e => e.Id, new ObjectId(eventId));

        //Events.Find(filter).Project()
        //pipeline.Add(matchPipeline);
        //var a = Events.AggregateAsync<InterestedUserObjectDto>(pipeline);

    }
*/

    public async Task<IEnumerable<NearEventDto>> GetNearEvents(Coordinates location)
    {
        
        var distancePipeline = new BsonDocument {
                                {"$geoNear", new BsonDocument {
                                    { "near", new BsonDocument {
                                    { "type", "Point" }, 
                                    { "coordinates", new BsonArray {location.Longitute, location.Latitude} },
                                    } },
                                { "distanceField", "Distance" },
                                { "minDistance", 0},
                                { "maxDistance", 10000 }, 
                                //{ "includeLocs", "dist.location" },  
                                { "spherical" , true }
                                }}
                            };
        var sortPipeline = new BsonDocument {
            {"$sort", new BsonDocument {
               { "Distance", 1 }
            }}
        };

        var limitPipeline = new BsonDocument {
            {"$limit", 5}
        };

        var pipeline = new List<BsonDocument>();
        pipeline.Add(distancePipeline);
        pipeline.Add(sortPipeline);
        pipeline.Add(limitPipeline);

        var events = new List<NearEventDto>();
        using(var cursor = await Events.AggregateAsync<NearEventDto>(pipeline)) {
            while(await cursor.MoveNextAsync()) {
                foreach (var doc in cursor.Current) {
                    events.Add(doc);
                }
            }
        }
        return events;
    }
    
    public async Task<Auth.Models.Event?> GetById(string id)
    {
        var filter = Builders<Auth.Models.Event>.Filter.Eq("_id", new ObjectId(id));
        var entity = (await Events.FindAsync(filter)).FirstOrDefault();
        return entity;
    }

    public async Task SetStreaming(string id, PrivateEventLiveStreamInfo streamInfo) 
    {
        var filter = Builders<Event>.Filter.Eq("_id", new ObjectId(id));
        var update = Builders<Event>.Update.Set(e => e.LiveStream, streamInfo);
        var result = await Events.UpdateOneAsync(filter, update);
        Console.WriteLine("matchedCount: {0}, modifiedCount: {1}", result.MatchedCount, result.ModifiedCount);
    }
}