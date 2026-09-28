using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database Configuration: Enterprise SQL Server provider with InMemory fallback for local/test runs
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString) && !builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<NineBlockDbContext>(options =>
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
}
else
{
    builder.Services.AddDbContext<NineBlockDbContext>(options =>
        options.UseInMemoryDatabase("NineBlockDb"));
}

// Register Domain & Evaluation Services (Clean Single-Responsibility, 0% Duplication)
builder.Services.AddScoped<NineBoxMatrixService>();
builder.Services.AddScoped<EmployeeEvaluationService>();

// CORS configuration for frontend clients
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed initial in-memory database if applicable
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NineBlockDbContext>();
    context.Database.EnsureCreated();
}

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
