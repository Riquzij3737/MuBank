using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mubank.Models
{
    public class UserDataFULLModel
    {
        [Key]
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public UserModel User { get; set; }
        [Required]
        [StringLength(100)]
        public string Country { get; set; } = "Unknown";        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
