﻿using Microsoft.AspNetCore.Mvc;
using agenda.Models;
using System.Diagnostics;
using SQLitePCL;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using NuGet.Protocol.Plugins;
using System.Drawing.Text;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http;

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
            return View();
        }

        public Pessoa BuscarPorNome(string login)
        {
            return _context.Pessoas.FirstOrDefault(x => x.nmPessoa.ToUpper() == login.ToUpper());
        }

        public Pessoa BuscarPorEmail(string login)
        {
            return _context.Pessoas.FirstOrDefault(x => x.edEmail.ToUpper() == login.ToUpper());
        }

        public String displayNomePessoa(string Usuario)
        {
            if(Usuario.Contains("@"))
            {
                return _context.Pessoas.Where(p => p.edEmail == Usuario).Select(p => p.nmPessoa).FirstOrDefault();
            } 
            else
            {
                return _context.Pessoas.Where(x => x.nmPessoa.ToUpper() == Usuario.ToUpper()).Select(x => x.nmPessoa).FirstOrDefault();
            }
        }

        [HttpPost]
        public IActionResult Logar(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senha = login.Senha.GerarHash();

                    var verificacaoSenha = _context.Pessoas.FirstOrDefault(x => x.coSenha == senha);

                    var colaboradorAdmin = _context.Pessoas
                        .Where(p => p.nmPessoa.ToUpper() == login.Usuario.ToUpper() && p.coSenha == senha)
                        .Join(_context.Colaboradores, p => p.idPessoa, c => c.idPessoa, (p, c) => c)
                        .Select(c => c.icAdministrador)
                        .FirstOrDefault();


                    Pessoa pessoaNome = BuscarPorNome(login.Usuario);

                    Pessoa pessoaEmail = BuscarPorEmail(login.Usuario);

                    if (pessoaNome != null || pessoaEmail != null)
                    {
                        if (verificacaoSenha != null)
                        {
                            if (colaboradorAdmin == true)
                            {
                                HttpContext.Session.SetString("IsAdmin", colaboradorAdmin.ToString());
                            }

                            HttpContext.Session.SetString("User", displayNomePessoa(login.Usuario));

                            if (pessoaNome != null)
                            {
                                HttpContext.Session.SetInt32("idPessoa", pessoaNome.idPessoa);
                            }
                            else if (pessoaEmail != null) 
                            {
                                HttpContext.Session.SetInt32("idPessoa", pessoaEmail.idPessoa);
                            }

                            return RedirectToAction("Index", "CentroAtendimento");
                        }
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Usuário e/ou senha inválido(s). Por favor, tente novamente.";
                        return View("Login");
                    }
                }
                TempData["MensagemErro"] = "Por favor, preencha todos os campos.";
                return View("Login");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não foi possível realizar seu login, tente novamente. Erro: {erro.Message}";
                return View("Login");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "CentroAtendimento");
        }

        public IActionResult CriarConta()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Pessoa pessoa)
        {
            try
            {
                if (pessoa.dtNascimento > DateTime.Now)
                {
                    TempData["MensagemErro"] = "A data de nascimento não pode estar no futuro.";

                    return View("CriarConta", pessoa);
                }

                var idade = DateTime.Now.Year - pessoa.dtNascimento.Year;
                if (idade > 100)
                {
                    TempData["MensagemErro"] = "A pessoa não pode ter mais de 100 anos.";

                    return View("CriarConta", pessoa);
                }

                if (pessoa.coSenhaConfirmar == "")
                {
                    TempData["MensagemErro"] = "É necessário confirmar a senha.";

                    return View("CriarConta", pessoa);
                }

                if (pessoa.coSenhaConfirmar != pessoa.coSenha)
                {
                    TempData["MensagemErro"] = "A senha e a confirmação de senha não correspondem.";

                    return View("CriarConta", pessoa);
                }

                if (pessoa.idSexo != 1 && pessoa.idSexo != 2)
                {
                    TempData["MensagemErro"] = "Selecione pelo menos 1 sexo.";

                    return View("CriarConta", pessoa);
                }

                if (ModelState.IsValid)
                {
                    pessoa.SetSenhaHash();
                    _context.Pessoas.Add(pessoa);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso!";
                    return RedirectToAction("Login");
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
                TempData["MensagemErro"] = erro.Message;

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
            return View("EsquecerSenha", model);
        }

        public IActionResult Teste()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
