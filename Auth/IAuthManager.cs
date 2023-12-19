
using PublicParkingsSofiaWebAPI.Controllers;
namespace PublicParkingsSofiaWebAPI.Auth
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(UserLogin user);
        Task<string> CreateToken();
    }
}
