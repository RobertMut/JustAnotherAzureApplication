namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<File> Files { get; set; }
    }
}
