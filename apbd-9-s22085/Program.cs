using apbd_8_s22085.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    opt.LogTo(Console.WriteLine);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// global exception file logger
app.Use(async (context, next) =>
{
    try
    {
        await next();
    } catch (Exception e)
    {
        // append to logs.txt
        await File.AppendAllTextAsync("logs.txt", $"{e}\n");
        // pass exception to default exception handler
        throw;
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();