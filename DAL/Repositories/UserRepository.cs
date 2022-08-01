using DAL.Models;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : BaseRepository<User, long>
    {
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
        public async Task<bool> ChangePassword(Guid resetKey, string newPassword)
        {
            var context = _dbContext as AppDbContext;
            var user = await context.Users.Where(x => x.resetPasswordKeys.Any(y => y.Id == resetKey && y.ExpirationDate > DateTime.Now && !y.IsUsed)).SingleOrDefaultAsync();
            if (user != null)
            {
                user.Password = newPassword;
                var result = await Update(user);
                return result.Password == newPassword;
            }
            return false;
        }
    }
}
