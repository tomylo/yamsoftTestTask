
using DAL;
using DBLogic.Contracts;
using System;
using System.Collections.Generic;

namespace DBLogic.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly YSDbContext _repoContext;
        private IRefreshTokenRepository _refreshTokenRepository;

        public RepositoryWrapper(YSDbContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }
        public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(_repoContext);
        

       
    }
}
