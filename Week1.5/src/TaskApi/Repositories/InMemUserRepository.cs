using TaskApi.Models;

namespace TaskApi.Repositories;

public class InMemUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private readonly object _lock = new(); //prevents race conditions

    public Task<User?> GetByUsernameAsync(string userName, CancellationToken ct = default)
    {
        lock (_lock) //locked threads
        {
            return Task.FromResult(
                _users.FirstOrDefault(u =>
                    u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                )
            );
        }
    }

    public Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }
    }

    public Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _users.Add(user);
            return Task.FromResult(user);
        }
    }
}
