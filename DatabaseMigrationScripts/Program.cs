// See https://aka.ms/new-console-template for more information

using DatabaseMigrationScripts;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Specialized;
using System.Text;

Console.Write("Please Enter the server you wish to connect to: ");
var myserver = Console.ReadLine();
Console.Write("Do you need to provide SQL Credentials? [Y] or [N] ");
var creds = Console.ReadLine();
var userId = "";
var pass = "";
var connectionString = "";
if (creds.ToLower().Equals("y"))
{
    Console.Write("Please Enter the user ID: ");
    userId = Console.ReadLine();
    Console.Write("Please Enter the password: ");
    pass = Services.GetPassword();
    connectionString = $"Server={myserver};Database=master; User ID={userId};Password={pass};Trust Server Certificate=True";
}
else
{
    connectionString = $"Server={myserver};Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
}

Console.Write("Enter the directory to store the Scripts: ");
var outputDirectory = Console.ReadLine();


using (SqlConnection sqlConnection = new SqlConnection(connectionString))
{
    ServerConnection serverConnection = new ServerConnection(sqlConnection);
    Server server = new Server(serverConnection);
    //var dbResults = ServerFunctions.GetDatabaseScripts(server.Databases, new Scripter(server));
    foreach (Database db in server.Databases)
    {
        if (!db.IsSystemObject)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Database {db.Name} Started. {DateTime.Now:MM/dd/yyyy hh:mm:ss}");
            Scripter scripter = new Scripter(server);
         
            scripter.Options.ScriptDrops = false;
            scripter.Options.WithDependencies = false;
            scripter.Options.ScriptSchema = true;
            scripter.Options.ScriptData = false;
            scripter.Options.IncludeHeaders = true;
            scripter.Options.SchemaQualify = true;
            scripter.Options.ScriptBatchTerminator = true;
            
            

            string outputFile = Path.Combine(outputDirectory, $"{db.Name}_SchemaObjects.sql");

            TableCollection tables = db.Tables;
            StoredProcedureCollection storedProcedures = db.StoredProcedures;
            UserDefinedFunctionCollection functions = db.UserDefinedFunctions;
            ViewCollection views = db.Views;


            StringBuilder scriptLines = new();
            scriptLines.AppendLine($"IF NOT EXISTS (SELECT name FROM master.sys.databases WHERE name = '{db.Name}')\nCREATE DATABASE {db.Name}");
            scriptLines.AppendLine($"USE {db.Name}\nGO");
            //StringCollection? tableScript;// = new StringCollection();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Getting Table Scripts.");
            scriptLines.AppendLine(ServerFunctions.GetTableScripts(tables,scripter));
            Console.WriteLine("Getting Procedure Scripts.");
            
            scriptLines.AppendLine(ServerFunctions.GetProcedureScripts(storedProcedures,scripter));
            Console.WriteLine("Getting Function Scripts.");
            scriptLines.AppendLine(ServerFunctions.GetFunctionScripts(functions, scripter));
            Console.WriteLine("Getting View Scripts.");
            scriptLines.AppendLine(ServerFunctions.GetViewScripts(views, scripter));



            File.WriteAllText(outputFile, scriptLines.ToString());
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Database {db.Name} has been completed. {DateTime.Now:MM/dd/yyyy hh:mm:ss}");

        }
    }
}
Console.ForegroundColor = ConsoleColor.White;



