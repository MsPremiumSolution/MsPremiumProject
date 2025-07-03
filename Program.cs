// Ficheiro: Program.cs (Versão Corrigida para a Sessão)

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext (o teu código, não mexi)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(5, 7, 25))
    )
);

// Resto dos teus serviços
builder.Services.AddControllersWithViews();

// --- CONFIGURAÇÃO DA SESSÃO ---
// <<< ADICIONAR AQUI >>> PASSO 1: Diz à aplicação ONDE guardar os dados da sessão.
builder.Services.AddDistributedMemoryCache();

// <<< ADICIONAR AQUI >>> PASSO 2: Regista os serviços da sessão e define as opções.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo que a sessão fica ativa
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Importante para funcionar sempre
});
// --- FIM DA CONFIGURAÇÃO DA SESSÃO ---


// Configuração da Autenticação por Cookies (o teu código, não mexi)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

// Resto dos teus serviços (o teu código, não mexi)
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddLogging();


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

// O app.UseSession() já está aqui, mas a ordem é importante.
// Vamos garantir que está no sítio certo.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// <<< ORDEM CORRETA >>> A Sessão deve ser ativada ANTES da Autenticação/Autorização.
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();