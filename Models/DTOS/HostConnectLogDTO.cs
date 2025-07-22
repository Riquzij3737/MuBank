namespace Mubank.Models
{
    public class HostConnectLogDTO
    {
        public Guid ConnectID { get; set; }
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
    }
}
