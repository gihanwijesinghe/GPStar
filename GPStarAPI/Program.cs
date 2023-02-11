using Microsoft.EntityFrameworkCore;
using GPStarAPI.Data;
using System.Text.Json.Serialization;
using GPStarAPI.Systems;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<GPStarAPIContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("GPStarAPIContext") ?? throw new InvalidOperationException("Connection string 'GPStarAPIContext' not found.")));

builder.Services.AddDbContext<GPStarContext>(options =>
    options.UseCosmos(builder.Configuration["CosmosDBGPStar"],
    databaseName: "GPStar"));

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddTransient<InvoiceSystem>();

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services?.GetService<IServiceScopeFactory>()?.CreateAsyncScope();
    var context = scope?.ServiceProvider.GetRequiredService<GPStarContext>();
    var result = await context!.Database.EnsureCreatedAsync();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
