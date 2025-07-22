using System.ComponentModel.DataAnnotations;

namespace Mubank.Models
{
    public class IPsBlockedModel
    {
        [Key]
        [Required]
        [StringLength(50)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(40)]
        public string Ip { get; set; }        
        [StringLength(200)]
        [DataType(DataType.Text)]
        public string? Reason { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateBlocked { get; set; } = DateTime.Now;  

    }
}
