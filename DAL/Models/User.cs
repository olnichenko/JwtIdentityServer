using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity<long>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<ResetPasswordKey>? resetPasswordKeys { get; set; }
    }
}
