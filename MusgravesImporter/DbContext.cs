using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DateTime = System.DateTime;

namespace MusgravesImporter
{
    public class DbContext
    {
        private SqlConnectionStringBuilder GetSqlBuilder()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "intdbmi01.127bf3941546.database.windows.net,1433";
            builder.UserID = "interlinc.reporting";
            builder.Password = "Thusshare12";
            builder.InitialCatalog = "REPORTING";
            //builder.ConnectTimeout = 120;

            //builder.DataSource = "interlincmiddlewaredbserver.database.windows.net";
            //builder.UserID = "ob";
            //builder.Password = "password@1";
            //builder.InitialCatalog = "Musgrave";

            return builder;
        }


        public void InsertIntoMusgrave(List<ReportingModel> model)
        {
            int row = 0;
            var builder = GetSqlBuilder();
            try
            {
                int count = 1;
               
               


           

                var weekGroup = model.GroupBy(a => a.Week);
                foreach (var week in weekGroup)
                {
                    DataTable tbl = new DataTable();
                    tbl.Columns.Add(new DataColumn("Week", typeof(string)));
                    tbl.Columns.Add(new DataColumn("Category", typeof(string)));
                    tbl.Columns.Add(new DataColumn("Location", typeof(string)));
                    tbl.Columns.Add(new DataColumn("Item", typeof(string)));
                    tbl.Columns.Add(new DataColumn("Quantity", typeof(Int32)));
                    tbl.Columns.Add(new DataColumn("DateCreated", typeof(DateTime)));
                    foreach (var item in week)
                    {
                     
                        DataRow dr = tbl.NewRow();
                        dr["Week"] = item.Week;
                        dr["Category"] = item.ProductType;
                        dr["Location"] = item.Location.SplitByApostrophe();
                        dr["Item"] = item.Product.SplitByApostrophe();
                        dr["Quantity"] = int.Parse(item.Sales);
                        dr["DateCreated"] = DateTime.Now;
                        tbl.Rows.Add(dr);
                        Console.WriteLine(count);

                        count++;
                    }
                    using SqlConnection connection = new SqlConnection(builder.ConnectionString);
                    using SqlBulkCopy ojBulkCopy = new SqlBulkCopy(connection);
                   ojBulkCopy.BulkCopyTimeout = 0;
                    ojBulkCopy.DestinationTableName = "Radius.Sales";
                    

                    ojBulkCopy.ColumnMappings.Add("Week", "Week");
                    ojBulkCopy.ColumnMappings.Add("Category", "Category");
                    ojBulkCopy.ColumnMappings.Add("Location", "Location");
                    ojBulkCopy.ColumnMappings.Add("Item", "Item");
                    ojBulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                    ojBulkCopy.ColumnMappings.Add("DateCreated", "DateCreated");

                    connection.Open();

                    ojBulkCopy.WriteToServer(tbl);
                    MoveFile(week?.FirstOrDefault()?.Week);
                    connection.Close();
                }
              


            }
            catch (Exception e)
            {
                
                LogFile.Write(e.Message);

            }

        }

        private void MoveFile(string week)
        {
            var location = Settings.GetFileLocation() + "OriginalProcessFiles//";

            string[] files = System.IO.Directory.GetFiles(Settings.GetFileLocation(), "*.csv");
            foreach (var file in files)
            {
                var w = week.Split(" ");

                var newWeek = $"{w[0]} {w[1]}";
                if (file.Contains(newWeek))
                {
                    var filename = file.Split("//")[3];
                    if (!Directory.Exists(location))
                    {
                        Directory.CreateDirectory(location);
                    }

                    File.Move(file, location + filename);
                }
              
            }
        }



    }
}
