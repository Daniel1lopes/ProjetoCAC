using Microsoft.AspNetCore.Mvc;
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
using OutroTeste.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.IdentityModel.Tokens;

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

                    var verificacaoSenha = _context.Pessoas.FirstOrDefault(x => x.coSenha == senha && x.edEmail == login.Usuario);

                    var colaboradorAdmin = _context.Pessoas
                        .Where(p => p.nmPessoa.ToUpper() == login.Usuario.ToUpper() && p.coSenha == senha)
                        .Join(_context.Colaboradores, p => p.idPessoa, c => c.idPessoa, (p, c) => c)
                        .Select(c => c.icAdministrador)
                        .FirstOrDefault();

                    Pessoa pessoaEmail = BuscarPorEmail(login.Usuario);

                    if (pessoaEmail != null)
                    {
                        if (verificacaoSenha != null)
                        {
                            if (colaboradorAdmin == true)
                            {
                                HttpContext.Session.SetString("IsAdmin", colaboradorAdmin.ToString());
                            }

                            HttpContext.Session.SetString("User", displayNomePessoa(login.Usuario));
                            
                            if (pessoaEmail != null) 
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
                    TempData["MensagemErro"] = "Eu acho que você está mentindo a sua idade...";

                    return View("CriarConta", pessoa);
                }

                if (String.IsNullOrEmpty(pessoa.coSenha))
                {
                    TempData["MensagemErro"] = "A senha não pode ser vazia.";

                    if (pessoa.coSenhaConfirmar != pessoa.coSenha)
                    {
                        TempData["MensagemErro"] = "A senha e a confirmação de senha não correspondem.";

                        return View("CriarConta", pessoa);
                    }

                    return View("CriarConta", pessoa);
                }

                var cpfExistente = _context.Pessoas.FirstOrDefault(p => p.nuCPF == pessoa.nuCPF);
                var emailExistente = _context.Pessoas.FirstOrDefault(p => p.edEmail == pessoa.edEmail);

                if (cpfExistente != null)
                {
                    TempData["MensagemErro"] = "Já existe um CPF com esse email.";

                    return View("CriarConta", pessoa);
                }

                if (emailExistente != null)
                {
                    TempData["MensagemErro"] = "Já existe uma conta com esse email.";

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
        public IActionResult EsquecerSenha()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RedifinirSenha(EsqueceuSenha esqueceuSenha)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Pessoa pessoa = _context.Pessoas.FirstOrDefault(x => x.edEmail.ToUpper() == esqueceuSenha.Email.ToUpper());

                    if (pessoa != null)
                    {
                        string novaSenha = pessoa.GerarNovaSenha();
                        string novaSenhaCriptografada = pessoa.SetNovaSenha(novaSenha);

                        pessoa.coSenha = novaSenhaCriptografada;

                        _context.Pessoas.Update(pessoa);
                        _context.SaveChanges();

                        TempData["MensagemSucesso"] = "Enviamos a senha com sucesso para o seu email registrado, se não encontrar, cheque seu spam.";
                        EnviarEmailSenha(pessoa.edEmail, novaSenha, pessoa.nmPessoa);
                        return RedirectToAction("EsquecerSenha");
                    }
                    TempData["MensagemErro"] = "Não consegumos refinir sua senha. Por favor, verifique os dados informados.";
                }
                return View("EsquecerSenha", esqueceuSenha);
            } catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos redefinir sua senha, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("EsquecerSenha");
            }   
        }
        // FUNÇÃO PARA ENVIAR EMAIL
        private void EnviarEmailSenha(string paraEmail, string novaSenha, string nomePessoa)
        {
            try
            {
                var fromAddress = new MailAddress("agendacac.ceub@gmail.com", "AgendaCAC"); 
                var toAddress = new MailAddress(paraEmail);
                const string fromPassword = "mrew xmmp kjtx qyzd"; 
                const string subject = "Sua nova senha - AgendaCAC";
                string body = $"Olá, {nomePessoa},\n\n Sua nova senha na AgendaCAC do CEUB é: {novaSenha}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", 
                    Port = 587, 
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception erro)
            {
                // Tratar a exceção 
            }
        }

        public IActionResult EditarUsuario()
        {
            var idPessoa = HttpContext.Session.GetInt32("idPessoa");
            Pessoa pessoa = _context.Pessoas.FirstOrDefault(p => p.idPessoa == idPessoa);

            if (pessoa == null)
            {
                TempData["MensagemErro"] = "Não foi encontrado o usuário.";
                return View();
            }
            
            var editarPessoa = new EditarPessoa
            {
                nmPessoa = pessoa.nmPessoa,
                nuCPF = pessoa.nuCPF,
                nuTelefone = pessoa.nuTelefone,
                edEmail = pessoa.edEmail,
                dtNascimento = pessoa.dtNascimento,
                idSexo = pessoa.idSexo
            };

            return View(editarPessoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UsuarioEditar(EditarPessoa editar)
        {
            var idPessoa = HttpContext.Session.GetInt32("idPessoa");

            Pessoa pessoa = _context.Pessoas.FirstOrDefault(p => p.idPessoa == idPessoa);

            var editarPessoaErro = new EditarPessoa
            {
                nmPessoa = pessoa.nmPessoa,
                nuCPF = pessoa.nuCPF,
                nuTelefone = pessoa.nuTelefone,
                edEmail = pessoa.edEmail,
                dtNascimento = pessoa.dtNascimento,
                idSexo = pessoa.idSexo
            };

            var userCadastrado = _context.Pessoas.FirstOrDefault(p => p.idPessoa == idPessoa);

            if (string.IsNullOrEmpty(editar.coSenha))
            {
                ModelState.Remove("coSenha");
            }
            if (string.IsNullOrEmpty(editar.coSenhaConfirmar))
            {
                ModelState.Remove("coSenhaConfirmar");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (editar.dtNascimento > DateTime.Now)
                    {
                        TempData["MensagemErro"] = "A data de nascimento não pode estar no futuro.";

                        return View("EditarUsuario", editarPessoaErro);
                    }

                    var idade = DateTime.Now.Year - editar.dtNascimento.Year;
                    if (idade > 100)
                    {
                        TempData["MensagemErro"] = "Eu acho que você está mentindo sua idade...";

                        return View("EditarUsuario", editarPessoaErro);
                    }

                    var cpfExistente = _context.Pessoas.FirstOrDefault(p => p.nuCPF == editar.nuCPF);
                    var emailExistente = _context.Pessoas.FirstOrDefault(p => p.edEmail == editar.edEmail);

                    if (cpfExistente != null)
                    {
                        ModelState.Remove("nuCPF");
                    }

                    if (emailExistente != null)
                    {
                        ModelState.Remove("edEmail");
                    }

                    if (!string.IsNullOrEmpty(editar.coSenha) && editar.coSenha == editar.coSenhaConfirmar)
                    {
                        pessoa.coSenha = pessoa.SetNovaSenha(editar.coSenha);
                        pessoa.coSenhaConfirmar = editar.coSenhaConfirmar; // Assuming you also want to check for null before setting this
                    }
                    else if (!string.IsNullOrEmpty(editar.coSenha) || !string.IsNullOrEmpty(editar.coSenhaConfirmar))
                    {
                        TempData["MensagemErro"] = "A senha e a confirmação de senha não correspondem.";
                        return View("EditarUsuario", editarPessoaErro);
                    }

                       pessoa.nmPessoa = editar.nmPessoa;
                       pessoa.nuTelefone = editar.nuTelefone;
                       pessoa.nuCPF = editar.nuCPF;
                       pessoa.idSexo = editar.idSexo;
                       pessoa.dtNascimento = editar.dtNascimento;
                       pessoa.edEmail = editar.edEmail;

                    _context.Pessoas.Update(pessoa);
                    _context.SaveChanges();

                        TempData["MensagemSucesso"] = "Usuário editado com sucesso!";
                        return RedirectToAction("EditarUsuario");              
                } 
                else
                {
                    var mensagemErro = "Não foi possível atualizar seu usuário, verifique os seguintes erros: ";
                    foreach (var modelStateKey in ViewData.ModelState.Keys)
                    {
                        var modelStateVal = ViewData.ModelState[modelStateKey];
                        foreach (var error in modelStateVal.Errors)
                        {
                            mensagemErro += $"{error.ErrorMessage} ";
                        }
                    }
                    TempData["MensagemErro"] = mensagemErro;
                    return RedirectToAction("EditarUsuario", editarPessoaErro);
                }
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos redefinir sua senha, tente novamente, detalhe do erro: {erro.Message}";
                return RedirectToAction("EditarUsuario");
            }
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
