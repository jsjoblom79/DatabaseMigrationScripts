# Database Migration Scripts

A .NET 6.0 console application that generates SQL migration scripts from SQL Server databases. This tool connects to a SQL Server instance and exports database objects (tables, stored procedures, functions, and views) as idempotent SQL scripts.

## Features

- Exports complete database schemas including:
  - Database creation scripts
  - Table structures
  - Stored procedures
  - User-defined functions
  - Views
- Generates idempotent scripts with existence checks (IF NOT EXISTS)
- Supports both Windows Authentication and SQL Server Authentication
- Organizes output into categorized folders
- Processes all non-system databases on a server
- Skips offline databases automatically

## Prerequisites

- .NET 6.0 SDK or later
- Access to a SQL Server instance
- Appropriate permissions to read database metadata

## Dependencies

- Microsoft.Data.SqlClient (5.2.2)
- Microsoft.SqlServer.SqlManagementObjects (171.30.0)

## Installation

1. Clone the repository
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build the solution:
   ```bash
   dotnet build
   ```

## Usage

Run the application and follow the interactive prompts:

```bash
dotnet run
```

You'll be prompted for:
1. **Server name**: The SQL Server instance to connect to (e.g., `localhost`, `server\instance`)
2. **Credentials**: Choose between SQL Server Authentication (Y) or Windows Authentication (N)
   - If using SQL Authentication, provide username and password
3. **Output directory**: The path where scripts will be saved

## Output Structure

The tool creates the following directory structure:

```
[Output Directory]
├── DatabaseScripts/
│   └── [DatabaseName]~CreateScript.sql
├── TableScripts/
│   └── [DatabaseName]~CreateTableScripts.sql
├── StoredProcedureScripts/
│   └── [DatabaseName]~CreateProcedureScripts.sql
├── FunctionScripts/
│   └── [DatabaseName]~CreateFunctionScripts.sql
└── ViewScripts/
    └── [DatabaseName]~CreateViewScripts.sql
```

Each script file contains all objects of that type for a specific database, with proper `USE [DatabaseName]` statements and existence checks.

## Script Features

All generated scripts include:
- `USE [DatabaseName]` context switching
- `GO` batch separators
- Existence checks to prevent errors on re-execution
- Schema qualification
- Proper headers and metadata

## Security Notes

- Passwords are masked during input (displayed as asterisks)
- Connection strings use `Trust Server Certificate=True` for development environments
- Consider reviewing security settings for production use

## Limitations

- Only processes non-system databases
- Skips system objects (those starting with `sp_` or marked as system objects)
- Does not export data, only schema
- Offline databases are skipped

## Contributing

Feel free to submit issues or pull requests for improvements.

## License

This project is licensed under the MIT License - see below for details.

```
MIT License

Copyright (c) 2025 jsjoblom79

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
