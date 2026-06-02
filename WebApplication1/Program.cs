using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = "Host=dpg-d8fea1egvqtc73985vlg-a.oregon-postgres.render.com;Port=5432;Database=fitness_club_lve4;Username=fitness_user;Password=5Iepw97QzAS37f6VqeopPFTZZlVkYq3h;SSL Mode=Require;Trust Server Certificate=true;";

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
