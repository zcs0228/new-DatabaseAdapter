using DatabaseAdapter.MSSQL;
using DatabaseAdapter.MySQL;
using DatabaseAdapter.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter
{
    public interface IDatabaseFactory : IMSSQLDatabaseFactory, IOracleDatabaseFactory, IMySQLDatabaseFactory
    {
    }
}
