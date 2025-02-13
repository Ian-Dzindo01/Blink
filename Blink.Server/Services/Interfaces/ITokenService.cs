using Blink.Models;

namespace Blink.Service.Interfaces
{
    public interface ITokenService
    {
        string CreateJWT(User user);
        public string GetJWT();
        public bool JWTIsExpired(string token);
        public void SetJWT(string jwt);
    }
}