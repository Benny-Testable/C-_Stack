using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// SQL Server Database Provider with InMemory fallback for isolated testing
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NineBlockDbContext>(options =>
{
    if (!string.IsNullOrEmpty(connectionString) && !builder.Environment.IsEnvironment("Testing"))
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
        });
    }
    else
    {
        options.UseInMemoryDatabase("NineBlockDb");
    }
});

// Register Services
builder.Services.AddScoped<NineBoxMatrixService>();
builder.Services.AddScoped<EmployeeEvaluationService>();

// CORS for React frontend (dev server on port 3000)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Ensure Database schema exists & seed initial reference data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NineBlockDbContext>();
    context.Database.EnsureCreated();
}

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
