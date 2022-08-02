using GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IUserRepository<T, Y> : IBaseRepository<T, Y> where T : class
    {
        Task<bool> ChangePassword(Guid resetKey, string newPassword);
    }
}
