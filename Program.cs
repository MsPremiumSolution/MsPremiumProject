// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore; // Adicionar este using (se não estiver já)
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
// Pode adicionar .LogTo aqui também se quiser ver as queries de Data Protection
);

// --- CONFIGURAÇÃO DO DATA PROTECTION ---
// Persiste as chaves no DataProtectionKeysContext
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>(); // <<< USA O SEU DBContext DEDICADO AQUI


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
builder.Services.AddLogging(); // Já adicionado pelo template


// =========================================================================
// Construção da Aplicação
var app = builder.Build();
// =========================================================================

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