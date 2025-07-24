using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mubank.Models.ModelsHaveShip
{
    public class AccountModel
    {        
        [Required]
        [Key,ForeignKey("UserAccount")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public UserModel UserAccount { get; set; }

        [Required]
        public string RoleName { get; set; } = "User";

        [Range(0, int.MaxValue)]
        public decimal Value { get; set; } = 0;
        
        public CardModel? Card { get; set; }

        [InverseProperty("AccountDeQuemFez")]
        public List<TransationsModel> TransationsRecived { get; set; }
        [InverseProperty("AccountDeQuemRecebeu")]
        public List<TransationsModel> TransationsMade { get; set; }
    }
}
