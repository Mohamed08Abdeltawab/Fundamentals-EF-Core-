/*
========================================================
Code Overview
--------------------------------------------------------
Purpose:
- Read connection string from appsettings.json
- Create AppDbContext manually
- Retrieve students
- Preview SQL using ToQueryString()
- Log actual executed SQL
- Compare ToQueryString() vs runtime SQL for Count()


Key Points:
- ToQueryString previews IQueryable shape
- Logging shows runtime SQL
- Count() final SQL differs from preview SQL
========================================================
*/


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrainingCenter.Data;


// Build configuration
IConfiguration configuration =
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .Build();


// Read connection string
string? connectionString =
    configuration.GetConnectionString("DefaultConnection");


if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Connection string not found.");
    return;
}


// Create options with logging
var options =
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .Options;


// Create context
using var context = new AppDbContext(options);


// Test connection
if (!context.Database.CanConnect())
{
    Console.WriteLine("Could not connect.");
    return;
}


Console.WriteLine("Connected successfully.");
Console.WriteLine();


// Run examples
//RetrieveAndPrintStudents(context);
//GetActiveStudentsCount(context);

// Call filtering function
GetActiveStudents(context);


/// <summary>
/// Retrieves only active students using Where()
/// </summary>
static void GetActiveStudents(AppDbContext context)
{
    // Build query (no execution yet)
    var query = context.Students
        .Where(s => s.Status == "Active")
        .OrderBy(s => s.StudentId);

    // Show generated SQL
    PreviewSQLUsingToQueryString(query.ToQueryString());


    // Execute query
    var students = query.ToList();

    // Print results
    Console.WriteLine("\nActive Students:");
    Console.WriteLine("----------------");

    foreach (var student in students)
    {
        Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");
    }

    Console.WriteLine();
    Console.WriteLine($"Total Active Students: {students.Count}");
}

static void PreviewSQLUsingToQueryString(string SQLString)
{
    Console.WriteLine("\nPreview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(SQLString);
    Console.WriteLine();

}




/// <summary>
/// Example 1:
/// Retrieve active students
/// </summary>
static void RetrieveAndPrintStudents(
    AppDbContext context)
{
    Console.WriteLine("Example 1 - Retrieve Students");
    Console.WriteLine("=============================");
    Console.WriteLine();


    // Build query first
    var query = context.Students
        .Where(s => s.Status == "Active")
        .OrderBy(s => s.StudentId);


    // Preview SQL
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    // Execute query
    var students = query.ToList();


    Console.WriteLine(
        $"Rows Returned: {students.Count}");


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}




/// <summary>
/// Example 2:
/// Show difference between ToQueryString() and Count() runtime SQL
/// </summary>
static void GetActiveStudentsCount(
    AppDbContext context)
{
    Console.WriteLine("Example 2 - Count Comparison");
    Console.WriteLine("============================");
    Console.WriteLine();


    // Build query first
    var query = context.Students
        .Where(s => s.Status == "Active");


    // Preview SQL before Count()
    Console.WriteLine("Preview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine();


    Console.WriteLine(
        "Now executing Count()...");
    Console.WriteLine(
        "Watch logging output above / below.");
    Console.WriteLine();


    // Actual execution
    int total = query.Count();


    Console.WriteLine(
        $"Total Active Students: {total}");


    Console.WriteLine();
    Console.WriteLine(
        "Important Note:");
    Console.WriteLine(
        "ToQueryString() previewed SELECT rows query.");
    Console.WriteLine(
        "But logging shows final executed COUNT(*) query.");


    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();
}
