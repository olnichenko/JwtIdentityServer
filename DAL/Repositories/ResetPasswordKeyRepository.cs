using DAL.Models;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ResetPasswordKeyRepository : BaseRepository<ResetPasswordKey, Guid>
    {
        public ResetPasswordKeyRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<ResetPasswordKey> Create(ResetPasswordKey resetPasswordKey, User user)
        {
            resetPasswordKey.User = user;
            if (user.resetPasswordKeys == null)
            {
                user.resetPasswordKeys = new List<ResetPasswordKey>();
            }
            user.resetPasswordKeys.Add(resetPasswordKey);
            _dbContext.Entry(user).State = EntityState.Modified;
            _dbContext.Add(resetPasswordKey);
            await _dbContext.SaveChangesAsync();
            return resetPasswordKey;
        }
    }
}
