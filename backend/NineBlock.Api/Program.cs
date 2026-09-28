using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register In-Memory Db for lightweight run & testing (can swap to SQL Server connection string in appsettings.json)
builder.Services.AddDbContext<NineBlockDbContext>(options =>
    options.UseInMemoryDatabase("NineBlockDb"));

// Register Services
builder.Services.AddScoped<NineBoxMatrixService>();
builder.Services.AddScoped<EmployeeEvaluationService>();

// CORS for React frontend (Webpack dev server on port 3000)
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

// Seed initial database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NineBlockDbContext>();
    context.Database.EnsureCreated();
}

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
