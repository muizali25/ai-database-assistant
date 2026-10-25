using Microsoft.EntityFrameworkCore;
using MiniPosAi.Data;
using MiniPosAi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddScoped<QueryExecutor>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Recreate in Development so updated seed data is applied.
    if (app.Environment.IsDevelopment())
        db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

app.UseCors();
app.MapControllers();
app.Run();
