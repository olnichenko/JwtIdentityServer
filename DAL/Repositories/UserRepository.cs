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
    }
}
