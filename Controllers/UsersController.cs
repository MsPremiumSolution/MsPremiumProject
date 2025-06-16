using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MSPremiumProject.Data;
using MSPremiumProject.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net; // Para hashing de password

namespace MSPremiumProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users/Index
        // Lista todos os utilizadores.
        public async Task<IActionResult> Index()
        {
            var utilizadores = await _context.Utilizadores
                                        .Include(u => u.Role)
                                        // .Where(u => u.Activo) // Descomente para mostrar apenas ativos por padrão na lista
                                        .ToListAsync();
            return View("~/Views/Admin/UsersList.cshtml", utilizadores);
        }

        // Action auxiliar opcional para navegação
        public IActionResult CreateUser()
        {
            return RedirectToAction(nameof(Create));
        }

        // GET: Users/Create
        // Mostra o formulário para criar um novo utilizador
        [HttpGet]
        public IActionResult Create()
        {
            if (TempData.ContainsKey("Mensagem"))
            {
                ViewBag.Mensagem = TempData["Mensagem"]?.ToString();
            }
            if (TempData.ContainsKey("MensagemErro"))
            {
                ViewBag.MensagemErro = TempData["MensagemErro"]?.ToString();
            }

            ViewBag.Roles = new SelectList(_context.Roles.ToList(), "RoleId", "Nome");
            return View("~/Views/Admin/CreateUser.cshtml", new Utilizador());
        }

        // POST: Users/Create
        // Processa os dados submetidos do formulário de criação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Utilizador utilizador)
        {
            Console.WriteLine("POST Create action foi chamada.");

            if (utilizador.RoleId > 0 && ModelState.ContainsKey(nameof(Utilizador.Role)))
            {
                ModelState.Remove(nameof(Utilizador.Role));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(utilizador.Pwp))
                    {
                        utilizador.Pwp = BCrypt.Net.BCrypt.HashPassword(utilizador.Pwp);
                    }
                    else
                    {
                        ModelState.AddModelError("Pwp", "A password é obrigatória.");
                        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "RoleId", "Nome", utilizador.RoleId);
                        return View("~/Views/Admin/CreateUser.cshtml", utilizador);
                    }

                    bool loginExists = await _context.Utilizadores.AnyAsync(u => u.Login == utilizador.Login);
                    if (loginExists)
                    {
                        ModelState.AddModelError("Login", "Este login já está em uso.");
                        ViewBag.Roles = new SelectList(_context.Roles.ToList(), "RoleId", "Nome", utilizador.RoleId);
                        return View("~/Views/Admin/CreateUser.cshtml", utilizador);
                    }

                    utilizador.Activo = true; // Novo utilizador é ativo por padrão
                    _context.Utilizadores.Add(utilizador);
                    await _context.SaveChangesAsync();

                    TempData["Mensagem"] = "Utilizador criado com sucesso!";
                    return RedirectToAction(nameof(Index)); // Redireciona para a lista
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Erro ao salvar na BD: {ex.InnerException?.Message ?? ex.Message}");
                    ModelState.AddModelError("", "Ocorreu um erro ao gravar os dados.");
                    TempData["MensagemErro"] = "Ocorreu um erro ao tentar criar o utilizador na base de dados.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                    ModelState.AddModelError("", "Ocorreu um erro inesperado.");
                    TempData["MensagemErro"] = "Ocorreu um erro inesperado ao processar o seu pedido.";
                }
            }

            Console.WriteLine("ModelState não é válido ou ocorreu um erro (POST Create).");
            ViewBag.Roles = new SelectList(_context.Roles.ToList(), "RoleId", "Nome", utilizador.RoleId);
            return View("~/Views/Admin/CreateUser.cshtml", utilizador);
        }

        // POST: Users/ToggleUserStatus/5
        // Alterna o estado 'Activo' do utilizador (Soft Delete/Reactivate)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(ulong id)
        {
            if (id == 0)
            {
                TempData["MensagemErro"] = "ID de utilizador inválido.";
                return RedirectToAction(nameof(Index));
            }

            var utilizador = await _context.Utilizadores.FindAsync(id);

            if (utilizador == null)
            {
                TempData["MensagemErro"] = "Utilizador não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                utilizador.Activo = !utilizador.Activo; // Alterna o estado atual
                _context.Update(utilizador); // Marca a entidade como modificada
                await _context.SaveChangesAsync(); // Salva a alteração

                string acao = utilizador.Activo ? "ativado" : "desativado";
                TempData["Mensagem"] = $"Utilizador {acao} com sucesso!";
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Erro ao alterar estado do utilizador (DbUpdateException): {ex.InnerException?.Message ?? ex.Message}");
                TempData["MensagemErro"] = "Não foi possível alterar o estado do utilizador.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao alterar estado do utilizador: {ex.Message}");
                TempData["MensagemErro"] = "Ocorreu um erro inesperado.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Comentários para a funcionalidade de Editar, caso queira adicionar no futuro
        /*
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(ulong id)
        {
            if (id == 0) return NotFound();
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null) return NotFound();
            ViewBag.Roles = new SelectList(_context.Role.ToList(), "RoleId", "Nome", utilizador.RoleId);
            return View("~/Views/Admin/EditUser.cshtml", utilizador); // Crie esta view
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ulong id, Utilizador utilizador)
        {
            if (id != utilizador.UtilizadorId) return BadRequest();

            if (utilizador.RoleId > 0 && ModelState.ContainsKey(nameof(Utilizador.Role)))
            {
                ModelState.Remove(nameof(Utilizador.Role));
            }
            
            // Lógica para tratar a password na edição:
            // Se Pwp vier vazia do formulário, significa que o utilizador não quer mudar a password.
            // Nesse caso, precisamos buscar a password antiga (hasheada) e mantê-la.
            // Se Pwp vier preenchida, então fazemos o hash da nova password.
            var utilizadorOriginal = await _context.Utilizadores.AsNoTracking().FirstOrDefaultAsync(u => u.UtilizadorId == id);
            if(utilizadorOriginal == null) return NotFound();

            if (string.IsNullOrEmpty(utilizador.Pwp)) // Password não foi alterada no formulário
            {
                utilizador.Pwp = utilizadorOriginal.Pwp; // Mantém a password antiga (já hasheada)
                // Remove a validação de Pwp se ela não for obrigatória na edição
                if (ModelState.ContainsKey(nameof(Utilizador.Pwp)))
                {
                     ModelState.Remove(nameof(Utilizador.Pwp)); // Para não dar erro de [Required] se Pwp não for mudada
                }
            }
            else // Password foi alterada no formulário
            {
                utilizador.Pwp = BCrypt.Net.BCrypt.HashPassword(utilizador.Pwp);
            }


            if (ModelState.IsValid) // Verifica ModelState APÓS tratar a password
            {
                try
                {
                    _context.Update(utilizador);
                    await _context.SaveChangesAsync();
                    TempData["Mensagem"] = "Utilizador atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(utilizador.UtilizadorId)) return NotFound();
                    else throw;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Erro ao atualizar utilizador: {ex.InnerException?.Message ?? ex.Message}");
                    ModelState.AddModelError("", "Ocorreu um erro ao atualizar os dados.");
                }
            }
            ViewBag.Roles = new SelectList(_context.Role.ToList(), "RoleId", "Nome", utilizador.RoleId);
            return View("~/Views/Admin/EditUser.cshtml", utilizador);
        }

        private bool UtilizadorExists(ulong id)
        {
            return _context.Utilizadores.Any(e => e.UtilizadorId == id);
        }
        */
    }
}