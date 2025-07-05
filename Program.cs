// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore; // Adicionar este using (se n�o estiver j�)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSPremiumProject.Data;
using MSPremiumProject.Services;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURA��O DO AppDbContext (Para os seus dados de neg�cio) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString) // Detec��o autom�tica da vers�o do MySQL
    )
    .LogTo(Console.WriteLine, LogLevel.Information) // Logging de queries SQL para depura��o
);

// --- CONFIGURA��O DO DataProtectionKeysContext (Para as chaves de seguran�a) ---
builder.Services.AddDbContext<DataProtectionKeysContext>(options =>
    options.UseMySql(
        connectionString, // Reutiliza a mesma string de conex�o
        ServerVersion.AutoDetect(connectionString)
    )
// Pode adicionar .LogTo aqui tamb�m se quiser ver as queries de Data Protection
);

// --- CONFIGURA��O DO DATA PROTECTION ---
// Persiste as chaves no DataProtectionKeysContext
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>(); // <<< USA O SEU DBContext DEDICADO AQUI


// Resto dos teus servi�os
builder.Services.AddControllersWithViews();

// --- CONFIGURA��O DA SESS�O ---
builder.Services.AddDistributedMemoryCache(); // ATEN��O: Sess�es em mem�ria n�o s�o persistentes entre restarts/inst�ncias
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

// Resto dos teus servi�os
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddLogging(); // J� adicionado pelo template


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