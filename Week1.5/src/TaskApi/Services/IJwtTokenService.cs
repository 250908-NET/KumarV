using TaskApi.Models;

namespace TaskApi.Services;

public interface IJwtTokenService
{
    string Create(string userId, string userName);
}
