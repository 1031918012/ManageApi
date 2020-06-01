using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repositories
{
    public static class Batch
    {
        public static string GetSqlDelete<T>(IQueryable<T> query) where T : class, new()
        {
            (string sql, string tableAlias) = GetBatchSql(query, OperationType.Delete);
            sql = $"DELETE `{tableAlias}` {sql}";
            return sql;
        }
        public static (string, List<MySqlParameter>) GetSqlUpdate<T>(IQueryable<T> query, Expression<Func<T, T>> expression) where T : class, new()
        {
            (string, List<MySqlParameter>) sql;
            (string sqlWhere, string tableAlias) = GetBatchSql(query, OperationType.Update);
            (string tableName, Dictionary<string, string> columnNameValueDict) = query.GetDbContext().GetTableInfo<T>();

            var sqlColumns = new StringBuilder();
            var sqlParameters = new List<MySqlParameter>();
            CreateUpdateBody(columnNameValueDict, tableAlias, expression.Body, ref sqlColumns, ref sqlParameters);
            if (string.IsNullOrEmpty(sqlWhere))
            {
                sql = ($"UPDATE `{tableName}` SET {sqlColumns.ToString()}", sqlParameters);
            }
            else
            {
                sql = ($"UPDATE `{tableName}` as `{tableAlias}` SET {sqlColumns.ToString()} {sqlWhere}", sqlParameters);
            }
            return sql;
        }
        #region 批量修改

        //public static (string, List<MySqlParameter>) GetSqlUpdate<T>(IQueryable<T> query, T updateValues, List<string> updateColumns = null) where T : class, new()
        //{
        //    (string sql, string tableAlias) = GetBatchSql(query, OperationType.Update);
        //    (string tableName, Dictionary<string, string> columnNameValueDict) = query.GetDbContext().GetTableInfo<T>();
        //    (string sqlColumns, List<MySqlParameter> sqlParameters) = GetSqlSetSegment(columnNameValueDict, updateValues, updateColumns);
        //    return ($"UPDATE `{tableName}` as `{tableAlias}` SET {sqlColumns}  {sql}", sqlParameters);
        //}


        //public static (string, List<MySqlParameter>) GetSqlSetSegment<T>(Dictionary<string, string> columnNameValueDict, T updateValues, List<string> updateColumns) where T : class, new()
        //{
        //    StringBuilder sqlColumns = new StringBuilder();
        //    List<MySqlParameter> sqlParameters = new List<MySqlParameter>();

        //    Type updateValuesType = typeof(T);
        //    var defaultValues = new T();
        //    foreach (var propertyNameColumnName in columnNameValueDict)
        //    {
        //        string propertyName = propertyNameColumnName.Key;
        //        string columnName = propertyNameColumnName.Value;
        //        var pArray = propertyName.Split(new char[] { '.' });
        //        Type lastType = updateValuesType;
        //        PropertyInfo property = lastType.GetProperty(pArray[0]);
        //        object propertyUpdateValue = property.GetValue(updateValues);
        //        object propertyDefaultValue = property.GetValue(defaultValues);
        //        for (int i = 1; i < pArray.Length; i++)
        //        {
        //            lastType = property.PropertyType;
        //            property = lastType.GetProperty(pArray[i]);
        //            propertyUpdateValue = propertyUpdateValue != null ? property.GetValue(propertyUpdateValue) : propertyUpdateValue;
        //            var lastDefaultValues = lastType.Assembly.CreateInstance(lastType.FullName);
        //            propertyDefaultValue = property.GetValue(lastDefaultValues);
        //        }

        //        bool isDifferentFromDefault = propertyUpdateValue != null && propertyUpdateValue?.ToString() != propertyDefaultValue?.ToString();
        //        if (isDifferentFromDefault || (updateColumns != null && updateColumns.Contains(propertyName)))
        //        {
        //            sqlColumns.Append($"`{columnName}` = @{columnName}, ");
        //            sqlParameters.Add(new MySqlParameter($"@{columnName}", propertyUpdateValue));
        //        }
        //    }
        //    if (String.IsNullOrEmpty(sqlColumns.ToString()))
        //    {
        //        throw new InvalidOperationException("SET Columns not defined. If one or more columns should be updated to theirs default value use 'updateColumns' argument.");
        //    }
        //    var sqlColumn = sqlColumns.ToString().Remove(sqlColumns.Length - 2, 2); // removes last excess comma and space: ", "
        //    return (sqlColumn, sqlParameters);
        //}

        #endregion

        public static (string, List<MySqlParameter>) GetSqlInsert<T>(this DbContext context, List<T> list) where T : class, new()
        {
            (string tableName, Dictionary<string, string> columnNameValueDict) = context.GetTableInfo<T>();
            var sqlColumns = new StringBuilder();
            var sqlValues = new StringBuilder();
            var sqlParameters = new List<MySqlParameter>();
            CreateInsertBody(columnNameValueDict, list, ref sqlColumns, ref sqlValues, ref sqlParameters);
            var insert = new StringBuilder()
                   .Append($" INSERT INTO `{tableName}` ")
                   .Append($"({sqlColumns})")
                   .Append(" VALUES")
                   .Append(sqlValues)
                   .ToString();
            return (insert, sqlParameters);
        }


        public static (string, string) GetBatchSql<T>(IQueryable<T> query, OperationType operationType) where T : class, new()
        {
            (string sqlQuery, string tableAlias) = query.ToSql();

            string sql = string.Empty;
            if (operationType == OperationType.Delete)
            {
                int indexForm = sqlQuery.IndexOf(Environment.NewLine);
                sql = sqlQuery.Substring(indexForm, sqlQuery.Length - indexForm);
            }
            else if (operationType == OperationType.Update)
            {
                int indexWHERE = sqlQuery.IndexOf("WHERE");
                sql = indexWHERE > 0 ? sqlQuery.Substring(indexWHERE, sqlQuery.Length - indexWHERE) : sql;
            }
            sql = sql.Contains("{") ? sql.Replace("{", "{{") : sql;
            sql = sql.Contains("}") ? sql.Replace("}", "}}") : sql;
            return (sql, tableAlias);
        }
        public static void CreateUpdateBody(Dictionary<string, string> columnNameValueDict, string tableAlias, Expression expression, ref StringBuilder sqlColumns, ref List<MySqlParameter> sqlParameters)
        {
            if (expression is MemberInitExpression memberInitExpression)
            {
                foreach (var item in memberInitExpression.Bindings)
                {
                    if (item is MemberAssignment assignment)
                    {
                        if (columnNameValueDict.TryGetValue(assignment.Member.Name, out string value))
                            sqlColumns.Append($" `{tableAlias}`.`{value}`");
                        else
                            sqlColumns.Append($" `{tableAlias}`.`{assignment.Member.Name}`");

                        sqlColumns.Append(" =");

                        CreateUpdateBody(columnNameValueDict, tableAlias, assignment.Expression, ref sqlColumns, ref sqlParameters);

                        if (memberInitExpression.Bindings.IndexOf(item) < (memberInitExpression.Bindings.Count - 1))
                            sqlColumns.Append(" ,");
                    }
                }
            }

            if (expression is MemberExpression memberExpression)
            {
                if (columnNameValueDict.TryGetValue(memberExpression.Member.Name, out string value))
                    sqlColumns.Append($" `{tableAlias}`.`{value}`");
                else
                    sqlColumns.Append($" `{tableAlias}`.`{memberExpression.Member.Name}`");
            }

            if (expression is ConstantExpression constantExpression)
            {
                var parmName = $"param_{sqlParameters.Count}";
                sqlParameters.Add(new MySqlParameter(parmName, constantExpression.Value));
                sqlColumns.Append($" @{parmName}");
            }

            if (expression is UnaryExpression unaryExpression)
            {
                switch (unaryExpression.NodeType)
                {
                    case ExpressionType.Convert:
                        CreateUpdateBody(columnNameValueDict, tableAlias, unaryExpression.Operand, ref sqlColumns, ref sqlParameters);
                        break;
                    case ExpressionType.Not:
                        sqlColumns.Append(" ~");//this way only for SQL Server 
                        CreateUpdateBody(columnNameValueDict, tableAlias, unaryExpression.Operand, ref sqlColumns, ref sqlParameters);
                        break;
                    default: break;
                }
            }

            if (expression is BinaryExpression binaryExpression)
            {
                CreateUpdateBody(columnNameValueDict, tableAlias, binaryExpression.Left, ref sqlColumns, ref sqlParameters);

                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.Add:
                        sqlColumns.Append(" +");
                        break;
                    case ExpressionType.Divide:
                        sqlColumns.Append(" /");
                        break;
                    case ExpressionType.Multiply:
                        sqlColumns.Append(" *");
                        break;
                    case ExpressionType.Subtract:
                        sqlColumns.Append(" -");
                        break;
                    default: break;
                }

                CreateUpdateBody(columnNameValueDict, tableAlias, binaryExpression.Right, ref sqlColumns, ref sqlParameters);
            }
        }



        public static void CreateInsertBody<T>(Dictionary<string, string> columnNameValueDict, List<T> list, ref StringBuilder sqlColumns, ref StringBuilder sqlValues, ref List<MySqlParameter> sqlParameters)
        {
            var column = new List<string>();
            foreach (var item in columnNameValueDict)
            {
                column.Add($"`{item.Value}`");
            }
            sqlColumns.Append(string.Join(",", column));

            var rows = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                var row = new List<object>();
                foreach (var item in columnNameValueDict)
                {
                    var value = list[i].GetType().GetProperty(item.Key).GetValue(list[i]);
                    var type = list[i].GetType().GetProperty(item.Key).PropertyType;
                    row.Add($"@{item.Value}_{i}");

                    AddParameter(type, value, $"{item.Value}_{i}", sqlParameters);
                }
                rows.Add("(" + string.Join(",", row) + ")");
            }
            sqlValues.Append(string.Join(",", rows));
        }


        private static void AddParameter(Type type, object value, string name, List<MySqlParameter> sqlParameters)
        {
          if (type.IsEnum)
            {
                if (value != null)
                {
                    var enumUnderlyingType = type.GetEnumUnderlyingType();
                    value = Convert.ChangeType(value, enumUnderlyingType);
                }
            }
            sqlParameters.Add(new MySqlParameter(name, value));
        }
        private static bool IsDateType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return IsDateType(Nullable.GetUnderlyingType(type));

            return type == typeof(DateTime) || type == typeof(DateTimeOffset);
        }
    }
}
