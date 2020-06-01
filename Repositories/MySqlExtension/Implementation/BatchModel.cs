using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class BatchModel
    {
        public BatchModel(string sql, List<MySqlParameter> par)
        {
            this.Sql = sql;
            this.ParameterList = par;
        }
        public string Sql { get; set; }
        public List<MySqlParameter> ParameterList { get; set; }
    }
}
