using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Configuration
{
    public static class ConnectionStringFactory
    {
        private static IApplicationSettings _applicationSettings;

        static ConnectionStringFactory()
        {
            _applicationSettings = new WebConfigApplicationSetting();
        }

        /// <summary>
        /// 获得连接字符串
        /// </summary>
        public static string DatabaseAdapterConnectionString
        {
            get { return _applicationSettings.DatabaseAdapterConnectionString; }
        }
    }
}
