using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{
    public class EspecialidadeController : Controller
    {
        private readonly IEspecialidadeService especialidadeService;

        public EspecialidadeController(IEspecialidadeService especialidadeService)
        {
            this.especialidadeService = especialidadeService;
        }

        // GET: EspecialidadeController
        public ActionResult Index()
        {
            var especialidades = especialidadeService.GetAll();
            var especialidadeViewModel = EspecialidadeMapper.ToViewModel(especialidades);
            return View(especialidadeViewModel);
        }

        // GET: EspecialidadeController/Details/5
        public ActionResult Details(uint id)
        {
            var especialidade = especialidadeService.Get(id);
            var especialidadeViewModel = especialidade != null ? EspecialidadeMapper.ToViewModel(especialidade) : new EspecialidadeViewModel();
            return View(especialidadeViewModel);
        }

        // GET: EspecialidadeController/Create
        public ActionResult Create()
        {
            var especialidadeViewModel = new EspecialidadeViewModel();
            return View(especialidadeViewModel);
        }

        // POST: EspecialidadeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EspecialidadeViewModel especialidadeViewModel)
        {
            if (ModelState.IsValid)
            {
                var especialidade = EspecialidadeMapper.ToEntity(especialidadeViewModel);
                especialidadeService.Create(especialidade);
                return RedirectToAction(nameof(Index));
            }
            return View(especialidadeViewModel);
        }

        // GET: EspecialidadeController/Edit/5
        public ActionResult Edit(uint id)
        {
            var especialidade = especialidadeService.Get(id);
            var especialidadeViewModel = especialidade != null ? EspecialidadeMapper.ToViewModel(especialidade) : new EspecialidadeViewModel();
            return View(especialidadeViewModel);
        }

        // POST: EspecialidadeController/Edit/5
        // POST: EspecialidadeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EspecialidadeViewModel especialidadeViewModel)
        {
            if (ModelState.IsValid)
            {
                var especialidade = EspecialidadeMapper.ToEntity(especialidadeViewModel);
                especialidadeService.Edit(especialidade);
                return RedirectToAction(nameof(Index));
            }
            return View(especialidadeViewModel);
        }

        // GET: EspecialidadeController/Delete/5
        public ActionResult Delete(uint id)
        {
            var especialidade = especialidadeService.Get(id);
            var especialidadeViewModel = especialidade != null ? EspecialidadeMapper.ToViewModel(especialidade) : new EspecialidadeViewModel();
            return View(especialidadeViewModel);
        }

        // POST: EspecialidadeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(EspecialidadeViewModel especialidadeViewModel)
        {
            especialidadeService.Delete(especialidadeViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
