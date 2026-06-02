using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Получаем строку подключения из appsettings.json
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

// 👇 ОТЛАДКА 👇
Console.WriteLine("=== DEBUG: Connection String Start ===");
Console.WriteLine($"Value: '{defaultConnection}'");
Console.WriteLine($"Length: {defaultConnection?.Length ?? 0}");
Console.WriteLine($"Starts with 'Host=': {defaultConnection?.StartsWith("Host=")}");
Console.WriteLine($"Starts with 'postgres://': {defaultConnection?.StartsWith("postgres://")}");
Console.WriteLine("=== DEBUG: Connection String End ===");

// Настраиваем контекст базы данных с использованием SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(defaultConnection); 
});

// Добавляем сервисы
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// === ИНИЦИАЛИЗАЦИЯ БАЗЫ ДАННЫХ ===
// Создаём таблицы автоматически, если их нет
 using (var scope = app.Services.CreateScope())
 {
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // EnsureCreated создаёт БД и таблицы на основе ваших моделей (для SQLite)
        db.Database.EnsureCreated();
        app.Logger.LogInformation("База данных SQLite успешно инициализирована.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Ошибка при инициализации базы данных.");
    }
}
// =================================

// Настройка конвейера обработки запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Редирект на HTTPS (работает корректно на Render)
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

// Маршрутизация
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
