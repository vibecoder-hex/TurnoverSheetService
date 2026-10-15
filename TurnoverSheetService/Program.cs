using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Services.DbContextConfiguration;
using TurnoverSheetService.Repositories;
using TurnoverSheetService.Services.DomainServices;

var builder = WebApplication.CreateBuilder(args);

string dbConnectionString = builder.Configuration["DatabaseConnection:ConnectionString"] ?? "";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<TurnoverSheetDbContext>(options => 
    options
        .UseNpgsql(dbConnectionString)
        .UseExceptionProcessor());

builder.Services.AddHttpLogging(options => { });

builder.Services.AddScoped<IRootAggregateCommandRepository, AggregateCommandRepository>();
builder.Services.AddScoped<IRootAggregateQueryRepository, AggregateQueryRepository>();

builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();
builder.Services.AddScoped<ISaldoManagementService, SaldoManagementService>();
builder.Services.AddScoped<IChargeManagementService, ChargeManagementService>();

var app = builder.Build();

app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapGet("/api/health", () => new
{
    Message = "Добро пожаловать в сервис по оплате услуг ЖКХ."
});

app.Run();