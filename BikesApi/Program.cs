using DataBase;
using Pipeline;
using Redis;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbAppContext>(options =>options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
        builder.Configuration.GetConnectionString("DefaultConnection"))));


builder.Services.AddSingleton<MongoDbService>(sp =>new MongoDbService(builder.Configuration["MongoDb:ConnectionString"]!));
builder.Services.AddSingleton<RedisService>(sp =>new RedisService(builder.Configuration["Redis:ConnectionString"]!));
builder.Services.AddScoped<IPipeline, ApiPipeline>();

builder.Services.AddSingleton<RedisService>(_ => new RedisService("localhost:6379"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
