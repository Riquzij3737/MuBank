using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Runtime.CompilerServices;

namespace Mubank.Models
{
    public class HostConnectLogModel
    {
        [Key]
        [Required]
        public Guid ConnectID { get; set; }
        [Required]
        [StringLength(50)]
        public string IpAddress { get; set; }
        [Required]
        [StringLength(300)]
        public string Message { get; set; }
        [Required]
        [Length(1, 65536)]
        public int Port { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime date { get; set; } = DateTime.Now;

    }
}
