using Auth.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Auth.Services;

public class MongoUserRepository : IUserRepository
{
    private IMongoDatabase db;
    private readonly IMongoClient _client;

    public MongoUserRepository(IMongoClient client) {
       
        _client = client;
        db = client.GetDatabase("user");
    }
    public async Task Create(User user)
    {
        await collection.InsertOneAsync(user);
    }

    public IMongoCollection<User> collection { 
        get {
            return db.GetCollection<User>("users");
        } 
    }

    public async Task<User?> FindById(string id) 
    {
        
        var filter = Builders<User>.Filter.Eq("Id", new ObjectId(id));
        var user = await collection.Find(filter).FirstOrDefaultAsync();
        return user;
    }

    public async Task<User?> FindByEmail(string email)
    {
        var emailFilter = Builders<User>.Filter.Eq("Email", email);
        var user = await collection.Find(emailFilter).FirstOrDefaultAsync();
        return user;
    }

    public async Task<bool> IsUserExists(string email)
    {
        return (await FindByEmail(email)) != null;
    }

    public async Task<bool> SetEmailVerificationToken(string id, string? token)
    {
        var filter = Builders<User>.Filter.Eq("Id", new ObjectId(id));
        var updateDefination = new UpdateDefinitionBuilder<User>()
                                .Set(e => e.EmailVerificationToken, token);

        var result = await collection.UpdateOneAsync(filter, updateDefination);
        return result.ModifiedCount == 1;
    }

     public async Task<bool> Verify(string id, string token)
    {
        var filter = Builders<User>.Filter
                    .Eq("Id", new ObjectId(id)) 
                    & Builders<User>.Filter
                    .Eq(e => e.EmailVerificationToken, token);
        
        var updateDefination = new UpdateDefinitionBuilder<User>()
                                .Set(e => e.EmailVerificationToken, null)
                                .Set(e => e.Verified, true);

        var result = await collection.UpdateOneAsync(filter, updateDefination);
        return result.ModifiedCount == 1;
    }
}