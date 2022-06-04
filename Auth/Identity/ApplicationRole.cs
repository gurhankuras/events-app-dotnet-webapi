using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace mongoidentity
{
    [CollectionName("Roles")]
    public class ApplicationRole: MongoIdentityRole<Guid>
    {
    }
}
