using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

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
            var disponibilidades = idPessoa.HasValue
                ? this.disponibilidadeService.GetByIdPessoa(idPessoa.Value)
                : this.disponibilidadeService.GetAll();

            return View(DisponibilidadeMapper.ToViewModel(disponibilidades));
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

        public IActionResult Create()
        {
            return View(new DisponibilidadeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DisponibilidadeViewModel disponibilidadeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(disponibilidadeViewModel);
            }

            this.disponibilidadeService.Create(DisponibilidadeMapper.ToEntity(disponibilidadeViewModel));
            return RedirectToAction(nameof(Index));
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

            if (this.disponibilidadeService.Get(id) == null)
            {
                return NotFound();
            }

            this.disponibilidadeService.Edit(DisponibilidadeMapper.ToEntity(disponibilidadeViewModel));
            return RedirectToAction(nameof(Index));
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
            if (this.disponibilidadeService.Get(id) == null)
            {
                return NotFound();
            }

            this.disponibilidadeService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
