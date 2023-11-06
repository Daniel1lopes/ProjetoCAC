using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using agenda.Models;
using System.Runtime.Intrinsics.X86;
using System;

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
            ViewBag.ServicoNome = servico.deServico;
            ViewBag.EspecialidadeNome = _context.Especialidades.FirstOrDefault(e => e.idEspecialidade == servico.idEspecialidade)?.nmEspecialidade;

            ViewBag.CentroAtendimentoID = centroAtendimento.idCentroAtendimento;
            ViewBag.servicoUnidadeAtendimentoID = _context.ServicosUnidadeAtendimento.FirstOrDefault(e => e.idServicoUnidadeAtendimento == servicoID)?.idServicoUnidadeAtendimento;
            return View(unidadesAtendimento);
        }


        [Route("CentroAtendimento/{centroAtendimentoID}/{especialidadeID}/{servicoUnidadeAtendimentoID}/DatasDisponiveis/")]
        public IActionResult DatasDisponiveis([FromRoute] short servicoUnidadeAtendimentoID, [FromRoute] short centroAtendimentoID, [FromRoute] short especialidadeID)
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

            var unidadeNome = _context.ServicosUnidadeAtendimento
                .Include(sua => sua.UnidadeAtendimento)
                .Where(sua => sua.idServicoUnidadeAtendimento == servicoUnidadeAtendimentoID)
                .Select(sua => sua.UnidadeAtendimento.nmUnidadeAtendimento)
                .FirstOrDefault();

            ViewBag.unidadeNome = unidadeNome;

            var servico = _context.ServicosUnidadeAtendimento
                .Include(sua => sua.Servico)
                .Where(sua => sua.idServicoUnidadeAtendimento == servicoUnidadeAtendimentoID)
                .Select(sua => sua.Servico.nmServico)
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

            return View("DatasDisponiveis", AgendaSelecionada);
        }

        public IActionResult HorariosMarcados(int idAgenda, int idPessoa, Agendamento agendamento)
        {
            ViewData["Title"] = "AgendaCAC - Horários Marcados";

            ViewBag.idAgenda = idAgenda;
            ViewBag.idPessoa = idPessoa;

            var agenda = _context.Agendas.Find(idAgenda); 
            var pessoa = _context.Pessoas.Find(idPessoa); 

            if (agenda != null && pessoa != null)
            {
                agendamento.Agenda = agenda;
                agendamento.Pessoa = pessoa;

                agendamento = new Agendamento
                {
                    dtAgendamento = DateTime.Now,
                    icAtivo = true,
                    idAgenda = idAgenda,
                    idPessoa = idPessoa
                };

                _context.Agendamentos.Add(agendamento);

                _context.SaveChanges();
            }
            else
            {
                var errors = new List<string>();
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }

                TempData["MensagemErro"] = string.Join("\n", errors);
            }

            return View();
        }
    }
}

