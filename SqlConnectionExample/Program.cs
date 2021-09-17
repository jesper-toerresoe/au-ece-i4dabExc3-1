using System;
using Microsoft.Data.SqlClient;

namespace SqlConnectionExample
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");

      RunQuery("CREATE TABLE example(id INT, name VARCHAR(20))");
      RunQuery("INSERT INTO example VALUES (1, 'adsf'), (2, 'dsapepe')");
    }

    static void RunQuery(string query)
    {
      using (var connection = ConnectToDatabase())
      {
        var command = new SqlCommand(query, connection);
        command.ExecuteNonQuery();
      }
    }

    // Note: Important to use this in a using clause
    static SqlConnection ConnectToDatabase()
    {
      var connection = new SqlConnection("Data Source=127.0.0.1,1433;Database=SqlConnection;User ID=SA;Password=12345678Aa#;");
      connection.Open();
      return connection;
    }
  }
}
