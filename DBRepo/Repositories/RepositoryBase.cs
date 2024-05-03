using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DAL;
using DBLogic.Contracts;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace DBLogic.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private YSDbContext RepositoryContext { get; set; }
        
        private static List<Type> SimpleTypes = new List<Type>()
        {
            typeof(string),
            typeof(bool),
            typeof(bool?),
            typeof(int),
            typeof(byte),
            typeof(sbyte),
            typeof(uint),
            typeof(ulong),
            typeof(short),
            typeof(short?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(long),
            typeof(float),
            typeof(decimal),
            typeof(decimal?),
            typeof(UInt64),
            typeof(UInt64?),
            typeof(double),
            typeof(double?)
        };

        protected RepositoryBase(YSDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            return await GetFirstOrDefaultAsync(predicate, false, includes);
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate = null,bool asNoTracking=false, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = RepositoryContext.Set<T>();


            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includes.Any())
            {
                query = includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            if (asNoTracking)
            {
              return await query.AsNoTracking().FirstOrDefaultAsync();
            }

            var queryStr = query.ToQueryString();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await GetAllAsync(null);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true)
        {
            return await GetAllAsync(predicate, pageNumber: null, pageSize: null, asNoTracking);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        {
            return await GetAllAsync(predicate, null, null,asNoTracking, null,includes);
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, int? pageNumber = null, int? pageSize = null, bool asNoTracking = false, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = RepositoryContext.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includes!=null && includes.Any())
            {
                query = includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            if (pageSize is > 0 )
            {
                var skip= pageNumber==1 ? 0 : pageNumber.Value * pageSize.Value;
                var q = query.Skip(skip).Take(pageSize.Value);
                var queryPageString = q.ToQueryString();
                 return await q?.ToListAsync();
            }

            var queryString = query.ToQueryString();
            return await query?.ToListAsync();
        }

        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }
        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }
        public void CreateRange(List<T> entity)
        {
            RepositoryContext.Set<T>().AddRange(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
        }

        public void Delete(List<T> entities)
        {
            RepositoryContext.Set<T>().RemoveRange(entities);
        }

        public void Save()
        {
            RepositoryContext.SaveChanges();
        }

        public async Task<int> CreateAndSaveRangeAsync(List<T> entity)
        {
            await RepositoryContext.Set<T>().AddRangeAsync(entity);
            return await RepositoryContext.SaveChangesAsync();
        }

        public async Task<T> CreateAndSaveAsync(T entity)
        {
            await RepositoryContext.Set<T>().AddAsync(entity);
            await RepositoryContext.SaveChangesAsync();
            
            return await Task.FromResult(entity);
        }


        public async Task<int> SaveChangesAsync()
        {
            return await RepositoryContext.SaveChangesAsync();
        }

        private async Task<List<TE>> SqlAsyncWithOutParam<TE>(string query, List<MySqlParameter> parms)
        {
            await using var command = RepositoryContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.StoredProcedure;
            if (parms != null && parms.Any())
            {
                command.Parameters.AddRange(parms.ToArray());
            }

            await RepositoryContext.Database.OpenConnectionAsync();

            List<TE> list = new List<TE>();
            await using (var result = await command.ExecuteReaderAsync())
            {
                TE obj;
                while (await result.ReadAsync())
                {
                    obj = Activator.CreateInstance<TE>();
                    if (SimpleTypes.Contains(typeof(TE)))
                    {
                        list.Add((TE)result[0]);
                        continue;
                    }

                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (IsAllowed(prop) && !object.Equals(result[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, result[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                }
            }
            await RepositoryContext.Database.CloseConnectionAsync();
            return list;
        }


        public async Task<List<TE>> SqlAsync<TE>(string query, List<MySqlParameter> parms )
        {
            await using var command = RepositoryContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            if (parms != null && parms.Any())
            {
                if (parms.Any(a => a.Direction == ParameterDirection.Output))
                {
                    return await SqlAsyncWithOutParam<TE>(query, parms);
                }
                command.Parameters.AddRange(parms.ToArray());
            }

            await RepositoryContext.Database.OpenConnectionAsync();

            List<TE> list = new List<TE>();
            if (parms != null && parms.Any(a => a.Direction == ParameterDirection.Output))
            {
                var res =await command.ExecuteScalarAsync();
                
            }
            
            await using (var result = await command.ExecuteReaderAsync())
            {
                TE obj;
                while (await result.ReadAsync())
                {
                    obj = Activator.CreateInstance<TE>();
                    
                    if (SimpleTypes.Contains(typeof(TE)))
                    {
                        list.Add((TE)result[0]);
                        continue;
                    }

                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (IsAllowed(prop) && !object.Equals(result[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, result[prop.Name], null);
                        }
                    }

                    list.Add(obj);
                }
            }
            await RepositoryContext.Database.CloseConnectionAsync();
            return list;
        }

        public async Task<List<TE>> SqlAsync<TE>(string query)
        {
            return await SqlAsync<TE>(query, null);
        }

        public async Task<int> SqlNonQueryAsync(string query, List<MySqlParameter> parms=null)
        {
            if (parms != null)
            {
                return await RepositoryContext.Database.ExecuteSqlRawAsync(query, parms);
            }

            return await RepositoryContext.Database.ExecuteSqlRawAsync(query);
        }

        private static bool IsAllowed(PropertyInfo prop)
        {
            if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
            {
                return false;
            }
            return SimpleTypes.Contains(prop.PropertyType);
        }

      
    }
}
