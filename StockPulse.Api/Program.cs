using Microsoft.EntityFrameworkCore;
using StockPulse.Api.Data;
using StockPulse.Api.Repositories;
using StockPulse.Api.External;
using StockPulse.Api.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStockRepository, StockRepository>();

builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.Configure<FinnhubOptions>(
    builder.Configuration.GetSection("Finnhub"));

builder.Services.AddHttpClient<IFinancialDataProvider, FinnhubFinancialDataProvider>(client =>
{
    client.BaseAddress = new Uri("https://finnhub.io/api/v1/");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
