using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ConsoleApp1
{
    public class Course
    {
        public string CourseID { get; set; }
        public string Title { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello E21I4DAB class!");
            string nonquery = "USE MySchool";
            string query = "SELECT CourseID, Title FROM  dbo.Course WHERE Course.CourseID = 'C1045'";
            //"Data Source=(localdb)\E21I4DAB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            string connection = @"Data Source = (localdb)\E21I4DAB; Initial Catalog = MySchool; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False"; 
            //CreateCommand(nonquery, connection);
            CreateQueryCommand(query, connection);
            //ReadOrderData(connection);
            //TestConnection.TestDBCon();
        }

        private static void CreateCommand(string queryString,
                                         string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                //connection.Close();

            } //connection.Dispose(); call when leaving scope 
        }

        private static void CreateQueryCommand(string queryString,
                                          string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                //SqlDataReader reader = command.ExecuteReader();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Course data = new Course();
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}",
                            reader[0], reader[1]));
                        data.CourseID = (string)reader["CourseID"];
                        data.Title = (string)reader["Title"];

                        Console.WriteLine(String.Format("{0}, {1}",
                            reader["CourseID"], reader["Year"]));

                        Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}",
                           reader["CourseID"], reader["Year"], reader["Title"],
                           reader["Credits"],reader["DepartmentID"]));
                    }
                }                      
            }
        }

        private static void ReadOrderData(string connectionString)
        {
            string queryString =
                "SELECT CourseID, Title FROM Course;";
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(
                    queryString, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}",
                            reader[0], reader[1]));
                    }
                }
            }
        }
    }




    class TestConnection
    {

        static class SqlHelper
        {
            // Set the connection, command, and then execute the command with non query.  
            public static Int32 ExecuteNonQuery(String connectionString, String commandText,
                CommandType commandType, params SqlParameter[] parameters)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(commandText, conn))
                    {
                        // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect   
                        // type is only for OLE DB.    
                        cmd.CommandType = commandType;
                        cmd.Parameters.AddRange(parameters);

                        conn.Open();
                        return cmd.ExecuteNonQuery();
                    }
                }
            }

            // Set the connection, command, and then execute the command and only return one value.  
            public static Object ExecuteScalar(String connectionString, String commandText,
                CommandType commandType, params SqlParameter[] parameters)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(commandText, conn))
                    {
                        cmd.CommandType = commandType;
                        cmd.Parameters.AddRange(parameters);

                        conn.Open();
                        return cmd.ExecuteScalar();
                    }
                }
            }

            // Set the connection, command, and then execute the command with query and return the reader.  
            public static SqlDataReader ExecuteReader(String connectionString, String commandText,
                CommandType commandType, params SqlParameter[] parameters)
            {
                SqlConnection conn = new SqlConnection(connectionString);

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    // When using CommandBehavior.CloseConnection, the connection will be closed when the   
                    // IDataReader is closed.  
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    return reader;
                }
            }
        }

        public static void TestDBCon()
        {
            //string connectionString = "Data Source=(local);Initial Catalog=MySchool;Integrated Security=True;Asynchronous Processing=true;";
            string connectionString = $"Data Source=localhost;Initial Catalog=MySchool;User ID=sa;Password=F20ITonk!!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            CountCourses(connectionString, 2012);
            Console.WriteLine();

            Console.WriteLine("Following result is the departments that started from 2007:");
            GetDepartments(connectionString, 2007);
            Console.WriteLine();

            Console.WriteLine("Add the credits when the credits of course is lower than 4.");
            AddCredits(connectionString, 4);
            Console.WriteLine();

            Console.WriteLine("Please press any key to exit...");
            Console.ReadKey();
        }

        static void CountCourses(String connectionString, Int32 year)
        {
            String commandText = "Select Count([CourseID]) FROM [MySchool].[dbo].[Course] Where Year=@Year";
            SqlParameter parameterYear = new SqlParameter("@Year", SqlDbType.Int);
            parameterYear.Value = year;

            Object oValue = SqlHelper.ExecuteScalar(connectionString, commandText, CommandType.Text, parameterYear);
            Int32 count;
            if (Int32.TryParse(oValue.ToString(), out count))
                Console.WriteLine("There {0} {1} course{2} in {3}.", count > 1 ? "are" : "is", count, count > 1 ? "s" : null, year);
        }

        // Display the Departments that start from the specified year.  
        static void GetDepartments(String connectionString, Int32 year)
        {
            String commandText = "dbo.GetDepartmentsOfSpecifiedYear";

            // Specify the year of StartDate  
            SqlParameter parameterYear = new SqlParameter("@Year", SqlDbType.Int);
            parameterYear.Value = year;

            // When the direction of parameter is set as Output, you can get the value after   
            // executing the command.  
            SqlParameter parameterBudget = new SqlParameter("@BudgetSum", SqlDbType.Money);
            parameterBudget.Direction = ParameterDirection.Output;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.StoredProcedure, parameterYear, parameterBudget))
            {
                Console.WriteLine("{0,-20}{1,-20}{2,-20}{3,-20}", "Name", "Budget", "StartDate",
                    "Administrator");
                while (reader.Read())
                {
                    Console.WriteLine("{0,-20}{1,-20:C}{2,-20:d}{3,-20}", reader["Name"],
                        reader["Budget"], reader["StartDate"], reader["Administrator"]);
                }
            }
            Console.WriteLine("{0,-20}{1,-20:C}", "Sum:", parameterBudget.Value);
        }

        // If credits of course is lower than the certain value, the method will add the credits.  
        static void AddCredits(String connectionString, Int32 creditsLow)
        {
            String commandText = "Update [MySchool].[dbo].[Course] Set Credits=Credits+1 Where Credits<@Credits";

            SqlParameter parameterCredits = new SqlParameter("@Credits", creditsLow);

            Int32 rows = SqlHelper.ExecuteNonQuery(connectionString, commandText, CommandType.Text, parameterCredits);

            Console.WriteLine("{0} row{1} {2} updated.", rows, rows > 1 ? "s" : null, rows > 1 ? "are" : "is");
        }
    }
}
