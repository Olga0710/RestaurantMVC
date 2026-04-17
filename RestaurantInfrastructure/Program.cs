using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantInfrastructure;
using RestaurantInfrastructure.Services;
using RestDomain.Models;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --- 1. РЕЄСТРАЦІЯ СЕРВІСІВ ---
builder.Services.AddControllersWithViews();

// КРИТИЧНО ВАЖЛИВО: додаємо підтримку Razor Pages для Identity
builder.Services.AddRazorPages();

// 2. Основна база даних (Розклад/Працівники)
builder.Services.AddDbContext<DbRestaurantContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. База даних для Identity (Користувачі/Паролі)
builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));

// 4. Налаштування Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<IdentityContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // ЦЕЙ РЯДОК ПІДКЛЮЧАЄ ГОТОВІ СТОРІНКИ ВХОДУ (виправляє 404)

// 5. Твої сервіси
builder.Services.AddScoped<IDataPortServiceFactory<Employer>, EmployerDataPortFactory>();

var app = builder.Build();

// --- 6. ІНІЦІАЛІЗАЦІЯ РОЛЕЙ ТА АДМІНА ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleInitializer.InitializeAsync(userManager, rolesManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка при створенні початкових даних (Seeding).");
    }
}

// --- 7. НАЛАШТУВАННЯ КОНВЕЄРА (MIDDLEWARE) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ВАЖЛИВИЙ ПОРЯДОК: Authentication ПЕРЕД Authorization
app.UseAuthentication();
app.UseAuthorization();

// Маршрут для MVC (HomeController тощо)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Маршрут для Identity (Login/Register) - ТЕПЕР ПРАЦЮВАТИМЕ
app.MapRazorPages();

app.Run();