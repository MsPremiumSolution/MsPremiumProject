// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data; // Verifique se este é o namespace correto para a sua AppDbContext
using MSPremiumProject.Services; // Verifique se este é o namespace correto para o seu IEmailSender

var builder = WebApplication.CreateBuilder(args);

// Configuração de Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); // Nível 'Information' é suficiente para produção

// --- CONFIGURAÇÃO DO AppDbContext (Para os seus dados de negócio) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("A connection string 'DefaultConnection' não foi encontrada.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// --- CONFIGURAÇÃO DO DataProtectionKeysContext (Para as chaves de segurança) ---
builder.Services.AddDbContext<DataProtectionKeysContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// --- CONFIGURAÇÃO DO DATA PROTECTION ---
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>();

// Adicionar controladores e vistas
builder.Services.AddControllersWithViews();

// --- CONFIGURAÇÃO DA SESSÃO ---
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- CONFIGURAÇÃO DA AUTENTICAÇÃO (COM MAIS SEGURANÇA) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;

        // ADICIONADO: O cookie só é enviado em pedidos HTTPS. Essencial em produção.
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        // ADICIONADO: Política mais restritiva para proteção contra CSRF.
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Adicionar outros serviços
builder.Services.AddTransient<IEmailSender, EmailSender>();

// =========================================================================
// Construção da Aplicação
var app = builder.Build();
// =========================================================================

// --- APLICAR MIGRAÇÕES E SEED DATA NO ARRANQUE (A FORMA SEGURA) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("A iniciar processo de migração da base de dados...");

        // Migrar o contexto principal da aplicação
        var appDbContext = services.GetRequiredService<AppDbContext>();
        if (appDbContext.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Migrações pendentes encontradas para AppDbContext. A aplicar...");
            await appDbContext.Database.MigrateAsync();
            logger.LogInformation("Migrações do AppDbContext aplicadas com sucesso.");
        }
        else
        {
            logger.LogInformation("AppDbContext já está atualizado. Nenhuma migração a aplicar.");
        }

        // Popular dados iniciais (se necessário)
        await SeedData.Initialize(appDbContext);
        logger.LogInformation("SeedData verificado/inicializado.");


        // Migrar o contexto das chaves de segurança
        var dpContext = services.GetRequiredService<DataProtectionKeysContext>();
        if (dpContext.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Migrações pendentes encontradas para DataProtectionKeysContext. A aplicar...");
            await dpContext.Database.MigrateAsync();
            logger.LogInformation("Migrações do DataProtectionKeysContext aplicadas com sucesso.");
        }
        else
        {
            logger.LogInformation("DataProtectionKeysContext já está atualizado.");
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "ERRO FATAL: Falha durante a migração ou seeding da base de dados. A aplicação vai parar.");
    }
}
// --- FIM DA SECÇÃO DE MIGRAÇÃO ---

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();