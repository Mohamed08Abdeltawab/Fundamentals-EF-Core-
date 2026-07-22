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
//GetActiveStudents(context);



// Call examples
Example_First(context);
Example_FirstOrDefault(context);
Example_Single(context);
Example_SingleOrDefault(context);


// Call main methods
GetStudentByIdUsingFind(context);




// Call main methods
GetStudentNames(context);




// Call main methods
GetFilteredStudents(context);



// Call main methods
CheckData(context);




// Call main methods
CompareCount(context);
PrintSeparator();

CompareAverage(context);
PrintSeparator();

CompareSum(context);


/// <summary>
/// Compares bad vs good COUNT approach.
/// </summary>
static void CompareCount(AppDbContext context)
{
    Console.WriteLine("COUNT EXAMPLE");
    Console.WriteLine();

    Console.WriteLine("BAD WAY:");
    Console.WriteLine();

    // Build query first
    var badQuery =
        context.Students;

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(badQuery.ToQueryString());

    // Execute query and load all rows into memory
    var students = badQuery.ToList();

    // Count happens in memory after data is already loaded
    int badCount =
        students.Count(s => s.Status == "Active");

    Console.WriteLine($"Bad Count (calculated in memory): {badCount}");
    Console.WriteLine();

    Console.WriteLine("GOOD WAY:");
    Console.WriteLine();

    // Build query first
    var goodQuery =
        context.Students
               .Where(s => s.Status == "Active");

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(goodQuery.ToQueryString());

    // Execute COUNT in the database
    // ToQueryString previews query shape,
    // runtime logging shows actual executed SQL for Count().
    int goodCount =
        goodQuery.Count();

    Console.WriteLine($"Good Count (calculated in database): {goodCount}");
    Console.WriteLine();
}


/// <summary>
/// Compares bad vs good AVERAGE approach.
/// </summary>
static void CompareAverage(AppDbContext context)
{
    Console.WriteLine("AVERAGE EXAMPLE");
    Console.WriteLine();

    Console.WriteLine("BAD WAY:");
    Console.WriteLine();

    // Build query first
    var badQuery =
        context.Enrollments;

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(badQuery.ToQueryString());

    // Execute query and load all rows into memory
    var enrollments = badQuery.ToList();

    // Average happens in memory after data is already loaded
    decimal badAverage =
        enrollments.Average(e => e.ProgressPercent);

    Console.WriteLine($"Bad Average (calculated in memory): {badAverage}");
    Console.WriteLine();

    Console.WriteLine("GOOD WAY:");
    Console.WriteLine();

    // Build query first
    var goodQuery =
        context.Enrollments
               .Select(e => e.ProgressPercent);

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(goodQuery.ToQueryString());

    // Execute AVERAGE in the database
    // ToQueryString previews query shape,
    // runtime logging shows actual executed SQL for Average().
    decimal goodAverage =
        goodQuery.Average();

    Console.WriteLine($"Good Average (calculated in database): {goodAverage}");
    Console.WriteLine();
}


/// <summary>
/// Compares bad vs good SUM approach.
/// </summary>
static void CompareSum(AppDbContext context)
{
    Console.WriteLine("SUM EXAMPLE");
    Console.WriteLine();

    Console.WriteLine("BAD WAY:");
    Console.WriteLine();

    // Build query first
    var badQuery =
        context.Courses;

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(badQuery.ToQueryString());

    // Execute query and load all rows into memory
    var courses = badQuery.ToList();

    // Sum happens in memory after data is already loaded
    int badSum =
        courses.Sum(c => c.DurationHours);

    Console.WriteLine($"Bad Sum (calculated in memory): {badSum}");
    Console.WriteLine();

    Console.WriteLine("GOOD WAY:");
    Console.WriteLine();

    // Build query first
    var goodQuery =
        context.Courses
               .Select(c => c.DurationHours);

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(goodQuery.ToQueryString());

    // Execute SUM in the database
    // ToQueryString previews query shape,
    // runtime logging shows actual executed SQL for Sum().
    int goodSum =
        goodQuery.Sum();

    Console.WriteLine($"Good Sum (calculated in database): {goodSum}");
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
/// Demonstrates Any() and All().
/// </summary>
static void CheckData(AppDbContext context)
{
    Console.WriteLine("Any() and All() Example");
    Console.WriteLine("-----------------------");
    Console.WriteLine();

    // --------------------------------------------------
    // Any() Example
    // --------------------------------------------------

    // Build query first
    var activeStudentsQuery =
        context.Students
               .Where(s => s.Status == "Active");

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(activeStudentsQuery.ToQueryString());

    // Execute query
    // ToQueryString previews query shape,
    // runtime logging shows actual executed SQL for Any().
    bool hasActiveStudents =
        activeStudentsQuery.Any();

    Console.WriteLine($"Has Active Students: {hasActiveStudents}");
    Console.WriteLine();

    // --------------------------------------------------
    // All() Example
    // --------------------------------------------------

    // Build query first
    var coursesQuery =
        context.Courses;

    // Preview SQL query shape
    PreviewSQLUsingToQueryString(coursesQuery.ToQueryString());

    // Execute query
    // ToQueryString previews query shape,
    // runtime logging shows actual executed SQL for All().
    bool allCoursesValid =
        coursesQuery.All(c => c.Price > 0);

    Console.WriteLine($"All Courses Price > 0: {allCoursesValid}");
    Console.WriteLine();
}




/// <summary>
/// Demonstrates combining Where(), Select(), and OrderByDescending().
/// </summary>
static void GetFilteredStudents(AppDbContext context)
{
    Console.WriteLine("Filtered Projection With Sorting");
    Console.WriteLine("--------------------------------");
    Console.WriteLine();

    // Build query first
    var query = context.Students.Where(s => s.Status == "Active")
        .Select(s => new
        {
            s.StudentId,
            FullName = s.FirstName + " " + s.LastName,
        })
        .OrderBy(s => s.FullName)
        .ThenBy(s => s.StudentId);

    // Preview SQL before execution
    PreviewSQLUsingToQueryString(query.ToQueryString());

    // Execute query
    var students = query.ToList();

    // Print results
    Console.WriteLine("\n\nFiltered Students:");
    Console.WriteLine("------------------");

    foreach (var student in students)
    {
        Console.WriteLine($"{student.StudentId} - {student.FullName}");
    }

    Console.WriteLine();
    Console.WriteLine($"Total Students: {students.Count}");
    Console.WriteLine();
}


/// <summary>
/// Retrieves only student names using projection.
/// </summary>
static void GetStudentNames(AppDbContext context)
{
    Console.WriteLine("Projection Example Using Select()");
    Console.WriteLine("---------------------------------");
    Console.WriteLine();

    // Build query first (no execution yet)
    var query =
        context.Students
               .Select(s => new
               {
                   s.FirstName,
                   s.LastName
               });

    // Preview SQL before execution
    PreviewSQLUsingToQueryString(query.ToQueryString());

    // Execute query
    var students = query.ToList();

    // Print results
    Console.WriteLine("\n\nStudent Names:");
    Console.WriteLine("--------------");

    foreach (var student in students)
    {
        Console.WriteLine($"{student.FirstName} {student.LastName}");
    }

    Console.WriteLine();
    Console.WriteLine($"\nTotal Students: {students.Count}");
    Console.WriteLine();
}





/// <summary>
/// Retrieves student by Primary Key using Find().
/// Best method for direct PK lookup.
/// May return tracked entity without executing SQL again.
/// </summary>
static void GetStudentByIdUsingFind(AppDbContext context)
{
    Console.WriteLine("Using Find()");
    Console.WriteLine("------------");

    // Find() does not support ToQueryString().
    // Runtime logging will show actual SQL only if query is sent to database.
    var student = context.Students.Find(1);

    PrintStudent(student);
}


/// <summary>
/// Prints student information in readable format.
/// </summary>
static void PrintStudent(dynamic? student)
{
    if (student != null)
    {
        Console.WriteLine("\n\nStudent Found:");
        Console.WriteLine(
            $"{student.StudentId} - {student.FirstName} {student.LastName}");
    }
    else
    {
        Console.WriteLine("Student not found.");
    }

    Console.WriteLine();
}



/// <summary>
/// First() returns the first matching row.
/// Use it when at least one row is expected.
/// </summary>
static void Example_First(AppDbContext context)
{
    Console.WriteLine("\nExample 1 - First()");
    Console.WriteLine("-------------------");

    // Build query first (no execution yet)
    var query = context.Students
        .Where(s => s.Status == "Active")
        .OrderBy(s => s.StudentId);

    // Preview query shape
    PreviewSQLUsingToQueryString(query.ToQueryString());

    // Execute query
    // Runtime logging will show the actual executed SQL.
    var student = query.First();

    Console.WriteLine("\nFirst Active Student:");
    Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");
}


/// <summary>
/// FirstOrDefault() returns the first matching row,
/// or null if no row exists.
/// </summary>
static void Example_FirstOrDefault(AppDbContext context)
{
    Console.WriteLine("\nExample 2 - FirstOrDefault()");
    Console.WriteLine("----------------------------");

    // Build query first (no execution yet)
    var query = context.Students
        .Where(s => s.Email == "notfound@student.com");

    // Preview query shape
    PreviewSQLUsingToQueryString(query.ToQueryString());

    // Execute query
    // Runtime logging will show the actual executed SQL.
    var student = query.FirstOrDefault();

    if (student == null)
    {
        Console.WriteLine("\nNo student found.");
    }
    else
    {
        Console.WriteLine("\nStudent Found:");
        Console.WriteLine($"{student.StudentId} - {student.FirstName} {student.LastName}");
    }
}


/// <summary>
/// Single() expects exactly one matching row.
/// Use it when the data must be unique.
/// </summary>
static void Example_Single(AppDbContext context)
{
    Console.WriteLine("\nExample 3 - Single()");
    Console.WriteLine("--------------------");

    // Build query first (no execution yet)
    var query = context.Courses
        .Where(c => c.Code == "EF-101");

    // Preview query shape
    PreviewSQLUsingToQueryString(query.ToQueryString());

    // Execute query
    // Runtime logging will show the actual executed SQL.
    var course = query.Single();

    Console.WriteLine("\nCourse Found:");
    Console.WriteLine($"{course.CourseId} - {course.Code} - {course.Title}");
}


/// <summary>
/// SingleOrDefault() expects zero or one matching row.
/// Returns null if none exists, but throws if duplicates exist.
/// </summary>
static void Example_SingleOrDefault(AppDbContext context)
{
    Console.WriteLine("\nExample 4 - SingleOrDefault()");
    Console.WriteLine("-----------------------------");

    // Build query first (no execution yet)
    var query = context.Courses
        .Where(c => c.Code == "UNKNOWN-999");

    // Preview query shape
    PreviewSQLUsingToQueryString(query.ToQueryString());

    // Execute query
    // Runtime logging will show the actual executed SQL.
    var course = query.SingleOrDefault();

    if (course == null)
    {
        Console.WriteLine("\nNo course found.");
    }
    else
    {
        Console.WriteLine("\nCourse Found:");
        Console.WriteLine($"{course.CourseId} - {course.Code} - {course.Title}");
    }
}

static void PreviewSQLUsingToQueryString(string SQLString)
{
    Console.WriteLine("\nPreview SQL using ToQueryString():");
    Console.WriteLine("----------------------------------");
    Console.WriteLine(SQLString);
    Console.WriteLine();
}


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
