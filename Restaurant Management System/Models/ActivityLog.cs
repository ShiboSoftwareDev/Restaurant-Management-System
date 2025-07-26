namespace Restaurant_Management_System.Models
{
    public class ActivityLog
    {
        public int      LogId       { get; set; }
        public string   EventName   { get; set; }
        public string   Description { get; set; }
        public string   Username    { get; set; }
        public DateTime LogTime     { get; set; }
    }
}
