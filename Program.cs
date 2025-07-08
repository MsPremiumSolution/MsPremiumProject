// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data; // Verifique se este � o namespace correto para a sua AppDbContext
using MSPremiumProject.Services; // Verifique se este � o namespace correto para o seu IEmailSender

var builder = WebApplication.CreateBuilder(args);

// Configura��o de Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // N�vel 'Information' � suficiente para produ��o

// --- CONFIGURA��O DO AppDbContext (Para os seus dados de neg�cio) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A connection string 'DefaultConnection' n�o foi encontrada.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// --- CONFIGURA��O DO DataProtectionKeysContext (Para as chaves de seguran�a) ---
builder.Services.AddDbContext<DataProtectionKeysContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// --- CONFIGURA��O DO DATA PROTECTION ---
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>();

// Adicionar controladores e vistas
builder.Services.AddControllersWithViews();

// --- CONFIGURA��O DA SESS�O ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- CONFIGURA��O DA AUTENTICA��O (COM MAIS SEGURAN�A) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;

        // ADICIONADO: O cookie s� � enviado em pedidos HTTPS. Essencial em produ��o.
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // ADICIONADO: Pol�tica mais restritiva para prote��o contra CSRF.
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Adicionar outros servi�os
builder.Services.AddTransient<IEmailSender, EmailSender>();

// =========================================================================
// Constru��o da Aplica��o
var app = builder.Build();
// =========================================================================

// --- APLICAR MIGRA��ES E SEED DATA NO ARRANQUE (A FORMA SEGURA) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("A iniciar processo de migra��o da base de dados...");

        // Migrar o contexto principal da aplica��o
        var appDbContext = services.GetRequiredService<AppDbContext>();
        if (appDbContext.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Migra��es pendentes encontradas para AppDbContext. A aplicar...");
            await appDbContext.Database.MigrateAsync();
            logger.LogInformation("Migra��es do AppDbContext aplicadas com sucesso.");
        }
        else
        {
            logger.LogInformation("AppDbContext j� est� atualizado. Nenhuma migra��o a aplicar.");
        }

        // Popular dados iniciais (se necess�rio)
        await SeedData.Initialize(appDbContext);
        logger.LogInformation("SeedData verificado/inicializado.");


        // Migrar o contexto das chaves de seguran�a
        var dpContext = services.GetRequiredService<DataProtectionKeysContext>();
        if (dpContext.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Migra��es pendentes encontradas para DataProtectionKeysContext. A aplicar...");
            await dpContext.Database.MigrateAsync();
            logger.LogInformation("Migra��es do DataProtectionKeysContext aplicadas com sucesso.");
        }
        else
        {
            logger.LogInformation("DataProtectionKeysContext j� est� atualizado.");
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "ERRO FATAL: Falha durante a migra��o ou seeding da base de dados. A aplica��o vai parar.");
    }
}
// --- FIM DA SEC��O DE MIGRA��O ---

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();