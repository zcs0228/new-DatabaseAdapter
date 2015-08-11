using DatabaseAdapter.Configuration;
using DatabaseAdapter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter
{
    public class DatabaseFactory : IDatabaseFactory
    {
        private string _connectionString;

        public DatabaseFactory()
        {
            _connectionString = ConnectionStringFactory.DatabaseAdapterConnectionString;
        }

        public DatabaseFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        #region  ExecuteSQL
        public int ExecuteSQL(string sqlString)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;

                try
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }

        public int ExecuteSQL(string sqlString, params SqlParameter[] parameters)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlString;
                cmd.CommandType = CommandType.Text;
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }

                try
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }


        public int ExecuteSQL(string sqlString, bool isStoredProcedure, params SqlParameter[] parameters)
        {
            int result = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlString;

                if (isStoredProcedure == true)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                }

                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }

                try
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }
        #endregion

        #region Query
        public DataTable Query(string queryString, params SqlParameter[] parameters)
        {
            DataTable result = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = queryString;
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }

                try
                {
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(result);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }
        #endregion

        #region Save 批量存储数据
        public int Save(string tableName, DataTable sourceTable)
        {
            int affected = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.CheckConstraints, transaction))
                    {
                        bulkCopy.BatchSize = 10;
                        bulkCopy.BulkCopyTimeout = 60;
                        bulkCopy.DestinationTableName = tableName;
                        try
                        {
                            bulkCopy.WriteToServer(sourceTable);
                            transaction.Commit();
                            affected = sourceTable.Rows.Count;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            affected = -1;
                            throw new Exception(ex.Source + ":" + ex.Message);
                        }
                    }
                }
            }

            return affected;
        }

        public int Save(string tableName, DataTable sourceTable, params string[] excludeColumnName)
        {
            int result;
            string[] columns = MSSQLHelper.GetDataTableColumnName(sourceTable, excludeColumnName);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = transaction;

                try
                {
                    foreach (DataRow dr in sourceTable.Rows)
                    {
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        string insertStr = MSSQLHelper.DataRowToInsert(tableName, dr, columns, parameters);
                        MSSQLHelper.ExecuteSQLForTransaction(cmd, insertStr, parameters.ToArray());
                    }
                    transaction.Commit();
                    result = sourceTable.Rows.Count;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }
            return result;
        }
        #endregion

        #region Update 批量更新
        public int Update(string tableName, DataTable sourceTable, string[] keyColumnName)
        {
            int result;
            string[] columns = MSSQLHelper.GetDataTableColumnName(sourceTable, keyColumnName);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = transaction;

                try
                {
                    foreach (DataRow dr in sourceTable.Rows)
                    {
                        List<SqlParameter> parameters = new List<SqlParameter>();
                        string updateStr = MSSQLHelper.DataRowToUpdate(tableName, dr, columns, keyColumnName, parameters);
                        MSSQLHelper.ExecuteSQLForTransaction(cmd, updateStr, parameters.ToArray());
                    }
                    transaction.Commit();
                    result = sourceTable.Rows.Count;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }

            }
            return result;
        }
        #endregion

        #region OracleExecuteSQL
        public int OracleExecuteSQL(string sqlString)
        {
            int result = 0;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlString;

                try
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }


        public int OracleExecuteSQL(string sqlString, params OracleParameter[] parameters)
        {
            int result = 0;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sqlString;

                foreach (OracleParameter item in parameters)
                {
                    cmd.Parameters.Add(item);
                }

                try
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }

        public int OracleExecuteSQL(string sqlString, bool isStoredProcedure, params OracleParameter[] parameters)
        {
            int result = 0;

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = conn.CreateCommand();

                if (isStoredProcedure == true)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                }
                cmd.CommandText = sqlString;

                try
                {
                    conn.Open();
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    result = -1;
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }
        #endregion

        #region OracleQuery
        public DataTable OracleQuery(string queryString, params OracleParameter[] parameters)
        {
            DataTable result = new DataTable();

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = queryString;

                foreach (OracleParameter item in parameters)
                {
                    cmd.Parameters.Add(item);
                }

                try
                {
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(result);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Source + ":" + ex.Message);
                }
            }

            return result;
        }
        #endregion

        public int OracleSave(string tableName, DataTable sourceTable, params string[] excludeColumnName)
        {
            throw new NotImplementedException();
        }

        public int OracleUpdate(string tableName, DataTable sourceTable, string[] keyColumnName)
        {
            throw new NotImplementedException();
        }
    }
}