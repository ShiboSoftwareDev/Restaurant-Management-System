namespace Restaurant_Management_System.Models
{
    public class MenuItem
    {
        public int     ItemId    { get; set; }
        public string  Name      { get; set; }
        public decimal Price     { get; set; }
        public string  Category  { get; set; }
        public bool    IsDeleted { get; set; }   // 0 = active | 1 = deleted
    }
}
