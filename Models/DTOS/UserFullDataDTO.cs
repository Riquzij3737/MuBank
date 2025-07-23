namespace Mubank.Models.DTOS
{
    public class UserFullDataDTO
    {
        public Guid UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }        
        public string RoleName { get; set; }
        public string? JwtToken { get; set; }        
        public DateTime DateCreated { get; set; }
        public List<TransationsModel>? Transactions { get; set; }


    }
}
