


namespace iManageConsoleUpstreamApp.Db
{
    public class Db : IDatabaseInterface
    {
        public MySql.Data.MySqlClient.MySqlConnection MySqlDbConnectService()
        {
            string connectionString = "server=localhost;user=root;database=db_imagic;ConvertZeroDateTime=True;password=;";
            MySql.Data.MySqlClient.MySqlConnection myConn = new MySql.Data.MySqlClient.MySqlConnection
            {
                ConnectionString = connectionString
            };
            
            
            return myConn;
        }
    }
}
