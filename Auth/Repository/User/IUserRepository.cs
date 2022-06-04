
using Auth.Models;

namespace Auth.Services;
public interface IUserRepository {
    Task Create(User user);
    Task<User?> FindById(string id);
    Task<User?> FindByEmail(string email);
    Task<bool> IsUserExists(string email);
    Task<bool> SetEmailVerificationToken(string id, string? token);
    Task<bool> Verify(string id, string token);
}