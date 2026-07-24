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









// Call main methods
int newStudentId = InsertStudent(context);

PrintSeparator();

DeleteStudent(context, newStudentId);


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
/// Prints a separator between examples.
/// </summary>
static void PrintSeparator()
{
    Console.WriteLine(new string('-', 60));
    Console.WriteLine();
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