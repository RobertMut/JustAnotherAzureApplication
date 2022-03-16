namespace Domain.Entities
{
    public class File
    {
        public string Filename { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
