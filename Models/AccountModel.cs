using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mubank.Models
{
    public class AccountModel
    {
        [Key]
        public Guid Id { get; set; }

        // FK para User
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; }

        // Transações enviadas: esse account foi o remetente
        [InverseProperty("Remetente")]
        public List<TransationsModel> TransaçõesEnviadas { get; set; }

        // Transações recebidas: esse account foi o destinatário
        [InverseProperty("Destinatario")]
        public List<TransationsModel> TransaçõesRecebidas { get; set; }
    }

}
