using DAL.Models;
using GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IResetPasswordKeyRepository<T, Y> : IBaseRepository<T, Y> where T : class
    {
        Task<ResetPasswordKey> Create(ResetPasswordKey resetPasswordKey, User user);
    }
}
