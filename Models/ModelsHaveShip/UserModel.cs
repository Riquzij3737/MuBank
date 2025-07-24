using Mubank.Models.ModelsHaveShip;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mubank.Models
{
    public class UserModel
    {
        [Key]
        [Required]
        public Guid Id { get; set; }        

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [InverseProperty("UserAccount")]
        public AccountModel Account { get; set; }

        public UserModel()
        {
            Id = Guid.NewGuid();
        }
    }

}
