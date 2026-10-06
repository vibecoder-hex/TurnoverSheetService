var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/health", () => new
{
    Message = "Добро пожаловать в сервис оборотной ведомости оплате услуг ЖКХ."
});

app.Run();