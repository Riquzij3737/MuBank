using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Mubank.Models.ModelsHaveShip
{
    public class CardModel
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Account")]
        public Guid AccountId { get; set; }

        [Required]
        public AccountModel Account { get; set; }

        [Required]
        [DataType(DataType.CreditCard)]
        public string CardNumber { get; set; }

        [Required]
        public string CardHolderName { get; set; } 

        [Required]
        public string CardType { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Value { get; set; } = 0;
       
        [Required]
        [DataType(DataType.Date)]
        public DateTime CreationDatae { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }
        
    }
}
