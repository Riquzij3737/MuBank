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
        public string RoleName { get; set; } = "NormalUser";

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

        [Length(0,int.MaxValue)]
        public decimal Value { get; set; } = 0;

        public UserModel()
        {
            Id = Guid.NewGuid();
        }
    }

}
