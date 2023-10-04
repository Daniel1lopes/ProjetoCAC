using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using OutroTeste.Models;

namespace OutroTeste.Controllers
{
    public class CentroAtendimentoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CentroAtendimentoController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Agendamento()
        {
            return View();
        }
        public IActionResult Index()
        {
            var centroAtendimentos = _context.CentroAtendimentos.ToList();
            return View(centroAtendimentos);
        }
        [Route("CentroAtendimento/Especialidades/{id}")]
        public IActionResult Especialidades([FromRoute] short id)
        {
            var centroAtendimento = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == id);
            if (centroAtendimento == null)
            {
                return NotFound();
            }
            var especialidades = _context.Especialidades
            .Where(e => e.idCentroAtendimento == id)
            .ToList();
            ViewBag.CentroAtendimentoNome = centroAtendimento.nmCentroAtendimento;
            return View(especialidades);
        }
        [Route("CentroAtendimento/Servicos/{id}")]
        public IActionResult Servicos([FromRoute] short id)
        {
            var especialidade = _context.Especialidades
            .Include(e => e.Servicos)
            .FirstOrDefault(e => e.idEspecialidade == id);
            if (especialidade == null)
            {
                return NotFound();
            }
            ViewBag.EspecialidadeNome = especialidade.nmEspecialidade;
            ViewBag.CentroAtendimentoNome = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == especialidade.idCentroAtendimento)?.nmCentroAtendimento;
            return View(especialidade.Servicos.ToList());
        }
        [Route("CentroAtendimento/UnidadesAtendimento/{centroId}/{servicoId}")]
        public IActionResult UnidadesAtendimento([FromRoute] short centroId, [FromRoute] short servicoId)
        {
            var centroAtendimento = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == centroId);
            if (centroAtendimento == null)
            {
                return NotFound();
            }
            var servico = _context.Servicos.FirstOrDefault(s => s.idServico == servicoId);
            if (servico == null)
            {
                return NotFound();
            }
            var unidadesAtendimento = _context.ServicoUnidadeAtendimento
            .Where(sua => sua.idServico == servicoId)
            .Select(sua => sua.UnidadeAtendimento)
            .ToList();
            ViewBag.CentroAtendimentoNome = centroAtendimento.nmCentroAtendimento;
            ViewBag.ServicoNome = servico.deServico;
            ViewBag.EspecialidadeNome = _context.Especialidades.FirstOrDefault(e => e.idEspecialidade == servico.idEspecialidade)?.nmEspecialidade;
            return View(unidadesAtendimento);
        }
        public IActionResult DatasDisponiveis([FromRoute] short idServico, [FromRoute] short idUnidadeAtendimento)
        {
            var datasDisponiveis = _context.Agendas
                .Where(a => a.idServico == idServico && a.idUnidadeAtendimento == idUnidadeAtendimento)
                .Join(
                    _context.UnidadesAtendimento,
                    a => a.idUnidadeAtendimento,
                    ua => ua.idUnidadeAtendimento,
                    (a, ua) => new { Agenda = a, UnidadeAtendimento = ua })
                .GroupJoin(
                    _context.Agendamentos.GroupBy(ag => ag.idAgenda)
                        .Select(g => new { idAgenda = g.Key, nuQtdeAgendamento = g.Count() }),
                    a => a.Agenda.idAgenda,
                    qa => qa.idAgenda,
                    (a, qa) => new { Agenda = a.Agenda, UnidadeAtendimento = a.UnidadeAtendimento, QtdeAgendamento = qa })
                .SelectMany(
                    x => x.QtdeAgendamento.DefaultIfEmpty(),
                    (x, qa) => new { x.Agenda, x.UnidadeAtendimento, QtdeAgendamento = (qa == null ? 0 : qa.nuQtdeAgendamento) })
                .Where(x => x.Agenda.nuVagas - x.Agenda.nuReserva - x.QtdeAgendamento > 0)
                .Select(x => new { x.Agenda.dtAgenda, x.UnidadeAtendimento })
                .Distinct();

            if (!datasDisponiveis.Any())
            {
                return NotFound();
            }

            return View(datasDisponiveis);
        }


    }
}
