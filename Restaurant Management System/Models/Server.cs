namespace Restaurant_Management_System.Models
{
    public class Server
    {
        public int  ServerId  { get; set; }
        public string Name    { get; set; }
        public bool IsDeleted { get; set; }   // 0 = active | 1 = deleted
    }
}
