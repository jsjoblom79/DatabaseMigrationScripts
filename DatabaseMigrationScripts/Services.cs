using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseMigrationScripts
{
    public class Services
    {
        public static string GetPassword()
        {
            var pwd = "";

            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000')
                {
                    pwd += i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }

        public static void CreateDirectoryStructure(string directory)
        {
            if (!Directory.Exists(directory))
            {
                //Create Directory
                Directory.CreateDirectory(directory);
                //Assume that if the parent directory needs to be created then all of them do.
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.DatabaseScripts}");
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.TableScripts}");
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.ViewScripts}");
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.FunctionScripts}");
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.StoredProcedureScripts}");

            }

            if (!Directory.Exists($"{directory}\\{DirectoryFolders.DatabaseScripts}"))
            {
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.DatabaseScripts}");
            }
            if (!Directory.Exists($"{directory}\\{DirectoryFolders.TableScripts}"))
            {
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.TableScripts}");
            }
            if (!Directory.Exists($"{directory}\\{DirectoryFolders.ViewScripts}"))
            {
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.ViewScripts}");
            }
            if (!Directory.Exists($"{directory}\\{DirectoryFolders.FunctionScripts}"))
            {
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.FunctionScripts}");
            }
            if (!Directory.Exists($"{directory}\\{DirectoryFolders.StoredProcedureScripts}"))
            {
                Directory.CreateDirectory($"{directory}\\{DirectoryFolders.StoredProcedureScripts}");
            }
        }
    }

    public enum DirectoryFolders
    {
        DatabaseScripts,
        TableScripts,
        ViewScripts,
        FunctionScripts,
        StoredProcedureScripts
    }
}
