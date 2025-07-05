// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DO AppDbContext (Para os seus dados de negócio) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString) // Detecção automática da versão do MySQL
    )
    .LogTo(Console.WriteLine, LogLevel.Information) // Logging de queries SQL para depuração
);

// --- CONFIGURAÇÃO DO DataProtectionKeysContext (Para as chaves de segurança) ---
builder.Services.AddDbContext<DataProtectionKeysContext>(options =>
    options.UseMySql(
        connectionString, // Reutiliza a mesma string de conexão
        ServerVersion.AutoDetect(connectionString)
    )
// Pode adicionar .LogTo(Console.WriteLine, LogLevel.Information) aqui também para ver as queries de Data Protection
);

// --- CONFIGURAÇÃO DO DATA PROTECTION ---
// Persiste as chaves no DataProtectionKeysContext
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>(); // Usa o DbContext dedicado


// Resto dos teus serviços
builder.Services.AddControllersWithViews();

// --- CONFIGURAÇÃO DA SESSÃO ---
builder.Services.AddDistributedMemoryCache(); // ATENÇÃO: Sessões em memória não são persistentes entre restarts/instâncias
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

// Resto dos teus serviços
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddLogging();


// =========================================================================
// Construção da Aplicação
var app = builder.Build();
// =========================================================================

// --- APLICAR MIGRATIONS E SEED DATA NO ARRANQUE ---
// ESTE BLOCO É CRUCIAL E DEVE SER EXECUTADO ANTES DE app.Run();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        // Aplica migrações para AppDbContext (seus dados de negócio)
        var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        appDbContext.Database.Migrate();
        // Opcional: Seeding inicial de dados (se você tem o SeedData.cs)
        await SeedData.Initialize(appDbContext);

        // Aplica migrações para DataProtectionKeysContext (chaves de segurança)
        var dpContext = serviceProvider.GetRequiredService<DataProtectionKeysContext>();
        dpContext.Database.Migrate();

        app.Logger.LogInformation("Database migrations and seeding applied successfully.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred during database migration/seeding. Application startup might fail.");
        // Em um ambiente de produção, é comum re-throw a exceção aqui
        // ou parar a aplicação para que o ambiente (Render.com) perceba a falha.
        // throw; // Se quiser que o startup falhe se a migração/seed falhar
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