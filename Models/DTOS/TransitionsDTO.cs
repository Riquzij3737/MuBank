namespace Mubank.Models
{
    public class TransitionsDTO
    {
        public Guid Id { get; set; }
        public Guid IDDequemfez { get; set; }
        public Guid IDDequemrecebeu { get; set; }
        public string title {  get; set; }
        public string description { get; set; }
        public decimal Value { get; set; }
        public string TransationData { get; set; } = DateTime.Now.ToString("dd:MM:yyyy");   
    }
}
