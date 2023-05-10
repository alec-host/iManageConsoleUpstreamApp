using IronXL;
using System.Data;
using System.Globalization;

namespace iManageConsoleUpstreamApp
{
    public class ProgramUtility
    {
        public static string FormatDateTime(string datetime)
        {
            var formattedDate = Convert.ToDateTime(datetime, new DateTimeFormatInfo { FullDateTimePattern = "yyyy-MM-dd HH:mm:ss" });
            return formattedDate.ToString("yyyy-MM-dd");
        }
        public static string GetFileExtension(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath).Trim();
            return fileExtension;
        }
        private static void ReadExcel()
        {
            WorkBook book = WorkBook.Load("C:\\Users\\admin\\Downloads\\Sample Unstructured Upstream.xlsx");
            WorkSheet sheet = book.DefaultWorkSheet;
            DataSet dataSet = book.ToDataSet(true);
            book.SaveAsJson("C:\\Users\\admin\\Downloads\\Sample.json");

            foreach (DataTable table in dataSet.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        Console.Write(row[i]);
                    }
                }
            }
        }
    }
}
