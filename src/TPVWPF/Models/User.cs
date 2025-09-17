using System.ComponentModel.DataAnnotations;


namespace TPVWPF.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Employee,
        Admin
    }
}
