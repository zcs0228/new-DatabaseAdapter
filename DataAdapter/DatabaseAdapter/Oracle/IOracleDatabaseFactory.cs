using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Oracle
{
    public interface IOracleDatabaseFactory
    {
        /// <summary>
        /// 执行SQL语句，返回受影响行数
        /// </summary>
        /// <param name="sqlString">需要执行的SQL语句</param>
        /// <returns>返回受影响行数</returns>
        int OracleExecuteSQL(string sqlString);

        /// <summary>
        /// 执行带参数的SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="sqlString">需要执行的SQL语句</param>
        /// <param name="parameters">参数对象数组</param>
        /// <returns>返回受影响行数</returns>
        int OracleExecuteSQL(string sqlString, params OracleParameter[] parameters);

        /// <summary>
        /// 执行带参数的SQL语句或者存储过程，返回受影响的行数
        /// </summary>
        /// <param name="sqlString">需要执行的SQL语句或者存储过程名称</param>
        /// <param name="isStoredProcedure">是否执行存储过程，true为执行存储过程</param>
        /// <param name="parameters">参数对象数组</param>
        /// <returns>返回受影响行数</returns>
        int OracleExecuteSQL(string sqlString, bool isStoredProcedure, params OracleParameter[] parameters);

        /// <summary>
        /// 执行查询SQL语句，返回DataTable结果集
        /// </summary>
        /// <param name="queryString">查询SQL语句</param>
        /// <param name="parameters">参数对象数组</param>
        /// <returns>返回DataTable结果集</returns>
        DataTable OracleQuery(string queryString, params OracleParameter[] parameters);

        /// <summary>
        /// 批量存储数据，返回受影响的行数
        /// </summary>
        /// <param name="tableName">存储数据的表名</param>
        /// <param name="sourceTable">需要存储的数据集合</param>
        /// <param name="excludeColumnName">需要排除的字段名称</param>
        /// <returns>返回受影响行数</returns>
        int OracleSave(string tableName, DataTable sourceTable, params string[] excludeColumnName);

        /// <summary>
        /// 批量更新数据，返回受影响行数
        /// </summary>
        /// <param name="tableName">需要更新的数据库名称</param>
        /// <param name="sourceTable">需要更新的数据集合</param>
        /// <param name="keyColumnName">筛选条件参考字段</param>
        /// <returns>返回受影响的行数</returns>
        int OracleUpdate(string tableName, DataTable sourceTable, string[] keyColumnName);
    }
}
