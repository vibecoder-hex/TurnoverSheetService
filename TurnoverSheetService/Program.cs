using Microsoft.EntityFrameworkCore;
using TurnoverSheetService.Services.DbContextConfiguration;
using TurnoverSheetService.Repositories;
using TurnoverSheetService.Services.DomainServices;

var builder = WebApplication.CreateBuilder(args);

string dbConnectionString = builder.Configuration["DatabaseConnection:ConnectionString"] ?? "";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextPool<TurnoverSheetDbContext>(options => options.UseNpgsql(dbConnectionString));
builder.Services.AddHttpLogging(options => { });

builder.Services.AddScoped<ISaldoRepository, ApartmentSaldoRepository>();
builder.Services.AddScoped<IChargesRepository, ApartmentChargesRepository>();
builder.Services.AddScoped<IPaymentsRepository, ApartmentPaymentsRepository>();
builder.Services.AddScoped<IApartmentsRepository, ApartmentsRepository>();

builder.Services.AddScoped<ISaldoCalculationService, SaldoCalculationService>();
builder.Services.AddScoped<IApartmentManagementService, ApartmentManagementService>();
builder.Services.AddScoped<IPaymentManagementService, PaymentManagementService>();

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