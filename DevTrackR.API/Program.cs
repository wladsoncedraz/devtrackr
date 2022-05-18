using DevTrackR.API.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DevTrackRCs");

builder.Services
    .AddDbContext<DevTrackRContext>(o => o.UseSqlServer(connectionString));

Console.WriteLine($"Connection String: {connectionString}");

// builder.Services
//     .AddDbContext<DevTrackRContext>(o => o.UseInMemoryDatabase("DevTrackR"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
