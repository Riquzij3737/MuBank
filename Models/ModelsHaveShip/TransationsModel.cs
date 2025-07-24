using Mubank.Models.ModelsHaveShip;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mubank.Models
{
    public class TransationsModel
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid IDDequemfez { get; set; }
        [Required]
        public Guid IDDequemrecebeu { get; set; }

        [ForeignKey("IDDequemfez")]
        [InverseProperty("TransationsMade")]
        public AccountModel AccountDeQuemFez { get; set; }
        [ForeignKey("IDDequemrecebeu")]
        [InverseProperty("TransationsRecived")]
        public AccountModel AccountDeQuemRecebeu { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public string TransationData { get; set; } = DateTime.Now.ToString("dd:MM:yyyy");
    }

}
