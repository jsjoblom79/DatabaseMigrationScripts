// See https://aka.ms/new-console-template for more information

using DatabaseMigrationScripts;
Console.ForegroundColor = ConsoleColor.Green;
Console.Title = "Database Migration Scripts";
Console.Write("Please Enter the server you wish to connect to: ");
string myserver = Console.ReadLine();
Console.Write("Do you need to provide SQL Credentials? [Y] or [N] ");
string creds = Console.ReadLine();
string? userId = string.Empty;
string? pass = string.Empty;
string connectionString = string.Empty;

if (string.IsNullOrEmpty(creds) || creds.ToLower().Equals("y"))
{
    Console.Write("Please Enter the user ID: ");
    userId = Console.ReadLine();
    Console.Write("Please Enter the password: ");
    pass = Services.GetPassword();
    Console.WriteLine();
    connectionString = $"Server={myserver};Database=master; User ID={userId};Password={pass};Trust Server Certificate=True";
}
else
{
    connectionString = $"Server={myserver};Integrated Security=True;Trust Server Certificate=True";
}

Console.Write("Enter the directory to store the Scripts: ");
string outputDirectory = Console.ReadLine();

if (!string.IsNullOrEmpty(outputDirectory))
{
    Services.CreateDirectoryStructure(outputDirectory);

    ServerFunctions.ProcessServer(connectionString, outputDirectory);
}


Console.ForegroundColor = ConsoleColor.White;



