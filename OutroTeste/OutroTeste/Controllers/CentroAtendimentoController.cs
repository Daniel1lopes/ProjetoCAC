using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using OutroTeste.Models;
using System.Runtime.Intrinsics.X86;
using System;

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

        [Route("{AtendimentoID}/Especialidades/")]
        public IActionResult Especialidades([FromRoute] short centroAtendimentoID)
        {
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

        [Route("{EspecialidadeID}/Servicos/")]
        public IActionResult Servicos([FromRoute] int EspecialidadeID)
        {
            var especialidade = _context.Especialidades
            .Include(e => e.Servicos)
            .FirstOrDefault(e => e.idEspecialidade == EspecialidadeID);
            if (especialidade == null)
            {
                return NotFound();
            }
            ViewBag.EspecialidadeNome = especialidade.nmEspecialidade;
            ViewBag.CentroAtendimentoNome = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == especialidade.idCentroAtendimento)?.nmCentroAtendimento;
            return View(especialidade.Servicos.ToList());
        }

        [Route("{centroAtendimentoID}/{servicoID}/UnidadesAtendimento")]
        public IActionResult UnidadesAtendimento([FromRoute] short centroAtendimentoID, [FromRoute] short servicoID)
        {
            var centroAtendimento = _context.CentroAtendimentos.FirstOrDefault(c => c.idCentroAtendimento == centroAtendimentoID);
            if (centroAtendimento == null)
            {
                return NotFound();
            }
            var servico = _context.Servicos.FirstOrDefault(s => s.idServico == servicoID);
            if (servico == null)
            {
                return NotFound();
            }
            var unidadesAtendimento = _context.ServicosUnidadeAtendimento
            .Where(sua => sua.idServico == servicoID)
            .Select(sua => sua.UnidadeAtendimento)
            .ToList();
            ViewBag.CentroAtendimentoNome = centroAtendimento.nmCentroAtendimento;
            ViewBag.ServicoNome = servico.deServico;
            ViewBag.EspecialidadeNome = _context.Especialidades.FirstOrDefault(e => e.idEspecialidade == servico.idEspecialidade)?.nmEspecialidade;

            return View(unidadesAtendimento);
        }


        [Route("{servicoUnidadeAtendimentoID}/DatasDisponiveis/")]
        public IActionResult DatasDisponiveis([FromRoute] short servicoUnidadeAtendimentoID)
        {
            var idServicoUnidadeAtendimento = _context.ServicosUnidadeAtendimento
            .Where(sua => sua.idServico == servicoId && sua.UnidadeAtendimento.idUnidadeAtendimento == centroId)
            .Select(sua => sua.idServicoUnidadeAtendimento)
            .FirstOrDefault();

            var DatasDisponiveis = _context.Agendas
                .FromSqlInterpolated($@"
            SELECT DISTINCT A.dtAgenda  
            FROM CACBD.CACTB.Agenda A
            INNER JOIN CACBD.CACTB.ServicoUnidadeAtendimento SUA
              ON SUA.idServicoUnidadeAtendimento = A.idServicoUnidadeAtendimento
            LEFT JOIN (
            SELECT idAgenda, COUNT(1) AS nuQtdeAgendamento
            FROM CACBD.CACTB.Agendamento
            GROUP BY idAgenda
            ) AS QA ON QA.idAgenda = A.idAgenda
            WHERE A.idServicoUnidadeAtendimento = {idServicoUnidadeAtendimento}
            AND (A.nuVagas - A.nuReserva - ISNULL(QA.nuQtdeAgendamento,0)) > 0")
                .Select(a => a.dtAgenda)
                .ToList();

            return View(DatasDisponiveis);
        }
    }
}

