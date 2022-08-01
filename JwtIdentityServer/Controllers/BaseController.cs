using DAL;
using Microsoft.AspNetCore.Mvc;

namespace JwtIdentityServer.Controllers
{
    public class BaseController : ControllerBase, IDisposable
    {
        protected readonly AppDbContext _appDbContext;

        public BaseController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public void Dispose()
        {
            _appDbContext.Dispose();
        }
    }
}
