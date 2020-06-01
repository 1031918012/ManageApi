using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Repositories
{
    public static class IQueryableExtensions
    {
        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        internal static (string, string) ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
            var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var database = (IDatabase)DataBaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            string sql = modelVisitor.Queries.First().ToString();
            string tableAlias = modelVisitor.Queries.First().Tables.First().Alias;
            return (sql, tableAlias);
        }

        /// <summary>
        /// 表相关信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        internal static (string, Dictionary<string, string>) GetTableInfo<TEntity>(this DbContext context) where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
                throw new InvalidOperationException("DbContext does not contain EntitySet for Type: " + typeof(TEntity).Name);
            var relationalData = entityType.Relational();
            string tableName = relationalData.TableName;
            var allProperties = entityType.GetProperties().AsEnumerable();
            string timestampDbTypeName = nameof(TimestampAttribute).Replace("Attribute", "").ToLower();
            var timeStampProperties = allProperties.Where(a => (a.IsConcurrencyToken && a.ValueGenerated == ValueGenerated.OnAddOrUpdate) || a.Relational().ColumnType == timestampDbTypeName);
            var allPropertiesExceptTimeStamp = allProperties.Except(timeStampProperties);
            //var TimeStampColumnName = timeStampProperties.FirstOrDefault()?.Relational().ColumnName;
            var properties = allPropertiesExceptTimeStamp.Where(a => a.Relational().ComputedColumnSql == null);
            var PropertyColumnNamesDict = properties.ToDictionary(a => a.Name, b => b.Relational().ColumnName);
            return (tableName, PropertyColumnNamesDict);
        }


        internal static DbContext GetDbContext(this IQueryable query)
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags).GetValue(query.Provider);
            var queryContextFactory = queryCompiler.GetType().GetField("_queryContextFactory", bindingFlags).GetValue(queryCompiler);
            var dependencies = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", bindingFlags).GetValue(queryContextFactory);
            var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName);
            var stateManagerProperty = queryContextDependencies.GetProperty("StateManager", bindingFlags | BindingFlags.Public).GetValue(dependencies);
            var stateManager = (IStateManager)stateManagerProperty;
            return stateManager.Context;
        }
    }
}
