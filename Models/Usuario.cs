namespace RpgApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set;}
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}