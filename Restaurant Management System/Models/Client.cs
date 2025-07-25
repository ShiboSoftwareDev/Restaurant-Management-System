namespace Restaurant_Management_System.Models
{
    public class Client
    {
        public int    ClientId      { get; set; }
        public string Name          { get; set; }
        public int    LoyaltyPoints { get; set; }
        public bool   IsDeleted     { get; set; }
    }
}
