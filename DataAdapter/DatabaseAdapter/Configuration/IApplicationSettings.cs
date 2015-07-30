using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Configuration
{
    interface IApplicationSettings
    {
        /// <summary>
        /// 读配置文件，获取连接字符串
        /// </summary>
        string DatabaseAdapterConnectionString { get; }
    }
}
