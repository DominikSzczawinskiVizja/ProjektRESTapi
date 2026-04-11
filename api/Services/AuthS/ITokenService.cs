using api.Models;
namespace api.Services.AuthS
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
    }
}
