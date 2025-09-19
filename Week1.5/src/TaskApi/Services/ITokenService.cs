namespace TaskApi.Services;

public interface ITokenService
{
    string CreateToken(string userId, string userName);
}
