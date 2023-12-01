using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using agenda.Models;
using System.Runtime.Intrinsics.X86;
using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using System.Collections;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace agenda.Controllers
{
    public class CentroAtendimentoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CentroAtendimentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var centroAtendimentos = _context.CentroAtendimentos.ToList();
            ViewData["Title"] = "AgendaCAC - Centro de Atendimento";
            return View(centroAtendimentos);
        }

        [Route("CentroAtendimento/Especialidades/{centroAtendimentoID}")]
        public IActionResult Especialidades([FromRoute] short centroAtendimentoID)
        {
            ViewData["Title"] = "AgendaCAC - Especialidades";
            var centroAtendimento = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == centroAtendimentoID);
            if (centroAtendimento == null)
            {
                return NotFound();
            }
            var especialidades = _context.Especialidades
            .Where(e => e.idCentroAtendimento == centroAtendimentoID)
            .ToList();

            ViewBag.CentroAtendimentoNome = centroAtendimento.nmCentroAtendimento;
            return View(especialidades);
        }

        [Route("CentroAtendimento/Servicos/{EspecialidadeID}")]
        public IActionResult Servicos([FromRoute] int EspecialidadeID)
        {
            ViewData["Title"] = "AgendaCAC - Serviços";
            var especialidade = _context.Especialidades
            .Include(e => e.Servico)
            .FirstOrDefault(e => e.idEspecialidade == EspecialidadeID);
            if (especialidade == null)
            {
                return NotFound();
            }

            ViewBag.EspecialidadeNome = especialidade.nmEspecialidade;
            ViewBag.CentroAtendimentoNome = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == especialidade.idCentroAtendimento)?.nmCentroAtendimento;
            ViewBag.centroAtendimentoID = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == especialidade.idCentroAtendimento)?.idCentroAtendimento;
            return View(especialidade.Servico.ToList());
        }

        [Route("CentroAtendimento/UnidadesAtendimento/{centroAtendimentoID}/{servicoID}")]
        public IActionResult UnidadesAtendimento([FromRoute] short centroAtendimentoID, [FromRoute] short servicoID)
        {
            ViewData["Title"] = "AgendaCAC - Unidade de Atendimento";
            var servico = _context.Servicos.FirstOrDefault(s => s.idServico == servicoID);
            if (servico == null)
            {
                return NotFound();
            }
            var centroAtendimento = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == centroAtendimentoID);
            if (centroAtendimento == null)
            {
                return NotFound();
            }
            var unidadesAtendimento = _context.ServicosUnidadeAtendimento
                .Where(sua => sua.idServico == servicoID)
                .Select(sua => sua.UnidadeAtendimento)
                .ToList();

            ViewBag.CentroAtendimentoID = centroAtendimento.idCentroAtendimento;
            ViewBag.ServicoID = servico.idServico;
            ViewBag.EspecialidadeID = _context.Especialidades.FirstOrDefault(e => e.idEspecialidade == servico.idEspecialidade)?.idEspecialidade;

            ViewBag.CentroAtendimentoNome = centroAtendimento.nmCentroAtendimento;
            ViewBag.ServicoNome = servico.nmServico;
            ViewBag.EspecialidadeNome = _context.Especialidades.FirstOrDefault(e => e.idEspecialidade == servico.idEspecialidade)?.nmEspecialidade;

            var servicoUnidadeAtendimentoIDs = unidadesAtendimento.ToDictionary(
                unidade => unidade.idUnidadeAtendimento,
                unidade => _context.ServicosUnidadeAtendimento
                .FirstOrDefault(e => e.idServico == servicoID && e.idUnidadeAtendimento == unidade.idUnidadeAtendimento)
                ?.idServicoUnidadeAtendimento);

            ViewBag.ServicoUnidadeAtendimentoIDs = servicoUnidadeAtendimentoIDs;

            ViewBag.CentroAtendimentoID = centroAtendimento.idCentroAtendimento;
            return View(unidadesAtendimento);
        }

        private bool LimitePorUser(int? idPessoa, int idAgenda)
        {
            if (!idPessoa.HasValue)
            {
                return false;
            }

            bool jaAgendou = _context.Agendamentos
                .Any(a => a.idPessoa == idPessoa.Value && a.idAgenda == idAgenda);

            return !jaAgendou;
        }

        [Route("CentroAtendimento/{centroAtendimentoID}/{especialidadeID}/{servicoUnidadeAtendimentoID}/DatasDisponiveis/")]
        public IActionResult DatasDisponiveis([FromRoute] short servicoUnidadeAtendimentoID, [FromRoute] short centroAtendimentoID, [FromRoute] short especialidadeID, short unidadeID)
        {

            ViewData["Title"] = "AgendaCAC - Datas Disponíveis";

            var AgendaSelecionada = _context.Agendas
                .FromSqlInterpolated($@"
             SELECT DISTINCT A.*
            FROM CACBD.CACTB.Agenda A
            INNER JOIN CACBD.CACTB.ServicoUnidadeAtendimento SUA
              ON SUA.idServicoUnidadeAtendimento = A.idServicoUnidadeAtendimento
            LEFT JOIN (
            SELECT idAgenda, COUNT(1) AS nuQtdeAgendamento
            FROM CACBD.CACTB.Agendamento
            GROUP BY idAgenda
            ) AS QA ON QA.idAgenda = A.idAgenda
            WHERE A.idServicoUnidadeAtendimento = {servicoUnidadeAtendimentoID}
            AND (A.nuVagas - A.nuReserva - ISNULL(QA.nuQtdeAgendamento,0)) > 0")
                .ToList();

            var unidadeNome = _context.UnidadesAtendimento
                .Where(ua => ua.idUnidadeAtendimento == unidadeID)
                .Select(ua => ua.nmUnidadeAtendimento)
                .FirstOrDefault();

            ViewBag.unidadeNome = unidadeNome;

            var servico = _context.ServicosUnidadeAtendimento
                .Where(sua => sua.idServicoUnidadeAtendimento == servicoUnidadeAtendimentoID)
                .Join(_context.Servicos,
                      sua => sua.idServico,
                      s => s.idServico,
                      (sua, s) => new { s.nmServico })
                .Select(x => x.nmServico)
                .FirstOrDefault();

            ViewBag.servicoNome = servico;

            var centroAtendimentoNome = _context.CentroAtendimentos.FirstOrDefault(CA => CA.idCentroAtendimento == centroAtendimentoID)?.nmCentroAtendimento;
            ViewBag.centroAtendimentoNome = centroAtendimentoNome;

            var especialidadeNome = _context.Especialidades.FirstOrDefault(E => E.idEspecialidade == especialidadeID)?.nmEspecialidade;
            ViewBag.especialidadeNome = especialidadeNome;

            var disponibilidadeAgenda = _context.DisponibilidadeAgendas
                .Where(da => da.idServicoUnidadeAtendimento == servicoUnidadeAtendimentoID && da.nuQtdeDisponivel > 0)
                .Select(sua => sua.nuQtdeDisponivel)
                .ToList();

            ViewBag.disponibilidadeAgenda = disponibilidadeAgenda;

            var idPessoa = HttpContext.Session.GetInt32("idPessoa");

            var limitePorEspecialidade = _context.AgendamentoFulls.Any(ag => ag.idPessoa == idPessoa
                                                                         && ag.idEspecialidade == especialidadeID
                                                                         && ag.icAtivoAgendamento == true);

            ViewBag.limitePorEspecialidade = limitePorEspecialidade;

            return View("DatasDisponiveis", AgendaSelecionada);
        }

        public IActionResult HorariosMarcados()
        {
            ViewData["Title"] = "AgendaCAC - Horários Marcados";

            var idPessoaHorario = HttpContext.Session.GetInt32("idPessoa");

            var PessoaImprimir = _context.Pessoas.Where(p => p.idPessoa == idPessoaHorario).Select(p => new { p.nmPessoa, p.nuTelefone }).FirstOrDefault();
            ViewBag.PessoaImprimir = PessoaImprimir;

            var HorarioAgendado = _context.AgendamentoFulls.Where(ag => ag.idPessoa == idPessoaHorario && ag.icAtivoAgendamento == true).ToList();

            ViewBag.HorarioAgendado = HorarioAgendado;

            return View();
        }

        [HttpPost]
        public IActionResult MarcacaoDeHorario(int idAgenda, int idPessoa, Agendamento agendamento)
        {
            try
            {
                var agenda = _context.Agendas.Find(idAgenda);
                var idPessoaSessao = HttpContext.Session.GetInt32("idPessoa");

                if (agenda != null && idPessoaSessao != null)
                {
                    var qtdeDisponivel = _context.DisponibilidadeAgendas.FirstOrDefault(da => da.idAgenda == idAgenda);
                    if (qtdeDisponivel.nuQtdeDisponivel - 1 >= 0)
                    {
                        agendamento.Agenda = agenda;

                        agendamento = new Agendamento
                        {
                            dtAgendamento = DateTime.Now,
                            icAtivo = true,
                            idAgenda = idAgenda,
                            idPessoa = (int)idPessoaSessao
                        };

                        _context.Agendamentos.Add(agendamento);

                        _context.SaveChanges();

                        TempData["MensagemSucesso"] = "Parabéns, sua consulta foi marcada com sucesso !";
                        return RedirectToAction("HorariosMarcados");
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Ops, sua vaga foi preenchida.";
                    }
                }

            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos apagar seu usuário, tente novamante, detalhe do erro: {erro.Message}";
            }

            return View();
        }

        [HttpPost]
        public IActionResult CancelarHorario(int idAgendamento)
        {
            var agendamento = _context.Agendamentos.FirstOrDefault(ag => ag.idAgendamento == idAgendamento);

            if (agendamento != null)
            {
                agendamento.icAtivo = false;
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Consulta desmarcada com sucesso !";
            }

            return RedirectToAction("HorariosMarcados");
        }

        [HttpPost]
        public IActionResult CancelarHorarioADM(int idAgendamento)
        {
            var agendamento = _context.Agendamentos.FirstOrDefault(ag => ag.idAgendamento == idAgendamento);

            if (agendamento != null)
            {
                agendamento.icAtivo = false;
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Consulta desmarcada com sucesso !";
            }

            return RedirectToAction("HorarioAdministrador");
        }

        public IActionResult HorarioAdministrador()
        {
            ViewData["Title"] = "AgendaCAC - Horários (ADM)";

            var CentroAtendimento = _context.CentroAtendimentos.Select(ca => new { ca.nmCentroAtendimento, ca.idCentroAtendimento }).ToList();
            ViewBag.CentroAtendimento = CentroAtendimento;

            var Especialidade = _context.Especialidades.Select(e => new { e.nmEspecialidade, e.idEspecialidade }).ToList();
            ViewBag.Especialidade = Especialidade;

            var Servico = _context.Servicos.Select(s => new { s.nmServico, s.idServico }).ToList();
            ViewBag.Servico = Servico;

            var nomeUser = _context.Pessoas.Select(nu => nu.nmPessoa).ToList();

            var HorarioAdministrador = _context.AgendamentoFulls.Where(af => af.icAtivoAgendamento == true).Join(_context.Pessoas, af => af.idPessoa, p => p.idPessoa,
                                                                      (af, p) => new
                                                                      {
                                                                          af.dtAgenda,
                                                                          af.hrInicio,
                                                                          af.hrFim,
                                                                          af.nmCentroAtendimento,
                                                                          af.nmEspecialidade,
                                                                          af.nmServico,
                                                                          af.idAgendamento,
                                                                          af.nmUnidadeAtendimento,
                                                                          p.nmPessoa,
                                                                          p.edEmail,
                                                                          p.nuTelefone
                                                                      }).ToList();

            ViewBag.HorarioAdministrador = HorarioAdministrador;

            return View();
        }

        [HttpGet]
        public JsonResult GetEspecialidadesPorCentro(int idCentroAtendimento)
        {
            var especialidades = _context.Especialidades
                .Where(e => e.idCentroAtendimento == idCentroAtendimento)
                .Select(e => new { e.idEspecialidade, e.nmEspecialidade })
                .ToList();

            return Json(especialidades);
        }

        [HttpGet]
        public JsonResult GetServicosFiltrados(int idCentroAtendimento, int idEspecialidade)
        {
        var servicos = _context.Servicos
            .Join(
                _context.Especialidades,
                servico => servico.idEspecialidade,
                especialidade => especialidade.idEspecialidade,
                (servico, especialidade) => new { Servico = servico, Especialidade = especialidade }
            )
            .Join(
                _context.CentroAtendimentos,
                se => se.Especialidade.idCentroAtendimento,
                centroAtendimento => centroAtendimento.idCentroAtendimento,
                (se, centroAtendimento) => new { se.Servico, se.Especialidade, CentroAtendimento = centroAtendimento }
            )
            .Where(x => x.Especialidade.idEspecialidade == idEspecialidade && x.CentroAtendimento.idCentroAtendimento == idCentroAtendimento)
            .Select(x => new
            {
                x.Servico.nmServico,
                x.Servico.idServico,
                x.Especialidade.idEspecialidade,
                x.CentroAtendimento.idCentroAtendimento
            })
            .ToList();

            return Json(servicos);
        }

        [HttpGet]
        public JsonResult FiltrarHorarios(int? centroAtendimento, int? especialidade, int? servico, string? nomeUsuario, string? dtInicio, string? dtFim)
        {
            var nmUser = _context.Pessoas.FirstOrDefault(p => p.nmPessoa == nomeUsuario);

            DateTime dataInicio;
            DateTime dataFim;

            DateTime.TryParseExact(dtInicio, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataInicio);
            DateTime.TryParseExact(dtFim, "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataFim);

            IEnumerable<object> horariosFiltrados = new List<object>(); 
            // Nome & Centro 
            if (nomeUsuario != null && centroAtendimento != null && especialidade == null && servico == null && dtInicio == null && dtFim == null)
            {
                horariosFiltrados = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento)
                    .Join(_context.Pessoas,
                          af => af.idPessoa,
                          p => p.idPessoa,
                          (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.Pessoa.nmPessoa == nomeUsuario)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda,
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                    .ToList();
            }
            // Nome & Centro & Especialidade
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico == null && dtInicio == null && dtFim == null)
            {
                horariosFiltrados = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade)
                    .Join(_context.Pessoas,
                          af => af.idPessoa,
                          p => p.idPessoa,
                          (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.Pessoa.nmPessoa == nomeUsuario)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda,
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                    .ToList();
            }
            // Nome & Centro & Especialidade & Servico 
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico != null && dtInicio == null && dtFim == null)
            {
                horariosFiltrados = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade && af.idServico == servico) 
                    .Join(_context.Pessoas,
                          af => af.idPessoa,
                          p => p.idPessoa,
                          (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.Pessoa.nmPessoa == nomeUsuario)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda,
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                    .ToList();
            }
            // Apenas Centro 
            if (nomeUsuario == null && centroAtendimento != null && especialidade == null && servico == null && dtInicio == null && dtFim == null)
            {
                horariosFiltrados = _context.AgendamentoFulls.Where(af => af.icAtivoAgendamento == true && af.idCentroAtendimento == centroAtendimento).Join(_context.Pessoas, af => af.idPessoa, p => p.idPessoa,
                                                          (af, p) => new
                                                          {
                                                              af.dtAgenda,
                                                              af.hrInicio,
                                                              af.hrFim,
                                                              af.nmCentroAtendimento,
                                                              af.nmEspecialidade,
                                                              af.nmServico,
                                                              af.idAgendamento,
                                                              af.nmUnidadeAtendimento,
                                                              p.nmPessoa,
                                                              p.edEmail,
                                                              p.nuTelefone
                                                          }).ToList();
            }
            // Centro & Especialidade
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico == null && dtInicio == null && dtFim == null)
            {
                horariosFiltrados = _context.AgendamentoFulls.Where(af => af.icAtivoAgendamento == true && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade).Join(_context.Pessoas, af => af.idPessoa, p => p.idPessoa,
                                                          (af, p) => new
                                                          {
                                                              af.dtAgenda,
                                                              af.hrInicio,
                                                              af.hrFim,
                                                              af.nmCentroAtendimento,
                                                              af.nmEspecialidade,
                                                              af.nmServico,
                                                              af.idAgendamento,
                                                              af.nmUnidadeAtendimento,
                                                              p.nmPessoa,
                                                              p.edEmail,
                                                              p.nuTelefone
                                                          }).ToList();
            }
            // Centro & Especialidade & Serviço
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico != null && dtInicio == null && dtFim == null)
            {
                horariosFiltrados = _context.AgendamentoFulls.Where(af => af.icAtivoAgendamento == true && af.idCentroAtendimento == centroAtendimento &&
                                                                        af.idEspecialidade == especialidade && af.idServico == servico).Join(_context.Pessoas, af => af.idPessoa, p => p.idPessoa,
                                                          (af, p) => new
                                                          {
                                                              af.dtAgenda,
                                                              af.hrInicio,
                                                              af.hrFim,
                                                              af.nmCentroAtendimento,
                                                              af.nmEspecialidade,
                                                              af.nmServico,
                                                              af.idAgendamento,
                                                              af.nmUnidadeAtendimento,
                                                              p.nmPessoa,
                                                              p.edEmail,
                                                              p.nuTelefone
                                                          }).ToList();
            }
            // Apenas Nome 
            if (nomeUsuario != null && centroAtendimento == null && especialidade == null && servico == null && dtInicio == null && dtFim == null)
            {
                horariosFiltrados = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento)
                    .Join(_context.Pessoas,
                          af => af.idPessoa,
                          p => p.idPessoa,
                          (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.Pessoa.nmPessoa == nomeUsuario)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda,
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                    .ToList();
            }

            // Data Inicio
            if (nomeUsuario == null && centroAtendimento == null && especialidade == null && servico == null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento)
                    .Join(_context.Pessoas,
                            af => af.idPessoa,
                            p => p.idPessoa,
                            (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda, 
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                 .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"), 
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Inicio & Centro
            if (nomeUsuario == null && centroAtendimento != null && especialidade == null && servico == null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento)
                    .Join(_context.Pessoas,
                            af => af.idPessoa,
                            p => p.idPessoa,
                            (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda,
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                 .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Inicio & Centro & Especialidade
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico == null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade)
                    .Join(_context.Pessoas,
                            af => af.idPessoa,
                            p => p.idPessoa,
                            (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda,
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                 .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Inicio & Centro & Especialidade & Servico
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico != null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                    .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade
                          && af.idServico == servico)
                    .Join(_context.Pessoas,
                            af => af.idPessoa,
                            p => p.idPessoa,
                            (af, p) => new { AgendamentoFull = af, Pessoa = p })
                    .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio)
                    .Select(joined => new
                    {
                        joined.AgendamentoFull.dtAgenda,
                        joined.AgendamentoFull.hrInicio,
                        joined.AgendamentoFull.hrFim,
                        joined.AgendamentoFull.nmCentroAtendimento,
                        joined.AgendamentoFull.nmEspecialidade,
                        joined.AgendamentoFull.nmServico,
                        joined.AgendamentoFull.idAgendamento,
                        joined.AgendamentoFull.nmUnidadeAtendimento,
                        joined.Pessoa.nmPessoa,
                        joined.Pessoa.edEmail,
                        joined.Pessoa.nuTelefone
                    })
                 .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Data Inicio
            if (nomeUsuario != null && centroAtendimento != null && especialidade == null && servico == null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Data Inicio
            if (nomeUsuario != null && centroAtendimento != null && especialidade == null && servico == null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Especialidade & Data Inicio
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico == null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Especialidade & Servico & Data Inicio
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico != null && dtInicio != null && dtFim == null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade
                          && af.idServico == servico)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }
            
            // Data Fim
            if (nomeUsuario == null && centroAtendimento == null && especialidade == null && servico == null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Fim & Centro
            if (nomeUsuario == null && centroAtendimento != null && especialidade == null && servico == null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Fim & Centro & Especialidade
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico == null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Fim & Centro & Especialidade & Serviço
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico != null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade
                          && af.idServico == servico)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Data Fim
            if (nomeUsuario != null && centroAtendimento == null && especialidade == null && servico == null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Data Fim
            if (nomeUsuario != null && centroAtendimento != null && especialidade == null && servico == null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Especialidade & Data Fim
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico == null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Especialidade & Servico & Data Fim
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico != null && dtInicio == null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade
                                          && af.idServico == servico)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Inicio Fim
            if (nomeUsuario == null && centroAtendimento == null && especialidade == null && servico == null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Inicio Fim & Centro
            if (nomeUsuario == null && centroAtendimento != null && especialidade == null && servico == null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Inicio Fim & Centro & Especialidade
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico == null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Data Inicio Fim & Centro & Especialidade & Serviço
            if (nomeUsuario == null && centroAtendimento != null && especialidade != null && servico != null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade
                          && af.idServico == servico)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Data Inicio Fim
            if (nomeUsuario != null && centroAtendimento == null && especialidade == null && servico == null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Data Inicio Fim
            if (nomeUsuario != null && centroAtendimento != null && especialidade == null && servico == null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Nome & Centro & Especialidade & Data Inicio Fim
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico == null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            // Tudo
            if (nomeUsuario != null && centroAtendimento != null && especialidade != null && servico != null && dtInicio != null && dtFim != null)
            {
                var horarios = _context.AgendamentoFulls
                   .Where(af => af.icAtivoAgendamento && af.idCentroAtendimento == centroAtendimento && af.idEspecialidade == especialidade
                          && af.idServico == servico)
                   .Join(_context.Pessoas,
                           af => af.idPessoa,
                           p => p.idPessoa,
                           (af, p) => new { AgendamentoFull = af, Pessoa = p })
                   .Where(joined => joined.AgendamentoFull.dtAgenda <= dataFim && joined.AgendamentoFull.dtAgenda >= dataInicio && joined.Pessoa.nmPessoa == nomeUsuario)
                   .Select(joined => new
                   {
                       joined.AgendamentoFull.dtAgenda,
                       joined.AgendamentoFull.hrInicio,
                       joined.AgendamentoFull.hrFim,
                       joined.AgendamentoFull.nmCentroAtendimento,
                       joined.AgendamentoFull.nmEspecialidade,
                       joined.AgendamentoFull.nmServico,
                       joined.AgendamentoFull.idAgendamento,
                       joined.AgendamentoFull.nmUnidadeAtendimento,
                       joined.Pessoa.nmPessoa,
                       joined.Pessoa.edEmail,
                       joined.Pessoa.nuTelefone
                   })
                .ToList();

                horariosFiltrados = horarios.Select(h => new
                {
                    dtAgenda = h.dtAgenda.ToString("dd/MM/yyyy"),
                    h.hrInicio,
                    h.hrFim,
                    h.nmCentroAtendimento,
                    h.nmEspecialidade,
                    h.nmServico,
                    h.idAgendamento,
                    h.nmUnidadeAtendimento,
                    h.nmPessoa,
                    h.edEmail,
                    h.nuTelefone
                }).ToList();
            }

            if (horariosFiltrados == null || !horariosFiltrados.Any())
            {
                return Json(new { success = false, message = "Nenhum horário encontrado." });
            }

            return Json(new { success = true, data = horariosFiltrados });
        }
    }
}