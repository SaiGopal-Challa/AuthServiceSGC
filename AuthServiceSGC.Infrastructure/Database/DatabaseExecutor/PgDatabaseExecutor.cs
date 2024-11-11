using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace AuthServiceSGC.Infrastructure.Database.DatabaseExecutor
{
    internal class PgDatabaseExecutor: IPgDatabaseExecutor
    {
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

        public async Task<TResponse> InsertAsync<TRequest, TResponse>(string tableName, TRequest requestModel, string dbConnString)
        {
            var (columns, values) = BuildInsertStatement(requestModel);
            var query = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";

            await using var connection = new NpgsqlConnection(dbConnString);
            await using var command = new NpgsqlCommand(query, connection);

            try
            {
                await connection.OpenAsync();
                BindParameters(command, requestModel);
                await command.ExecuteNonQueryAsync();
                // Commit transaction if needed ( need to add later )
            }
            catch (NpgsqlException ex)
            {
                // Log and handle exceptions as needed
                throw new Exception("Error executing insert operation", ex);
            }

            return default; // Return appropriate response (e.g., new record ID or status)
        }

        public async Task<TResponse> UpdateAsync<TRequest, TResponse>(string tableName, TRequest requestModel, string dbConnString)
        {
            var (setClause, keyColumn) = BuildUpdateStatement(requestModel);
            var query = $"UPDATE {tableName} SET {setClause} WHERE {keyColumn} = @{keyColumn}";

            await using var connection = new NpgsqlConnection(dbConnString);
            await using var command = new NpgsqlCommand(query, connection);

            try
            {
                await connection.OpenAsync();
                BindParameters(command, requestModel);
                await command.ExecuteNonQueryAsync();
            }
            catch (NpgsqlException ex)
            {
                // Log and handle exceptions as needed
                throw new Exception("Error executing update operation", ex);
            }

            return default; // Return appropriate response (e.g., success/failure status)
        }

        public async Task ExecuteAsync(string sql, object parameters, string dbConnString)
        {
            await using var connection = new NpgsqlConnection(dbConnString);
            await using var command = new NpgsqlCommand(sql, connection);

            try
            {
                await connection.OpenAsync();
                BindParameters(command, parameters);
                await command.ExecuteNonQueryAsync();
            }
            catch (NpgsqlException ex)
            {
                // Log and handle exceptions as needed
                throw new Exception("Error executing SQL command", ex);
            }
        }

        private (List<string> columns, List<string> values) BuildInsertStatement<TRequest>(TRequest requestModel)
        {
            var columns = new List<string>();
            var values = new List<string>();

            var properties = GetCachedProperties<TRequest>();
            foreach (var property in properties)
            {
                columns.Add(property.Name);
                values.Add($"@{property.Name}");
            }

            return (columns, values);
        }

        private (string setClause, string keyColumn) BuildUpdateStatement<TRequest>(TRequest requestModel)
        {
            var setClauses = new List<string>();
            string keyColumn = GetKeyColumn<TRequest>();

            var properties = GetCachedProperties<TRequest>();
            foreach (var property in properties)
            {
                if (property.Name != keyColumn)
                {
                    setClauses.Add($"{property.Name} = @{property.Name}");
                }
            }

            var setClause = string.Join(", ", setClauses);
            return (setClause, keyColumn);
        }

        private void BindParameters(NpgsqlCommand command, object model)
        {
            if (model == null) return;

            var properties = GetCachedProperties(model.GetType());
            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                command.Parameters.AddWithValue($"@{property.Name}", value ?? DBNull.Value);
            }
        }

        private PropertyInfo[] GetCachedProperties<T>()
        {
            return GetCachedProperties(typeof(T));
        }

        private PropertyInfo[] GetCachedProperties(Type type)
        {
            if (!_propertyCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties();
                _propertyCache[type] = properties;
            }
            return properties;
        }

        private string GetKeyColumn<TRequest>()
        {
            var properties = GetCachedProperties(typeof(TRequest));
            return properties.FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)))?.Name;
        }

        //building for stored procedure execution
        // use transactions(commits & rollbacks) , request & response parameters binder , also see if to use auto mapper
        public async Task<TResponse> ExecuteSpAsync<TRequest, TResponse>(string spName, TRequest requestModel, TResponse responseModel, string dbConnString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(dbConnString) )
                {
                    //build scope level objects within scope, also update required objects within scope
                    try
                    {

                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {

            }
            return default;
        }
    
        // executor => orm ; Make sure DTOs parameter names are same as column names / DB level parameter names (could be view columns etc)
    
    }
}
