using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(defaultConnection); 
});

// Добавляем Razor Pages и сессии для простой авторизации.
builder.Services.AddRazorPages();
builder.Services.AddSession();

var app = builder.Build();

// Проверка доступности базы данных (сайт и WinForms должны использовать одну БД).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        if (!db.Database.CanConnect())
        {
            app.Logger.LogWarning("Не удалось подключиться к базе данных FitnessClubDB (LocalDB).");
        }
    }
    catch (Exception ex)
    {
        // Не падаем при старте, но логируем — иначе Visual Studio воспринимает это как падение сайта.
        app.Logger.LogError(ex, "Ошибка при проверке подключения к базе данных.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// В Development при запуске только по HTTP (профиль "http") редирект на HTTPS приводит к предупреждению.
// Включаем редирект только когда известен HTTPS порт или не Development.
if (!app.Environment.IsDevelopment() || !string.IsNullOrWhiteSpace(app.Configuration["ASPNETCORE_HTTPS_PORT"]))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapRazorPages();

app.Run();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // Важно для wwwroot

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
