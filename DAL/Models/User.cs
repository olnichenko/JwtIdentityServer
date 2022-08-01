using GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity<long>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
