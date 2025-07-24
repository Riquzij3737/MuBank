using Mubank.Models;
using Mubank.Models.ModelsHaveShip;

namespace Mubank.Services.IServices
{
    public interface ITokenService
    {
        public string GenerationToken(AccountModel Claim);                
    }
}
