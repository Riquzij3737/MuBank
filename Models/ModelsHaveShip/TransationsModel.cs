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
        public Guid RemetenteId { get; set; }

        [ForeignKey("RemetenteId")]
        [InverseProperty("TransaçõesEnviadas")]
        public AccountModel Remetente { get; set; }
        
        [Required]
        public Guid DestinatarioId { get; set; }

        [ForeignKey("DestinatarioId")]
        [InverseProperty("TransaçõesRecebidas")]
        public AccountModel Destinatario { get; set; }

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
