using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MusgravesImporter
{
    public class DbContext
    {

        public SqlConnection GetConnection()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "intdbmi01.public.127bf3941546.database.windows.net,3342";
                builder.UserID = "interlinc.reporting";
                builder.Password = "Thusshare12";
                builder.InitialCatalog = "MUSGRAVES";
                SqlConnection connection = new SqlConnection(builder.ConnectionString);
                return connection;
            }

            catch (SqlException e)
            {
                return null;
            }
        }



    }
}
