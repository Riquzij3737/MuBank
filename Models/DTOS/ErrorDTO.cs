namespace Mubank.Models
{
    public class ErrorDTO
    {
        public Guid IdError { get; set; }
        public string MessageError { get; set; }
        public int HttpStatusCode { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
