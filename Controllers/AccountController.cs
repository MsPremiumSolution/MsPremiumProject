using Microsoft.AspNetCore.Mvc;
using MSPremiumProject.Data; // Seu AppDbContext
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Models; // Seus modelos Utilizador, PasswordResetToken, Role
using MSPremiumProject.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Cryptography; // Para gerar tokens de reset
using MSPremiumProject.Services;   // Para IEmailSender
using Microsoft.Extensions.Logging; // Para ILogger

namespace MSPremiumProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context; // interage com a base de dados 
        private readonly IEmailSender _emailSender; // para enviar emails
        private readonly ILogger<AccountController> _logger; // registar logs

        // Adicione esta configuração para facilitar a alteração do IP/Host para teste
        private const string DevelopmentHostOverride = "192.168.1.155:5042"; // ip de casa
       

        public AccountController(AppDbContext context, IEmailSender emailSender, ILogger<AccountController> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }



        // GET: /Account/Login
        // ... (código existente) ...
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Account/Login.cshtml", new LoginViewModel());
        }

        // POST: /Account/Login
        // ... (código existente) ...
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Utilizadores
                                   .Include(u => u.Role)
                                   .FirstOrDefaultAsync(u => u.Login.ToLower() == model.Login.ToLower());

                if (user != null)
                {
                    if (!user.Activo)
                    {
                        ModelState.AddModelError(string.Empty, "Esta conta de utilizador está desativada.");
                        return View("~/Views/Account/Login.cshtml", model);
                    }

                    if (BCrypt.Net.BCrypt.Verify(model.Password, user.Pwp))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UtilizadorId.ToString()),
                            new Claim(ClaimTypes.Name, user.Login),
                            new Claim("FullName", user.Nome ?? string.Empty),
                            new Claim(ClaimTypes.Role, user.Role?.Nome ?? "DefaultRole")
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            AllowRefresh = true,
                            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddMinutes(60)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        _logger.LogInformation($"Utilizador {user.Login} logado com sucesso.");

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Tentativa de login falhada para {model.Login}: Password incorreta.");
                        ModelState.AddModelError(string.Empty, "Login ou password inválidos.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Tentativa de login falhada: Utilizador {model.Login} não encontrado.");
                    ModelState.AddModelError(string.Empty, "Login ou password inválidos.");
                }
            }
            else
            {
                _logger.LogWarning("Tentativa de login falhada: ModelState inválido.");
            }
            return View("~/Views/Account/Login.cshtml", model);
        }


        // POST: /Account/Logout
        // ... (código existente) ...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Utilizador deslogado.");
            HttpContext.Session.Clear(); // Limpa a sessão também
            return RedirectToAction(nameof(Login), "Account");
        }

        // GET: /Account/AccessDenied
        // ... (código existente) ...
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View("~/Views/Account/AccessDenied.cshtml");
        }

        // GET: /Account/ForgotPassword
        // ... (código existente) ...
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View("~/Views/Account/ForgotPassword.cshtml", new ForgotPasswordViewModel());
        }


        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.Login.ToLower() == model.Login.ToLower()); // Assumindo que Login é o email

                // << ALTERAÇÃO 1: Tratar o caso de o utilizador NÃO existir primeiro >>
                if (user == null)
                {
                    // Log do evento
                    _logger.LogWarning($"Tentativa de redefinição de password para login (email) não encontrado: {model.Login}");

                    // Adiciona um erro ao ModelState que será exibido na view
                    ModelState.AddModelError(string.Empty, "O login (email) fornecido não foi encontrado.");

                    // Retorna para a mesma view, que agora exibirá o erro
                    return View("~/Views/Account/ForgotPassword.cshtml", model);
                }

                // Se chegámos aqui, o utilizador EXISTE. Agora fazemos o resto.

                // (Opcional) Invalida tokens de reset anteriores para este utilizador.
                var existingTokens = _context.PasswordResetTokens
                                       .Where(t => t.UtilizadorId == user.UtilizadorId && !t.IsUsed && t.ExpirationDate > DateTime.UtcNow);
                foreach (var oldToken in existingTokens)
                {
                    oldToken.IsUsed = true;
                }

                // Gera um token de redefinição seguro e aleatório
                var tokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                var expirationDate = DateTime.UtcNow.AddHours(1);

                // Cria um novo registo de token de redefinição.
                var passwordResetToken = new PasswordResetToken
                {
                    UtilizadorId = user.UtilizadorId,
                    TokenValue = tokenValue,
                    ExpirationDate = expirationDate,
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(passwordResetToken);
                await _context.SaveChangesAsync();

                // Lógica para construir o URL de callback
                string callbackUrl;
                if (Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || Request.Host.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(DevelopmentHostOverride))
                    {
                        var pathAndQuery = Url.Action("ResetPassword", "Account", new { userId = user.UtilizadorId.ToString(), token = tokenValue });
                        callbackUrl = $"http://{DevelopmentHostOverride}{pathAndQuery}";
                    }
                    else
                    {
                        callbackUrl = Url.Action("ResetPassword", "Account",
                            new { userId = user.UtilizadorId.ToString(), token = tokenValue }, protocol: "http");
                    }
                }
                else
                {
                    callbackUrl = Url.Action("ResetPassword", "Account",
                        new { userId = user.UtilizadorId.ToString(), token = tokenValue }, protocol: Request.Scheme);
                }
                _logger.LogInformation($"Callback URL gerado: {callbackUrl}");

                try
                {
                    await _emailSender.SendEmailAsync(
                       user.Login,
                       "Redefinir Password - MSPremiumProject",
                       $"Olá {user.Nome ?? "utilizador"},<br/><br/>Para redefinir a sua password, por favor clique no link abaixo:<br/><a href='{callbackUrl}'>Redefinir Password</a><br/><br/>Se não solicitou esta alteração, pode ignorar este email.<br/><br/>Obrigado,<br/>Equipa MSPremiumProject");
                    _logger.LogInformation($"Email de redefinição de password enviado para {user.Login}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Falha ao enviar email de redefinição para {user.Login}. Detalhes: {ex.Message}");
                    // Mesmo que o email falhe, por segurança, não informe o utilizador do erro interno.
                    // A página de confirmação genérica ainda é a melhor abordagem aqui.
                }

                // << ALTERAÇÃO 2: A view de confirmação só é retornada em caso de SUCESSO >>
                return View("~/Views/Account/ForgotPasswordConfirmation.cshtml");
            }

            // Se o modelo não for válido (ex: campo email vazio), retorna para a view com os erros de validação
            return View("~/Views/Account/ForgotPassword.cshtml", model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        // ... (código existente) ...
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View("~/Views/Account/ForgotPasswordConfirmation.cshtml");
        }


        // GET: /Account/ResetPassword
        // ... (código existente) ...
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Tentativa de ResetPassword com userId ou token em falta.");
                TempData["ErrorMessage"] = "Link de redefinição inválido ou incompleto.";
                return RedirectToAction(nameof(Login));
            }

            if (!ulong.TryParse(userId, out ulong parsedUserId))
            {
                _logger.LogWarning($"Tentativa de ResetPassword com userId inválido: {userId}");
                TempData["ErrorMessage"] = "Link de redefinição inválido.";
                return RedirectToAction(nameof(Login));
            }

            var storedToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.UtilizadorId == parsedUserId &&
                                          t.TokenValue == token &&
                                          !t.IsUsed &&
                                          t.ExpirationDate > DateTime.UtcNow);

            if (storedToken == null)
            {
                _logger.LogWarning($"Token de ResetPassword inválido ou expirado para userId: {userId}, token: {token}");
                TempData["ErrorMessage"] = "O link de redefinição de password é inválido ou expirou. Por favor, tente novamente.";
                return RedirectToAction("ForgotPassword");
            }

            var model = new ResetPasswordViewModel { UserId = userId, Token = token };
            return View("~/Views/Account/ResetPassword.cshtml", model);
        }


        // POST: /Account/ResetPassword
        // ... (código existente) ...
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Account/ResetPassword.cshtml", model);
            }

            if (!ulong.TryParse(model.UserId, out ulong parsedUserId))
            {
                ModelState.AddModelError(string.Empty, "Erro ao processar o seu pedido. ID de utilizador inválido.");
                return View("~/Views/Account/ResetPassword.cshtml", model);
            }

            var user = await _context.Utilizadores.FindAsync(parsedUserId);
            if (user == null)
            {
                _logger.LogError($"Utilizador não encontrado durante ResetPassword (POST). UserId: {model.UserId}");
                ModelState.AddModelError(string.Empty, "Utilizador não encontrado. O link pode ser inválido.");
                return View("~/Views/Account/ResetPassword.cshtml", model);
            }

            var storedToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.UtilizadorId == parsedUserId &&
                                          t.TokenValue == model.Token &&
                                          !t.IsUsed &&
                                          t.ExpirationDate > DateTime.UtcNow);
            // verifica novamente a validade do token
            if (storedToken == null)
            {
                _logger.LogWarning($"Token de ResetPassword inválido ou expirado (POST) para userId: {model.UserId}, token: {model.Token}");
                ModelState.AddModelError(string.Empty, "O link de redefinição de password é inválido ou expirou. Por favor, tente novamente.");
                return View("~/Views/Account/ResetPassword.cshtml", model);
            }
            // hash da nova password,guarda na bd a nova password
            user.Pwp = BCrypt.Net.BCrypt.HashPassword(model.Password);
            storedToken.IsUsed = true;

            _context.Update(user);
            _context.Update(storedToken);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Password redefinida com sucesso para o utilizador ID: {user.UtilizadorId}");
                TempData["SuccessMessage"] = "A sua password foi redefinida com sucesso. Pode agora fazer login.";
                return RedirectToAction(nameof(Login));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Erro de concorrência ao atualizar password para utilizador ID: {user.UtilizadorId}");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao tentar guardar as alterações devido a um conflito de dados. Tente novamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao guardar alterações de password para utilizador ID: {user.UtilizadorId}");
                ModelState.AddModelError(string.Empty, "Ocorreu um erro inesperado ao tentar redefinir a sua password. Tente novamente.");
            }

            return View("~/Views/Account/ResetPassword.cshtml", model);
        }

    }
}