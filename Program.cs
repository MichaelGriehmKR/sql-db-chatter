using System;    
using System.Threading;
using System.Data.SqlClient;
using Serilog;

namespace DatabaseChatter
{
    class Program
    {
        static void Main(string[] args)
        {
            const int minutesToRun = 10;
            const int millisecondsBetweenChatter = 300;

            DateTime endTime = DateTime.UtcNow.AddMinutes(minutesToRun);
        
            Console.WriteLine($"Starting chatter with the database for {minutesToRun} minutes at {DateTime.UtcNow} (UTC), the app will attempt to insert a record every {millisecondsBetweenChatter} milliseconds.");
            
            Log.Logger = new LoggerConfiguration().WriteTo.File("database-chatter.log").CreateLogger();

            Log.Information("The Database Chatter application has started and the Logging has been configured.");

            Log.Information("Ensuring databsase table exists.");

            EnsureChatterTableExists();

            Log.Information("Completed ensuring databsase table exists.");
            
            Log.Information("Getting last run id.");

            var lastTestingRun = GetLastTestingRun();

            var thisTestingRun = lastTestingRun++;

            Log.Information($"TestingRun {thisTestingRun} is beginning.");

            var chatterResults = RunChatter(thisTestingRun, endTime, millisecondsBetweenChatter);

            Log.Information($"TestingRun {thisTestingRun} ended at {DateTime.UtcNow} (UTC) after attempting to insert {chatterResults.InsertCount} records with {chatterResults.ErrorCount} errors.");
        }

        private static string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();  
                connBuilder.DataSource = "<azure-sql-server-name>.database.windows.net";  
                connBuilder.UserID = "";  
                connBuilder.Password = "";  
                connBuilder.InitialCatalog = ""; 

                return connBuilder.ConnectionString;
            }
        }

        private static void EnsureChatterTableExists()
        {
            using(SqlConnection connection = new SqlConnection(ConnectionString)) 
            {  
                connection.Open();  
                
                string command = "if not exists (select * from sysobjects where name='Testing' and xtype='U') " +
                                    "create table DatabaseChatter ( " + 
                                        "Id int IDENTITY(1,1) PRIMARY KEY, " + 
                                        "InsertTimestamp DATETIME NOT NULL DEFAULT GETUTCDATE(), " + 
                                        "TestingRun INT NOT NULL, " + 
                                        "InsertAttempt INT NOT NULL " +
                                    ") " + 
                                "go";

                using(SqlCommand sqlCmd = new SqlCommand(command, connection)) 
                {  
                    sqlCmd.ExecuteNonQuery();
                }  
            }
        }

        private static int GetLastTestingRun()
        {
            int? lastTestingRun = null;

            using(SqlConnection connection = new SqlConnection(ConnectionString)) 
            {  
                connection.Open();  
                
                using(SqlCommand sqlCmd = new SqlCommand("SELECT MAX(TestingRun) FROM [dbo].[DatabaseChatter]", connection)) 
                {  
                    lastTestingRun = (int?)sqlCmd.ExecuteScalar();
                }  
            }

            if (!lastTestingRun.HasValue)
            {
                lastTestingRun = 0;
            }

            return lastTestingRun.Value;
        }

        private static ChatterResults RunChatter(int thisTestingRun, DateTime endTime, int millisecondsBetweenChatter)
        {
            int insertCount = 1;
            int errorCount = 0;

            while (DateTime.UtcNow < endTime)
            {
                try 
                {  
                    using(SqlConnection connection = new SqlConnection(ConnectionString)) 
                    {  
                        connection.Open();  
                        
                        using(SqlCommand sqlCmd = new SqlCommand("INSERT INTO [dbo].[DatabaseChatter] (TestingRun, InsertAttempt) VALUES (@TestingRun, @InsertAttempt)", connection)) 
                        {  
                            sqlCmd.Parameters.AddWithValue("@TestingRun", thisTestingRun);
                            sqlCmd.Parameters.AddWithValue("@InsertAttempt", insertCount);

                            sqlCmd.ExecuteNonQuery();
                        }  
                    }

                    Console.WriteLine($"[INFO] InsertAttempt {insertCount} was successful");
                } 
                catch (Exception ex) 
                {  
                    errorCount++;

                    Log.Error($"The following error was encountered during InsertAttempt {insertCount}: {ex.Message}");

                    Console.WriteLine($"[ERROR] InsertAttempt {insertCount} encounter the following error: {ex.Message}");
                }

                insertCount++;

                Thread.Sleep(millisecondsBetweenChatter);
            }

            return new ChatterResults(){ InsertCount = insertCount, ErrorCount = errorCount };
        }
    }

    internal struct ChatterResults
    {
        internal int InsertCount { get; set; }

        internal int ErrorCount { get; set; }
    }
}
