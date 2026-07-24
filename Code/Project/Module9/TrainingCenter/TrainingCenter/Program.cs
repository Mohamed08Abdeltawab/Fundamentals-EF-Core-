/*
========================================================
Code Overview
--------------------------------------------------------
Purpose:
- Demonstrate Insert and Delete in EF Core
- Show SQL preview where ToQueryString() is available
- Retrieve generated ID after insert
- Delete a student using StudentId
- Show actual INSERT and DELETE SQL using runtime logging

Key Points:
- Add() marks entity as Added
- Remove() marks entity as Deleted
- SaveChanges() triggers INSERT and DELETE execution
- Generated ID is available after SaveChanges()
- ToQueryString() previews SELECT query shape
- Runtime logging shows actual executed INSERT and DELETE SQL
========================================================
*/

using System.Diagnostics;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TrainingCenter.Data;
using TrainingCenter.Entities;


// Configuration setup
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();


// Read connection string
string? connectionString =
    configuration.GetConnectionString("DefaultConnection");


// Validate values
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Connection string not found.");
    return;
}


// Create options
var options =
    new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .Options;


// Create context
using var context = new AppDbContext(options);


// Test connection if relevant
if (!context.Database.CanConnect())
{
    Console.WriteLine("Database connection failed.");
    return;
}

Console.WriteLine("Connected successfully.");
Console.WriteLine();


/*
// Simulate API DTO
var dto = new UpdateStudentDto
{
    StudentId = 1,
    FirstName = "UpdatedName",
    PhoneNumber = "0799999999"
};

// Run examples
UpdateUsingLoadThenUpdate(context, dto);

context.ChangeTracker.Clear();

Console.WriteLine(new string('=', 70));

UpdateUsingControlledAttach(context, dto);

context.ChangeTracker.Clear();

Console.WriteLine(new string('=', 70));

UpdateUsingAttach(context, dto);

*/







/*

// Call main methods
int newStudentId = InsertStudent(context);

PrintSeparator();

DeleteStudent(context, newStudentId);


*/

/*
// Call main methods
long badTime = RunBadBulkInsert(context);

PrintSeparator();

long goodTime = RunGoodBulkInsert(context);

PrintSeparator();

long bulkTime = RunBestBulkInsertUsingBulkExtensions(context);

PrintSeparator();

ShowComparison(badTime, goodTime, bulkTime);

*/



// Call main methods
RunBadExampleWithoutTransaction(context);

PrintSeparator();

RunGoodExampleWithTransaction(context);


/// <summary>
/// Demonstrates the bad approach where multiple operations are executed without a transaction.
/// </summary>
static void RunBadExampleWithoutTransaction(AppDbContext context)
{
    Console.WriteLine("BAD EXAMPLE - No Transaction");
    Console.WriteLine("----------------------------");
    Console.WriteLine();

    try
    {
        var student = new Student
        {
            FirstName = "Bad",
            LastName = "Transaction",
            Email = "bad@test.com",
            Status = "Active",
            RegisteredAt = DateTime.Now
        };

        // Add() marks the entity as Added
        context.Students.Add(student);

        Console.WriteLine("Student added to Change Tracker as Added.");
        Console.WriteLine("INSERT does not support ToQueryString().");
        Console.WriteLine("Runtime logging will show the actual executed INSERT SQL.");
        Console.WriteLine();

        // SaveChanges() executes INSERT immediately
        int affectedRows = context.SaveChanges();

        Console.WriteLine($"Student inserted. Affected Rows: {affectedRows}");
        Console.WriteLine($"Generated Student ID: {student.StudentId}");
        Console.WriteLine();

        // Simulate failure after the first save
        throw new Exception("Something went wrong after inserting student.");

        // This code will not execute because of the simulated error above
        var course = new Course
        {
            Title = "Broken Course",
            DurationHours = 10
        };

        context.Courses.Add(course);
        context.SaveChanges();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine("Problem: Student was saved, but the second operation was not completed.");
        Console.WriteLine();
    }
}


/// <summary>
/// Demonstrates the good approach where multiple operations are protected by a transaction.
/// </summary>
static void RunGoodExampleWithTransaction(AppDbContext context)
{
    Console.WriteLine("GOOD EXAMPLE - With Transaction");
    Console.WriteLine("-------------------------------");
    Console.WriteLine();

    using var transaction = context.Database.BeginTransaction();

    try
    {
        var student = new Student
        {
            FirstName = "Good",
            LastName = "Transaction",
            Email = "good@test.com",
            Status = "Active",
            RegisteredAt = DateTime.Now
        };

        // Add() marks the entity as Added
        context.Students.Add(student);

        Console.WriteLine("Student added to Change Tracker as Added.");
        Console.WriteLine("INSERT does not support ToQueryString().");
        Console.WriteLine("Runtime logging will show the actual executed INSERT SQL.");
        Console.WriteLine();

        // SaveChanges() executes INSERT inside the transaction
        int affectedRows = context.SaveChanges();

        Console.WriteLine($"Student inserted inside transaction. Affected Rows: {affectedRows}");
        Console.WriteLine($"Generated Student ID: {student.StudentId}");
        Console.WriteLine();

        // Simulate failure before the transaction is committed
        throw new Exception("Failure inside transaction.");

        // This code will not execute because of the simulated error above
        var course = new Course
        {
            Title = "Safe Course",
            DurationHours = 20
        };

        context.Courses.Add(course);
        context.SaveChanges();

        // Commit only if all operations succeed
        transaction.Commit();

        Console.WriteLine("Transaction committed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");

        // Rollback cancels all operations inside this transaction
        transaction.Rollback();

        Console.WriteLine("Transaction rolled back. Nothing was saved from this transaction.");
        Console.WriteLine();
    }
}


/// <summary>
/// Prints a separator between examples.
/// </summary>
static void PrintSeparator()
{
    Console.WriteLine(new string('-', 60));
    Console.WriteLine();
}



/// <summary>
/// Demonstrates the bad bulk insert approach by calling SaveChanges() inside the loop.
/// </summary>
static long RunBadBulkInsert(AppDbContext context)
{
    Console.WriteLine("BAD BULK INSERT - SaveChanges() Inside Loop");
    Console.WriteLine("-------------------------------------------");
    Console.WriteLine();

    var stopwatch = Stopwatch.StartNew();

    for (int i = 1; i <= 10; i++)
    {
        var student = new Student
        {
            FirstName = "Bad",
            LastName = $"Student{i}",
            Email = $"bad{i}@test.com",
            Status = "Active",
            RegisteredAt = DateTime.Now
        };

        context.Students.Add(student);

        // BAD: SaveChanges() inside the loop creates many database calls
        int affectedRows = context.SaveChanges();

        Console.WriteLine(
            $"Inserted Student {i} | Generated ID: {student.StudentId} | Affected Rows: {affectedRows}");
    }

    stopwatch.Stop();

    Console.WriteLine();
    Console.WriteLine($"Time Taken - BAD: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine("Result: Many database calls were executed.");
    Console.WriteLine();

    return stopwatch.ElapsedMilliseconds;
}


/// <summary>
/// Demonstrates the good bulk insert approach by adding all entities first,
/// then calling SaveChanges() only once.
/// </summary>
static long RunGoodBulkInsert(AppDbContext context)
{
    Console.WriteLine("GOOD BULK INSERT - Single SaveChanges()");
    Console.WriteLine("---------------------------------------");
    Console.WriteLine();

    var stopwatch = Stopwatch.StartNew();

    var students = new List<Student>();

    for (int i = 1; i <= 10000; i++)
    {
        students.Add(new Student
        {
            FirstName = "Good",
            LastName = $"Student{i}",
            Email = $"good{i}@test.com",
            Status = "Active",
            RegisteredAt = DateTime.Now
        });
    }

    context.Students.AddRange(students);

    Console.WriteLine("Students added to Change Tracker as Added.");
    Console.WriteLine("INSERT does not support ToQueryString().");
    Console.WriteLine("Runtime logging will show the actual executed INSERT SQL.");
    Console.WriteLine();

    int affectedRows = context.SaveChanges();

    stopwatch.Stop();

    Console.WriteLine($"Affected Rows: {affectedRows}");
    Console.WriteLine($"Time Taken - GOOD: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine("Result: One SaveChanges() call was executed.");
    Console.WriteLine();

    return stopwatch.ElapsedMilliseconds;
}


/// <summary>
/// Demonstrates the best bulk insert approach using EFCore.BulkExtensions.
/// </summary>
static long RunBestBulkInsertUsingBulkExtensions(AppDbContext context)
{
    Console.WriteLine("BEST BULK INSERT - EFCore.BulkExtensions");
    Console.WriteLine("----------------------------------------");
    Console.WriteLine();

    var stopwatch = Stopwatch.StartNew();

    var students = new List<Student>();

    for (int i = 1; i <= 10000; i++)
    {
        students.Add(new Student
        {
            FirstName = "Bulk",
            LastName = $"Student{i}",
            Email = $"bulk{i}@test.com",
            Status = "Active",
            RegisteredAt = DateTime.Now
        });
    }

    Console.WriteLine("Students prepared in memory.");
    Console.WriteLine("BulkInsert() performs optimized database bulk operation.");
    Console.WriteLine("BulkInsert() does not use SaveChanges().");
    Console.WriteLine();

    context.BulkInsert(students);

    stopwatch.Stop();

    Console.WriteLine($"Inserted Rows: {students.Count}");
    Console.WriteLine($"Time Taken - BULK: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine("Result: Optimized bulk insert operation was executed.");
    Console.WriteLine();

    return stopwatch.ElapsedMilliseconds;
}


/// <summary>
/// Shows a simple performance comparison between the bad, good, and bulk approaches.
/// </summary>
static void ShowComparison(long badTime, long goodTime, long bulkTime)
{
    Console.WriteLine("COMPARISON");
    Console.WriteLine("----------");
    Console.WriteLine($"BAD  Time : {badTime} ms");
    Console.WriteLine($"GOOD Time : {goodTime} ms");
    Console.WriteLine($"BULK Time : {bulkTime} ms");
    Console.WriteLine();
    Console.WriteLine("Fewer SaveChanges() calls usually means better performance.");
    Console.WriteLine("BulkExtensions is usually best for large datasets.");
}




/// <summary>
/// Displays generated SQL before execution.
/// </summary>
static void PreviewSQLUsingToQueryString(string SQLString)
{
    Console.WriteLine("\nPreview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(SQLString);
    Console.WriteLine();
}


/// <summary>
/// Inserts a new student and returns the generated StudentId.
/// </summary>
static int InsertStudent(AppDbContext context)
{
    Console.WriteLine("INSERT EXAMPLE");
    Console.WriteLine("--------------");
    Console.WriteLine();

    // Create a new Student entity in memory
    var newStudent = new Student
    {
        FirstName = "Ali",
        LastName = "Hassan",
        Email = "ali.hassan@example.com",
        Status = "Active",
        RegisteredAt = DateTime.Now
    };

    // Add() tells EF Core to track this entity as Added
    context.Students.Add(newStudent);

    Console.WriteLine("Student added to Change Tracker as Added.");
    Console.WriteLine("INSERT does not support ToQueryString().");
    Console.WriteLine("Runtime logging will show the actual executed INSERT SQL.");
    Console.WriteLine();

    // Execute INSERT
    int affectedRows = context.SaveChanges();

    Console.WriteLine($"Affected Rows       : {affectedRows}");
    Console.WriteLine($"Generated Student ID: {newStudent.StudentId}");
    Console.WriteLine("Student inserted successfully.");
    Console.WriteLine();

    return newStudent.StudentId;
}


/// <summary>
/// Deletes a student by StudentId.
/// </summary>
static void DeleteStudent(AppDbContext context, int studentId)
{
    Console.WriteLine("DELETE EXAMPLE");
    Console.WriteLine("--------------");
    Console.WriteLine();

    // Build query first
    var query =
        context.Students
               .Where(s => s.StudentId == studentId);

    // Preview SELECT SQL before execution
    PreviewSQLUsingToQueryString(query.ToQueryString());

    // Execute query
    // ToQueryString previews query shape,
    // runtime logging shows actual executed SQL for FirstOrDefault().
    var student = query.FirstOrDefault();

    if (student == null)
    {
        Console.WriteLine($"No student found with ID = {studentId}");
        return;
    }

    Console.WriteLine(
        $"Deleting Student: {student.FirstName} {student.LastName} (ID = {student.StudentId})");

    // Remove() tells EF Core to track this entity as Deleted
    context.Students.Remove(student);

    Console.WriteLine();
    Console.WriteLine("Student marked as Deleted in Change Tracker.");
    Console.WriteLine("DELETE does not support ToQueryString().");
    Console.WriteLine("Runtime logging will show the actual executed DELETE SQL.");
    Console.WriteLine();

    // Execute DELETE
    int affectedRows = context.SaveChanges();

    Console.WriteLine($"Affected Rows: {affectedRows}");
    Console.WriteLine("Student deleted successfully.");
    Console.WriteLine();
}



/// <summary>
/// Pattern 1: Load Then Update (Recommended)
/// </summary>
static void UpdateUsingLoadThenUpdate(
    AppDbContext context,
    UpdateStudentDto dto)
{
    Console.WriteLine("\n\nPattern 1 - Load Then Update");
    Console.WriteLine("----------------------------");

    var student = context.Students
        .FirstOrDefault(s => s.StudentId == dto.StudentId);

    if (student == null)
    {
        Console.WriteLine("Student not found.");
        return;
    }

    student.FirstName = dto.FirstName;
    student.PhoneNumber = dto.PhoneNumber;

    context.SaveChanges();

    Console.WriteLine("Updated using Load-Then-Update.");
}


/// <summary>
/// Pattern 2: Controlled Attach (Update specific fields only)
/// </summary>
static void UpdateUsingControlledAttach(
    AppDbContext context,
    UpdateStudentDto dto)
{
    Console.WriteLine("\n\nPattern 2 - Controlled Attach");
    Console.WriteLine("-----------------------------");

    var student = new Student
    {
        StudentId = dto.StudentId,
        PhoneNumber = dto.PhoneNumber
    };

    context.Attach(student);

    context.Entry(student)
        .Property(s => s.PhoneNumber)
        .IsModified = true;

    context.SaveChanges();

    Console.WriteLine("Updated using Attach + IsModified.");
}


/// <summary>
/// Pattern 3: Direct Update using Update()
/// ⚠️ Dangerous with partial DTO
/// </summary>
static void UpdateUsingAttach(
    AppDbContext context,
    UpdateStudentDto dto)
{
    Console.WriteLine("\n\nPattern 3 - Update() (Full Update)");
    Console.WriteLine("----------------------------------");

    var student = new Student
    {
        StudentId = dto.StudentId,
        FirstName = dto.FirstName,
        PhoneNumber = dto.PhoneNumber
    };

    try
    {
        context.Students.Update(student);
        context.SaveChanges();

        Console.WriteLine("Updated using Update().");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("\n\n Update() failed due to missing required fields.");
        Console.WriteLine("Reason:");
        Console.WriteLine(ex.InnerException?.Message);
    }
}


/// <summary>
/// Simulates data coming from API request
/// </summary>
public class UpdateStudentDto
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}