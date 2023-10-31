using Microsoft.AspNetCore.Mvc;
using agenda.Models;
using System.Diagnostics;
using SQLitePCL;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using OutroTeste.Models;

namespace agenda.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            var mensagemSucesso = TempData["MensagemSucesso"] as string;
            if (!string.IsNullOrEmpty(mensagemSucesso))
            {
                ViewData["MensagemSucesso"] = mensagemSucesso;
            }

            return View();
        }

        public IActionResult CriarConta()
        {
            ViewBag.Sexos = _context.Sexos.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Pessoa pessoa)
        {
            try
            {
                if (pessoa.coSenhaConfirmar != pessoa.coSenha)
                {
                    TempData["MensagemErro"] = "A senha e a confirmação de senha não correspondem.";

                    return View("CriarConta", pessoa);
                }
                else if (pessoa.idSexo == 1 || pessoa.idSexo == 2) 
                {
                    if (ModelState.IsValid)
                    {

                        pessoa.SetSenhaHash();
                        _context.Pessoas.Add(pessoa);
                        _context.SaveChanges();

                        TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                        return RedirectToAction("Login");
                    }
                    TempData["MensagemErro"] = "Selecione pelo menos 1 sexo.";

                    return View("CriarConta", pessoa);
                }

                else
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();

                    foreach (var error in errors)
                    {
                        TempData["MensagemErro"] = (error);
                    }
                    return View("CriarConta", pessoa);
                }
            }

            catch (Exception erro)
            {
                TempData["MensagemErro"] = (erro);

                return RedirectToAction("CriarConta");
            }
        }
        [AllowAnonymous, HttpGet("esqueceu-senha")]
        public IActionResult EsquecerSenha()
        {
            return View();
        }

        [AllowAnonymous, HttpPost("esqueceu-senha")]
        public IActionResult EsquecerSenha(EsqueceuSenha model)
        {
            if (ModelState.IsValid)
            {

                ModelState.Clear();
                model.EmailMandado = true;
            }
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
