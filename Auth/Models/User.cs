using MongoDB.Bson;

namespace Auth.Models;

public class User 
{
    public ObjectId Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Verified { get; set; } = false;
    public string EmailVerificationToken { get; set; }
}