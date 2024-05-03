using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using System.Linq;

namespace DBLogic.Contracts
{
    //base interface for all DB interfaces
    public interface IRepositoryBase<T>
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate=null, bool asNoTracking = false, params Expression<Func<T, object>>[] includes);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);

        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate,bool asNoTracking=true);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, params Expression<Func<T, object>>[] includes);



        Task<T> CreateAndSaveAsync(T entity);
        Task<int> CreateAndSaveRangeAsync(List<T> entity);
        Task<int> SaveChangesAsync();
        Task<List<TE>> SqlAsync<TE>(string query);
        Task<List<TE>> SqlAsync<TE>(string query, List<MySqlParameter> parms);
        Task<int> SqlNonQueryAsync(string query, List<MySqlParameter> parms = null);

    
        
        void Create(T entity);
        void CreateRange(List<T> entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(List<T> entities);
        void Save();
    }
}
