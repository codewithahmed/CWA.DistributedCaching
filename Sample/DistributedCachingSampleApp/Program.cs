using CWA.DistributedCache;
using DistributedCachingSampleApp.Data;
using DistributedCachingSampleApp.DomainModel;
using DistributedCachingSampleApp.Settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<CacheEntryOptions>(builder.Configuration.GetSection("CacheEntryOptions"));
builder.Services.AddDistributedCaching(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=database.sqlite")); // will be created in web project root

builder.Services.AddControllers();
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleDistributedCachingApp V1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();