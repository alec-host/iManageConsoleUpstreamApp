using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using System.Data;

namespace iManageConsoleUpstreamApp.Db
{
    public class DataRepository
    {
        public static void CreateFolderDocumentSchema() 
        {
            using (MySqlConnection conn = new Db().MySqlDbConnectService())
            {
                if (conn.State == ConnectionState.Closed)

                    conn.Open();

                    string sql =
                        "CREATE TABLE IF NOT EXISTS `tbl_file_info` ( " +
                            "`_id` INT(10) NOT NULL AUTO_INCREMENT, " +
                            "`file` VARCHAR(100) NOT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`folder_path` VARCHAR(250) NOT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`base_path` VARCHAR(150) NOT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`folders` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`parent_folder_id` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`custom1` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`custom2` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`custom29` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`class` VARCHAR(50) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`operator` VARCHAR(100) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci', " +
                            "`create_date` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, " +
                            "`edit_date` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP, " +
                            "`is_uploaded` INT(10) NULL DEFAULT '0', " +
                            "PRIMARY KEY (`_id`) USING BTREE, " +
                            "INDEX `parent_folder_id` (`parent_folder_id`) USING BTREE " +
                        ")" +
                        "COLLATE='utf8mb4_general_ci' " +
                        "ENGINE=InnoDB";

                    MySqlCommand cmd = new MySqlCommand(sql,conn);

                    cmd.ExecuteNonQuery();

                    conn.Close();
            }
        }
        public static void CreateSubFolderTrackerSchema()
        {
            using (MySqlConnection conn = new Db().MySqlDbConnectService())
            {
                if (conn.State == ConnectionState.Closed)

                    conn.Open();

                string sql =
                    "CREATE TABLE IF NOT EXISTS `tbl_sub_folder` ( " +
                        "`_id` INT(10) NOT NULL AUTO_INCREMENT, " +
                        "`folder_name` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_general_ci', " +
                        "`parent_folder_id` VARCHAR(50) NOT NULL COLLATE 'utf8mb4_general_ci', " +
                        "PRIMARY KEY (`_id`) USING BTREE, " +
                        "INDEX `parent_folder_id` (`parent_folder_id`) USING BTREE " +
                    ")" +
                    "COLLATE='utf8mb4_general_ci' " +
                    "ENGINE=InnoDB";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.ExecuteNonQuery();

                conn.Close();
            }
        }
        public static MySqlDataReader GetFolderDocumentRecords()
        {
            MySqlConnection conn = new Db().MySqlDbConnectService();
            
            if (conn.State == ConnectionState.Closed)
                conn.Open();
                string sql = "SELECT " +
                             "`_id`," +
                             "`file`," +
                             "`folder_path`," +
                             "`parent_folder_id`," +
                             "`custom1`," +
                             "`custom2`," +
                             "`custom29`," +
                             "`class`," +
                             "`operator`," +
                             "`create_date`," +
                             "`edit_date` " +
                             "FROM " +
                             "`tbl_file_info` " +
                             "WHERE " +
                             "`is_processed` = 0 " +
                             "LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(sql,conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                
                return reader;
        }
        public static void FlagRecordAsProcessed(int recordID) { 
            MySqlConnection conn = new Db().MySqlDbConnectService();
            if(conn.State == ConnectionState.Closed)
                conn.Open();
                string sql = "UPDATE `tbl_file_info` SET `is_processed` = 1 WHERE `is_processed` = 0 AND `_id` = @recordID";
                MySqlCommand cmd = new MySqlCommand(sql,conn);
                cmd.Parameters.AddWithValue("@recordID",recordID);
                cmd.ExecuteNonQuery();
                conn.Close();
        }
        public static void TrackSubFolderId(string createdFolderName,string parentFolderId) 
        {
            MySqlConnection conn = new Db().MySqlDbConnectService();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string sql = "INSERT " +
                         "INTO " +
                         "`tbl_sub_folder` " +
                         "(`folder_name`,`parent_folder_id`) " +
                         "VALUES  " +
                         "(@folderName,@folderId) " +
                         "WHERE " +
                         "`parent_folder_id` != @folderId";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@folderName", createdFolderName);
            cmd.Parameters.AddWithValue ("@folderId", parentFolderId);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
