using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace mongoidentity
{
    [CollectionName("Users")]
    public class ApplicationUser: MongoIdentityUser<Guid>
    {
        public LinkedInInfo? LinkedinInfo { get; set; }
    }
}

public class LinkedInInfo {
    public string AccessToken { get; set; }
    public string Expires { get; set; }
}
