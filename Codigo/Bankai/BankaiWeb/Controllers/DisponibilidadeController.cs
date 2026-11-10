using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BankaiWeb.Controllers
{
    public class DisponibilidadeController : Controller
    {
        private readonly IDisponibilidadeService disponibilidadeService;

        public DisponibilidadeController(IDisponibilidadeService disponibilidadeService)
        {
            this.disponibilidadeService = disponibilidadeService;
        }

        public IActionResult Index(uint? idPessoa)
        {
            var profissionalId = idPessoa ?? this.disponibilidadeService.GetFirstProfissionalId();
            ViewBag.IdPessoa = profissionalId ?? 0;

            if (!profissionalId.HasValue || !this.disponibilidadeService.IsProfissional(profissionalId.Value))
            {
                ViewBag.PessoaIndisponivel = true;
                return View(Enumerable.Empty<DisponibilidadeViewModel>());
            }

            ViewBag.PessoaIndisponivel = false;
            var disponibilidades = this.disponibilidadeService.GetByIdPessoa(profissionalId.Value);
            return View(DisponibilidadeMapper.ToViewModel(disponibilidades));
        }

        // Recebe a disponibilidade semanal enviada pela view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SalvarLote(List<DisponibilidadeViewModel>? disponibilidades, uint? idPessoa)
        {
            if (disponibilidades == null || disponibilidades.Count != 7)
            {
                return BadRequest();
            }

            uint profissionalId = idPessoa ?? disponibilidades[0].IdPessoa ?? 0;
            var diasEsperados = new HashSet<string> { "D", "SF", "TF", "QF", "QIF", "SXF", "S" };
            var diasEnviados = disponibilidades.Select(disponibilidade => disponibilidade.DiaSemana).ToHashSet();
            var idsExistentes = disponibilidades.Where(disponibilidade => disponibilidade.Id != 0)
                .Select(disponibilidade => disponibilidade.Id)
                .ToList();

            if (!diasEsperados.SetEquals(diasEnviados)
                || idsExistentes.Count != idsExistentes.Distinct().Count()
                || disponibilidades.Any(disponibilidade => disponibilidade.IdPessoa != profissionalId))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IdPessoa = profissionalId;
                ViewBag.PessoaIndisponivel = !this.disponibilidadeService.IsProfissional(profissionalId);
                return View("Index", disponibilidades);
            }

            if (!this.disponibilidadeService.IsProfissional(profissionalId))
            {
                ViewBag.IdPessoa = profissionalId;
                ViewBag.PessoaIndisponivel = true;
                return View("Index", disponibilidades);
            }

            foreach (var disponibilidade in disponibilidades.Where(item => item.Id != 0))
            {
                var existente = this.disponibilidadeService.Get(disponibilidade.Id);
                if (existente == null)
                {
                    return NotFound();
                }

                if (existente.IdPessoa != profissionalId || existente.DiaSemana != disponibilidade.DiaSemana)
                {
                    return BadRequest();
                }
            }

            foreach (var disponibilidade in disponibilidades)
            {
                var entity = DisponibilidadeMapper.ToEntity(disponibilidade);
                if (disponibilidade.Id == 0)
                {
                    this.disponibilidadeService.Create(entity);
                }
                else
                {
                    this.disponibilidadeService.Edit(entity);
                }
            }

            return RedirectToAction(nameof(Index), new { idPessoa = profissionalId });
        }

        public IActionResult Details(uint id)
        {
            var disponibilidade = this.disponibilidadeService.Get(id);
            if (disponibilidade == null)
            {
                return NotFound();
            }

            return View(DisponibilidadeMapper.ToViewModel(disponibilidade));
        }

        public IActionResult Edit(uint id)
        {
            var disponibilidade = this.disponibilidadeService.Get(id);
            if (disponibilidade == null)
            {
                return NotFound();
            }

            return View(DisponibilidadeMapper.ToViewModel(disponibilidade));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(uint id, DisponibilidadeViewModel disponibilidadeViewModel)
        {
            if (id != disponibilidadeViewModel.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(disponibilidadeViewModel);
            }

            var existente = this.disponibilidadeService.Get(id);
            if (existente == null)
            {
                return NotFound();
            }

            if (disponibilidadeViewModel.IdPessoa != (uint)existente.IdPessoa)
            {
                return BadRequest();
            }

            this.disponibilidadeService.Edit(DisponibilidadeMapper.ToEntity(disponibilidadeViewModel));
            return RedirectToAction(nameof(Index), new { idPessoa = disponibilidadeViewModel.IdPessoa });
        }

        public IActionResult Delete(uint id)
        {
            var disponibilidade = this.disponibilidadeService.Get(id);
            if (disponibilidade == null)
            {
                return NotFound();
            }

            return View(DisponibilidadeMapper.ToViewModel(disponibilidade));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(uint id)
        {
            var disponibilidade = this.disponibilidadeService.Get(id);
            if (disponibilidade == null)
            {
                return NotFound();
            }

            this.disponibilidadeService.Delete(id);
            return RedirectToAction(nameof(Index), new { idPessoa = (uint)disponibilidade.IdPessoa });
        }
    }
}