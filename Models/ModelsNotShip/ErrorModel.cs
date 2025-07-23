using System.ComponentModel.DataAnnotations;

namespace Mubank.Models
{
    public class ErrorModel
    {
        [Key]
        [Required]
        public Guid IdError { get; set; }
        [Required]
        [StringLength(100)]
        [DataType(DataType.Text)]
        public string MessageError { get; set; }
        [Required]   
        public int HttpStatusCode { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date {  get; set; } = DateTime.Now;

        public ErrorModel()
        {
            IdError = Guid.NewGuid();
        }
    }
}
