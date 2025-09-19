using TaskApi.Models;

namespace TaskApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string userName, CancellationToken ct = default);
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<User> CreateAsync(User user, CancellationToken ct = default);
}
