// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Logging para todos os níveis, incluindo Trace/Debug
builder.Logging.ClearProviders(); // Limpa os providers padrão
builder.Logging.AddConsole(); // Adiciona console logger
builder.Logging.SetMinimumLevel(LogLevel.Trace); // Configura o nível mínimo para TRACE

// --- CONFIGURAÇÃO DO AppDbContext (Para os seus dados de negócio) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
    .LogTo(Console.WriteLine, LogLevel.Information) // Já tem isto, mas o nível acima garante mais detalhes
);

// --- CONFIGURAÇÃO DO DataProtectionKeysContext (Para as chaves de segurança) ---
builder.Services.AddDbContext<DataProtectionKeysContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
    .LogTo(Console.WriteLine, LogLevel.Information) // Adicionar log para este contexto também
);

// --- CONFIGURAÇÃO DO DATA PROTECTION ---
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>();

// Resto dos teus serviços
builder.Services.AddControllersWithViews();

// --- CONFIGURAÇÃO DA SESSÃO ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- CONFIGURAÇÃO DA AUTENTICAÇÃO ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

builder.Services.AddTransient<IEmailSender, EmailSender>();
// builder.Services.AddLogging(); // Já configurado acima com ClearProviders e AddConsole

// =========================================================================
// Construção da Aplicação
var app = builder.Build();
// =========================================================================

// --- APLICAR MIGRATIONS E SEED DATA NO ARRANQUE ---
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        app.Logger.LogInformation("Attempting to apply database migrations and seeding...");

        var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        appDbContext.Database.Migrate();
        app.Logger.LogInformation("AppDbContext migrations applied.");

        await SeedData.Initialize(appDbContext);
        app.Logger.LogInformation("SeedData initialization completed.");

        var dpContext = serviceProvider.GetRequiredService<DataProtectionKeysContext>();
        dpContext.Database.Migrate();
        app.Logger.LogInformation("DataProtectionKeysContext migrations applied.");

        app.Logger.LogInformation("Database migrations and seeding applied successfully.");
    }
    catch (Exception ex)
    {
        // MUITO IMPORTANTE: Garante que qualquer erro no arranque da BD é fatal
        app.Logger.LogCritical(ex, "FATAL ERROR: Application failed to start due to database or migration issues.");
        throw; // Force o crash para que o Render.com mostre o erro nos logs
    }
}
// --- FIM DA APLICAÇÃO DE MIGRATIONS E SEED DATA NO ARRANQUE ---


// Pipeline de Pedidos HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// A ordem é CRÍTICA para middleware de segurança e sessões:
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();