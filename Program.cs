// Ficheiro: Program.cs

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
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
// Pode adicionar .LogTo(Console.WriteLine, LogLevel.Information) aqui tamb�m para ver as queries de Data Protection
);

// --- CONFIGURA��O DO DATA PROTECTION ---
// Persiste as chaves no DataProtectionKeysContext
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionKeysContext>(); // Usa o DbContext dedicado


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
builder.Services.AddLogging();


// =========================================================================
// Constru��o da Aplica��o
var app = builder.Build();
// =========================================================================

// --- APLICAR MIGRATIONS E SEED DATA NO ARRANQUE ---
// ESTE BLOCO � CRUCIAL E DEVE SER EXECUTADO ANTES DE app.Run();
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        // Aplica migra��es para AppDbContext (seus dados de neg�cio)
        var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        appDbContext.Database.Migrate();
        // Opcional: Seeding inicial de dados (se voc� tem o SeedData.cs)
        await SeedData.Initialize(appDbContext);

        // Aplica migra��es para DataProtectionKeysContext (chaves de seguran�a)
        var dpContext = serviceProvider.GetRequiredService<DataProtectionKeysContext>();
        dpContext.Database.Migrate();

        app.Logger.LogInformation("Database migrations and seeding applied successfully.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred during database migration/seeding. Application startup might fail.");
        // Em um ambiente de produ��o, � comum re-throw a exce��o aqui
        // ou parar a aplica��o para que o ambiente (Render.com) perceba a falha.
        // throw; // Se quiser que o startup falhe se a migra��o/seed falhar
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