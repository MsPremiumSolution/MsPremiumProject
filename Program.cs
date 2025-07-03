// Ficheiro: Program.cs (Vers�o Corrigida para a Sess�o)

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do DbContext (o teu c�digo, n�o mexi)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(5, 7, 25))
    )
);

// Resto dos teus servi�os
builder.Services.AddControllersWithViews();

// --- CONFIGURA��O DA SESS�O ---
// <<< ADICIONAR AQUI >>> PASSO 1: Diz � aplica��o ONDE guardar os dados da sess�o.
builder.Services.AddDistributedMemoryCache();

// <<< ADICIONAR AQUI >>> PASSO 2: Regista os servi�os da sess�o e define as op��es.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo que a sess�o fica ativa
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Importante para funcionar sempre
});
// --- FIM DA CONFIGURA��O DA SESS�O ---


// Configura��o da Autentica��o por Cookies (o teu c�digo, n�o mexi)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

// Resto dos teus servi�os (o teu c�digo, n�o mexi)
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddLogging();


// =========================================================================
// Constru��o da Aplica��o
var app = builder.Build();
// =========================================================================


// Pipeline de Pedidos HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// O app.UseSession() j� est� aqui, mas a ordem � importante.
// Vamos garantir que est� no s�tio certo.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// <<< ORDEM CORRETA >>> A Sess�o deve ser ativada ANTES da Autentica��o/Autoriza��o.
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();