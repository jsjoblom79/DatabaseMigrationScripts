using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace DatabaseMigrationScripts
{
    public class ServerFunctions
    {
        public static DataSet QueryServer(string host, string userId, string pass,string dbName)
        {
            DataSet result = new DataSet();
            var connectionString = $"Server={host};Database={dbName};User ID={userId};Password={pass};Trust Server Certificate=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var serverConnection = new ServerConnection(connection);
                Server server = new Server(serverConnection);
                 result = server.ConnectionContext.ExecuteWithResults($"SELECT name FROM sys.procedures WHERE name not LIKE 'sp_%'");
            }
            return result;

        }
        internal static void GetFunctionScripts(Database db, UserDefinedFunctionCollection functions, Scripter scripter, string outputDirectory)
        {
            string funcInfo = "";
            foreach (UserDefinedFunction func in functions)
            {
                if (!func.IsSystemObject)
                {
                    var cnt = 0;
                    var procScript = scripter.Script(new Urn[] { func.Urn });

                    foreach (var line in procScript)
                    {
                        if(cnt == procScript.Count - 1)
                        {
                            funcInfo += $"{line}' EXEC (@var_{func.Name}) END\n";
                        }
                        else
                        {
                            funcInfo += $"{line}\n";
                        }
                        
                        if (cnt == 1)
                        {
                            funcInfo += $"IF NOT EXISTS (SELECT name FROM sys.objects WHERE name='{func.Name}')\nBEGIN\n" +
                                $"DECLARE @var_{func.Name} VARCHAR(MAX) = \n'";
                        }
                        cnt++;
                    }
                }
            }
            File.WriteAllText($"{outputDirectory}\\{DirectoryFolders.FunctionScripts}\\{db.Name}_CreateFunctionScripts.sql", funcInfo);
        }

        internal static void GetProcedureScripts(Database db, StoredProcedureCollection storedProcedures, Scripter scripter, string outputDirectory)
        {
            string procInfo = "";


            foreach (StoredProcedure proc in storedProcedures)
            {
                if (!proc.IsSystemObject)
                {
                    var cnt = 0;
                    var procScript = scripter.Script(new Urn[] { proc.Urn });

                    foreach (var line in procScript)
                    {
                        if(cnt == procScript.Count - 1)
                        {
                            procInfo += $"{line}' EXEC(@var_{proc.Name}) END\n";
                        }
                        else
                        {
                            procInfo += $"{line}\n";
                        }
                        
                        if (cnt == 1)
                        {
                            procInfo += $"IF NOT EXISTS (SELECT name FROM sys.procedures WHERE name='{proc.Name}')\n" +
                                $"BEGIN\nDECLARE @var_{proc.Name} VARCHAR(MAX) =\n'";
                        }
                        cnt++;
                    }
                }
            }
            File.WriteAllText($"{outputDirectory}\\{DirectoryFolders.StoredProcedureScripts}\\{db.Name}_CreateProcedureScripts.sql", procInfo);
        }

        internal static void GetTableScripts(Database db, TableCollection tables, Scripter scripter, string outputDirectory)
        {
            string tableinfo = "";
            foreach (Table table in tables)
            {
                if (!table.IsSystemObject)
                {
                    var cnt = 0;
                    var tableScript = scripter.Script(new Urn[] { table.Urn });

                    foreach (var line in tableScript)
                    {
                        tableinfo += $"{line}\n";
                        if (cnt == 1)
                        {
                            tableinfo += $"IF NOT EXISTS (SELECT name FROM sys.tables WHERE name='{table.Name}')\n";
                        }
                        cnt++;
                    }
                }
            }
            File.WriteAllText($"{outputDirectory}\\{DirectoryFolders.TableScripts}\\{db.Name}_CreateTableScripts.sql",tableinfo);
        }

        internal static void GetViewScripts(Database db, ViewCollection views, Scripter scripter, string outputDirectory)
        {
            string viewInfo = "";
            foreach (View view in views)
            {
                if (!view.IsSystemObject)
                {
                    var cnt = 0;
                    var procScript = scripter.Script(new Urn[] { view.Urn });

                    foreach (var line in procScript)
                    {
                        if (cnt == procScript.Count-1)
                        {
                            viewInfo += $"{line}'\nEXEC (@var_{view.Name.Replace(" ","_")}) END\n";
                        }
                        else
                        {
                            viewInfo += $"{line}\n";
                        }
  
                        if (cnt == 1)
                        {
                            viewInfo += $"IF NOT EXISTS (SELECT name FROM sys.views WHERE name='{view.Name}')\n" +
                                $"BEGIN\nDECLARE @var_{view.Name.Replace(" ", "_")} VARCHAR(MAX) = '\n";
                        }
                        cnt++;
                    }
                }
            }
            File.WriteAllText($"{outputDirectory}\\{DirectoryFolders.ViewScripts}\\{db.Name}_CreateViewScripts.sql", viewInfo);
        }

        public static void ProcessServer(string connectionString, string outputDirectory)
        {
            //Create a sqlConnection object with the connection string.
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                //create a server connection with the sqlconnection
                ServerConnection serverConnection = new ServerConnection(sqlConnection);
                //Create a server object with the server connection.
                Server server = new Server(serverConnection);
               
                //Read the databases from the server.database list
                foreach (Database db in server.Databases)
                {
                    if (!db.IsSystemObject && db.Status != DatabaseStatus.Offline)
                    {

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Currently working on Database - {db.Name}. {DateTime.Now:MM/dd/yyyy hh:mm:ss}");
                        
                        //Create the scripter object and set the options
                        Scripter scripter = new Scripter(server);
                        scripter.Options.ScriptDrops = false;
                        scripter.Options.WithDependencies = false;
                        scripter.Options.ScriptSchema = true;
                        scripter.Options.ScriptData = false;
                        scripter.Options.IncludeHeaders = true;
                        scripter.Options.SchemaQualify = true;
                        scripter.Options.ScriptBatchTerminator = true;

                        //Gather the collections to for creating scripts
                        TableCollection tables = db.Tables;
                        StoredProcedureCollection storedProcedures = db.StoredProcedures;
                        UserDefinedFunctionCollection functions = db.UserDefinedFunctions;
                        ViewCollection views = db.Views;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        GetDatabaseScripts(db, scripter, outputDirectory);
                        Console.WriteLine("Database Script Complete.");
                        
                        GetTableScripts(db, tables, scripter, outputDirectory);
                        Console.WriteLine("Table Scripts Complete.");
                        
                        GetProcedureScripts(db,storedProcedures, scripter, outputDirectory);
                        Console.WriteLine("Procedure Scripts Complete.");
                        
                        GetFunctionScripts(db, functions, scripter, outputDirectory);
                        Console.WriteLine("Function Scripts Complete.");
                        
                        GetViewScripts(db, views, scripter, outputDirectory);
                        Console.WriteLine("View Scripts Complete.");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Database {db.Name} has been completed. {DateTime.Now:MM/dd/yyyy hh:mm:ss}");

                    }
                }
            }
        }
        public static void GetDatabaseScripts(Database database, Scripter scripter, string outputDirectory)
        {
            var dbInfo = "";
            var dbScript = scripter.Script(new Urn[] {database.Urn});
            foreach (var line in dbScript)
            {
                dbInfo += $"{line}\n";
            }

            File.WriteAllText($"{outputDirectory}\\{DirectoryFolders.DatabaseScripts}\\{database.Name}_CreateScript.sql",dbInfo);
        }
    }
}
