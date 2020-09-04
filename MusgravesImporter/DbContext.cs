using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace MusgravesImporter
{
    public class DbContext
    {
        private SqlConnectionStringBuilder GetSqlBuilder()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //builder.DataSource = "intdbmi01.public.127bf3941546.database.windows.net,3342";
            //builder.UserID = "interlinc.reporting";
            //builder.Password = "Thusshare12";
            //builder.InitialCatalog = "MUSGRAVES";

            builder.DataSource = "interlincmiddlewaredbserver.database.windows.net";
            builder.UserID = "ob";
            builder.Password = "password@1";
            builder.InitialCatalog = "Musgrave";

            return builder;
        }


        public void InsertIntoMusgrave(List<ReportingModel> model)
        {
            int row = 0;
            var builder = GetSqlBuilder();
            try
            {
                int count = 1;
               
                using SqlConnection connection = new SqlConnection(builder.ConnectionString);


                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("Week", typeof(string)));
                tbl.Columns.Add(new DataColumn("Category", typeof(string)));
                tbl.Columns.Add(new DataColumn("Location", typeof(string)));
                tbl.Columns.Add(new DataColumn("Item", typeof(string)));
                tbl.Columns.Add(new DataColumn("Quantity", typeof(Int32)));


                foreach (var item in model)
                {
                    DataRow dr = tbl.NewRow();
                    dr["Week"] = item.Week;
                    dr["Category"] = item.ProductType;
                    dr["Location"] = item.Location.SplitByApostrophe();
                    dr["Item"] = item.Product.SplitByApostrophe();
                    dr["Quantity"] = int.Parse(item.Sales);
                    tbl.Rows.Add(dr);
                    Console.WriteLine(count);

                    count++;
                }
                using SqlBulkCopy ojBulkCopy = new SqlBulkCopy(connection);
                ojBulkCopy.DestinationTableName = "Test";

                ojBulkCopy.ColumnMappings.Add("Week", "Week");
                ojBulkCopy.ColumnMappings.Add("Category", "Category");
                ojBulkCopy.ColumnMappings.Add("Location", "Location");
                ojBulkCopy.ColumnMappings.Add("Item", "Item");
                ojBulkCopy.ColumnMappings.Add("Quantity", "Quantity");

                connection.Open();

              ojBulkCopy.WriteToServer(tbl);
               
         
            }
            catch (Exception e)
            {
                LogFile.Write(e.Message);

            }

        }




    }
}
