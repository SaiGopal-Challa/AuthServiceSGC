using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Database
{
    public interface IPgDatabaseExecutor
    {
        Task<TResponse> InsertAsync<TRequest, TResponse>(string tableName, TRequest requestModel, string dbConnString);
        Task<TResponse> UpdateAsync<TRequest, TResponse>(string tableName, TRequest requestModel, string dbConnString);
        Task ExecuteAsync(string sql, object parameters, string dbConnString);
    }
}
