using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAdapter.Configuration
{
    public class WebConfigApplicationSetting : IApplicationSettings
    {
        public string DatabaseAdapterConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DBConn"].ToString(); }
        }        
    }
}
