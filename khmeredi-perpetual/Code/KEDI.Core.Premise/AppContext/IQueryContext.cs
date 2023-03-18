using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CKBS.AppContext;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Utilities.Extionsions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KEDI.Core.Premise.AppContext
{
    public interface IQueryContext
    {
        SqlConnection NewSqlConnection(string connectionString = "");
        SqlCommand FromSqlCommand(SqlConnection connection, string tableName);
        SqlCommand FromSqlCommand(SqlConnection connection, Type entityType);
        IEnumerable<TEntity> Query<TEntity>(string tableName) where TEntity : class, new();
        IEnumerable<TEntity> Query<TEntity>() where TEntity : class, new();
        IEnumerable<Dictionary<string, object>> ToDictionaries(string tableName);
        IEnumerable<Dictionary<string, object>> ToDictionaries(Type typeOfEntity);
        Task<IEnumerable<Dictionary<string, object>>> ToDictionariesAsync(string tableName);
        Task<IEnumerable<Dictionary<string, object>>> ToDictionariesAsync(Type typeOfEntity);
        string GetTableName(Type entityType);
    }

    public class QueryContext : IQueryContext
    {
        private readonly ILogger<QueryContext> _logger;
        private readonly DataContext _dataContext;
        private readonly string _connectionString;
        public QueryContext(ILogger<QueryContext> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
            _connectionString = dataContext.Database.GetDbConnection().ConnectionString;
        }

        public virtual SqlConnection NewSqlConnection(string connectionString = "")
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = _connectionString;
            }
            return new SqlConnection(connectionString);
        }

        public SqlCommand FromSqlCommand(SqlConnection connection, string tableName)
        {
            connection.Open();
            var command = new SqlCommand($"uspSelectTable", connection);
            command.Parameters.AddWithValue("@tableName", tableName);
            command.CommandType = CommandType.StoredProcedure;
            return command;
        }

        public SqlCommand FromSqlCommand(SqlConnection connection, Type entityType)
        {
            var tableName = GetTableName(entityType);
            return FromSqlCommand(connection, tableName);
        }

        public Task<IEnumerable<Dictionary<string, object>>> ToDictionariesAsync(Type typeOfEntity)
        {
            string tableName = GetTableName(typeOfEntity);
            return ToDictionariesAsync(tableName);
        }

        public async Task<IEnumerable<Dictionary<string, object>>> ToDictionariesAsync(string tableName)
        {
            using var conn = new SqlConnection(_connectionString);
            var reader = await FromSqlCommand(conn, tableName).ExecuteReaderAsync();
            var dict = reader.ToDictionaries();
            return dict;
        }

        public IEnumerable<Dictionary<string, object>> ToDictionaries(Type typeOfEntity)
        {
            string tableName = GetTableName(typeOfEntity);
            return ToDictionaries(tableName);
        }

        public IEnumerable<Dictionary<string, object>> ToDictionaries(string tableName)
        {
            using var conn = new SqlConnection(_connectionString);
            var reader = FromSqlCommand(conn, tableName).ExecuteReader();
            return reader.ToDictionaries();
        }

        public IEnumerable<TEntity> Query<TEntity>(string tableName) where TEntity : class, new()
        {
            using var conn = new SqlConnection(_connectionString);
            var reader = FromSqlCommand(conn, tableName).ExecuteReader();
            return reader.Set<TEntity>();
        }

        public IEnumerable<TEntity> Query<TEntity>() where TEntity : class, new()
        {
            var tableName = GetTableName(typeof(TEntity));
            return Query<TEntity>(tableName);
        }

        public string GetTableName<TEntity>() where TEntity : class
        {
            return GetTableName(typeof(TEntity));
        }

        public string GetTableName(Type entityType)
        {
            IAnnotation annotation = _dataContext.Model.FindEntityType(entityType).FindAnnotation("Relational:TableName");
            if(annotation == null) {
                _logger.LogError($"Table annotation of {entityType.Name} is not found.");
                return string.Empty;
            }
            return $"{annotation.Value}";
        }
    }
}