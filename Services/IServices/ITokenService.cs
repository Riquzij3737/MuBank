using Mubank.Models;

namespace Mubank.Services.IServices
{
    public interface ITokenService
    {
        public string GenerationToken(UserModel Claim);                
    }
}
