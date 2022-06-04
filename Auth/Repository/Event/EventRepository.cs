
using Auth.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Auth.Services;

class MongoEventRepository : IEventRepository
{
    private IMongoDatabase db;
    private readonly IMongoClient _client;

    public MongoEventRepository(IMongoClient client)
    {
        db = client.GetDatabase("eventsapp");
        _client = client;
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
}