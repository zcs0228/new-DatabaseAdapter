using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Infrastructure
{
    public class MSSQLHelper
    {
        public static string[] GetDataTableColumnName(DataTable dt, string[] excludeColumn)
        {
            IList<string> result = new List<string>();

            foreach (DataColumn item in dt.Columns)
            {
                if (!excludeColumn.Contains(item.ColumnName))
                {
                    result.Add(item.ColumnName);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 将DataRow转换为Insert语句
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="dr">数据源</param>
        /// <param name="columns">列名</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static string DataRowToInsert(string tableName, DataRow dr, string[] columns, List<SqlParameter> parameters)
        {
            string baseString = "INSERT INTO ";
            StringBuilder insertStr = new StringBuilder();
            insertStr.Append(baseString).Append(tableName);

            StringBuilder columnStr = new StringBuilder("(");
            StringBuilder valueStr = new StringBuilder("(");
            foreach (string item in columns)
            {
                string guid = Guid.NewGuid().ToString().Replace("-", "");

                columnStr.Append(item).Append(",");
                valueStr.Append("@I").Append(guid).Append(",");
                parameters.Add(new SqlParameter("@I" + guid, dr[item]));
            }
            columnStr.Remove(columnStr.Length - 1, 1).Append(")");
            valueStr.Remove(valueStr.Length - 1, 1).Append(")");
            insertStr.Append(" ").Append(columnStr.ToString());
            insertStr.Append(" VALUES ").Append(valueStr.ToString());

            return insertStr.ToString();
        }

        /// <summary>
        /// 将DataRow转换为Update语句
        /// </summary>
        /// <param name="tableName">数据库表名</param>
        /// <param name="dr">数据源</param>
        /// <param name="columns">列名</param>
        /// <param name="keyColumnName">判断条件列名</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        public static string DataRowToUpdate(string tableName, DataRow dr, string[] columns,
            string[] keyColumnName, List<SqlParameter> parameters)
        {
            string baseString = "UPDATE ";
            StringBuilder updateStr = new StringBuilder();
            updateStr.Append(baseString).Append(tableName).Append(" SET ");
            foreach (string item in columns)
            {
                string guid = Guid.NewGuid().ToString().Replace("-", "");

                updateStr.Append(item).Append("=@U").Append(guid).Append(",");
                parameters.Add(new SqlParameter("@U" + guid, dr[item]));
            }
            updateStr.Remove(updateStr.Length - 1, 1);
            updateStr.Append(" WHERE ");
            foreach (string item in keyColumnName)
            {
                string guid = Guid.NewGuid().ToString().Replace("-", "");

                updateStr.Append(item).Append("=@W").Append(guid).Append(" AND ");
                parameters.Add(new SqlParameter("@W" + guid, dr[item]));
            }
            updateStr.Remove(updateStr.Length - 5, 5);
            return updateStr.ToString();
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="sqlString"></param>
        /// <param name="parameters"></param>
        public static void ExecuteSQLForTransaction(SqlCommand cmd, string sqlString, params SqlParameter[] parameters)
        {
            cmd.CommandText = sqlString;
            foreach (var item in parameters)
            {
                cmd.Parameters.Add(item);
            }
            cmd.ExecuteNonQuery();
        }
    }
}
