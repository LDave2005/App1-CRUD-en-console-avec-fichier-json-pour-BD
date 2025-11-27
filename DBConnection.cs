using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class DBConnection
    {
        private static string connectionString = "Server=localhost;Database=GestionUtilisateurs;User Id=root;Password=;";
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
