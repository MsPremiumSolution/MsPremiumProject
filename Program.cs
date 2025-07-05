// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura��o de Logging para todos os n�veis, incluindo Trace/Debug
builder.Logging.ClearProviders(); // Limpa os providers padr�o
builder.Logging.AddConsole(); // Adiciona console logger
builder.Logging.SetMinimumLevel(LogLevel.Trace); // Configura o n�vel m�nimo para TRACE

// --- CONFIGURA��O DO AppDbContext (Para os seus dados de neg�cio) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
    .LogTo(Console.WriteLine, LogLevel.Information) // J� tem isto, mas o n�vel acima garante mais detalhes
);

// --- CONFIGURA��O DO DataProtectionKeysContext (Para as chaves de seguran�a) ---
builder.Services.AddDbContext<DataProtectionKeysContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
    .LogTo(Console.WriteLine, LogLevel.Information) // Adicionar log para este contexto tamb�m
);

// --- CONFIGURA��O DO DATA PROTECTION ---
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>();

// Resto dos teus servi�os
builder.Services.AddControllersWithViews();

// --- CONFIGURA��O DA SESS�O ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- CONFIGURA��O DA AUTENTICA��O ---
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
// builder.Services.AddLogging(); // J� configurado acima com ClearProviders e AddConsole

// =========================================================================
// Constru��o da Aplica��o
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
        // MUITO IMPORTANTE: Garante que qualquer erro no arranque da BD � fatal
        app.Logger.LogCritical(ex, "FATAL ERROR: Application failed to start due to database or migration issues.");
        throw; // Force o crash para que o Render.com mostre o erro nos logs
    }
}
// --- FIM DA APLICA��O DE MIGRATIONS E SEED DATA NO ARRANQUE ---


// Pipeline de Pedidos HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// A ordem � CR�TICA para middleware de seguran�a e sess�es:
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();