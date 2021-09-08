using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    static class Baza
    {
        static SqlConnection connection;
        static SqlCommand command;
        static SqlDataReader reader;

        public static void OtvaranjeKonekcijeSBazom()
        {
            connection = new SqlConnection("Server = tcp:crypto.database.windows.net,1433; Data Source = crypto.database.windows.net; Initial Catalog = CryptoBaza; Persist Security Info = False; User ID = ivauzarev; Password =crypto2101!; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;");
            connection.Open();
        }

        public static SqlDataReader IzvrsavanjeUpita(string upit)
        {
            command = new SqlCommand(upit, connection);
            reader = command.ExecuteReader();
            return reader;
        }

        public static void ZatvaranjeKonekcijeSBazom()
        {
            connection.Close();
            if(reader != null) reader.Close();
        }
    }
}
