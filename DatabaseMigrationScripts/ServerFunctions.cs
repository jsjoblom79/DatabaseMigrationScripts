using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.SqlServer.Management.Dmf.ExpressionNodeFunction;

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
        internal static string? GetFunctionScripts(UserDefinedFunctionCollection functions, Scripter scripter)
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
            return funcInfo;
        }

        internal static string? GetProcedureScripts(StoredProcedureCollection storedProcedures, Scripter scripter)
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
            return procInfo;
        }

        internal static string? GetTableScripts(TableCollection tables, Scripter scripter)
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
            return tableinfo;
        }

        internal static string? GetViewScripts(ViewCollection views, Scripter scripter)
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
            return viewInfo;
        }

        public static string? GetDatabaseScripts(DatabaseCollection databases, Scripter scripter)
        {
            var dbInfo = "";
            foreach (Database database in databases)
            {
                if (!database.IsSystemObject && database.IsAccessible)
                {
                    var dbScript = scripter.Script(new Urn[] {database.Urn});
                    foreach (var line in dbScript)
                    {
                        dbInfo += line;
                    }
                   // File.WriteAllText(@$"C:\DatabaseScripts\{database.Name}_DBSchema.sql", dbInfo);
                }
            }
            return dbInfo;
        }
    }
}
