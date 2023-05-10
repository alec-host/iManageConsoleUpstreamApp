using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iManageConsoleUpstreamApp.Db
{
    public class Utiliy
    {
        public static dynamic SqlDataToJson(MySqlDataReader MySqlDataReader) 
        {
            var dataTable = new DataTable();

            dataTable.Load(MySqlDataReader);

            string jsonString = string.Empty;

            jsonString = JsonConvert.SerializeObject(dataTable);
        
            return jsonString;
        }
    }
}
