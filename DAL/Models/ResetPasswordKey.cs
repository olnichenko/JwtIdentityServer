using GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ResetPasswordKey : BaseEntity<Guid>
    {
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
        public User User { get; set; }
    }
}
