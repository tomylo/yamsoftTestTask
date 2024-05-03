using DAL;
using DAL.Models;
using DBLogic.Contracts;

namespace DBLogic.Repositories
{
    public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        private YSDbContext _context;
        public RefreshTokenRepository(YSDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
