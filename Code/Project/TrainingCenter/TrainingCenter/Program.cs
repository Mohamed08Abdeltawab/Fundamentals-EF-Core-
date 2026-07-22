using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrainingCenter.Data;

// Build configuration
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Read connection string
string? connectionString = configuration.GetConnectionString("DefaultConnection");

// Validate connection string.
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("Connection string not found.");
    return;
}


// Configure EF Core
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

// Create context
using var context = new AppDbContext(options);

// Test
Console.WriteLine("================================================");

Console.WriteLine(context.Database.CanConnect()
    ? "Connected! You are ready to retrieve Data :-)"
    : "Failed to connect.");

Console.WriteLine("================================================");

