using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{
    public class GarantiaController : Controller
    {
        private readonly IGarantiaService garantiaService;

        public GarantiaController(IGarantiaService garantiaService)
        {
            this.garantiaService = garantiaService;
        }

        // GET: Garantia
        public ActionResult Index()
        {
            var chamados = garantiaService.GetAll();
            var chamadosViewModel = GarantiaMapper.ToViewModel(chamados);
            return View(chamadosViewModel);
        }

        // GET: Garantia/Details/5
        public ActionResult Details(uint id)
        {
            var chamado = garantiaService.Get(id);
            var chamadoViewModel = chamado != null ? GarantiaMapper.ToViewModel(chamado) : new GarantiaViewModel();
            return View(chamadoViewModel);
        }

        // GET: Garantia/Create
        public ActionResult Create()
        {
            var garantiaViewModel = new GarantiaViewModel
            {
                DataChamado = DateTime.Now,
                StatusGarantia = "S"
            };
            return View(garantiaViewModel);
        }

        // POST: Garantia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GarantiaViewModel garantiaViewModel)
        {
            if (ModelState.IsValid)
            {
                var chamado = GarantiaMapper.ToEntity(garantiaViewModel);
                garantiaService.Create(chamado);
                return RedirectToAction(nameof(Index));
            }
            return View(garantiaViewModel);
        }

        // GET: Garantia/Edit/5
        public ActionResult Edit(uint id)
        {
            var chamado = garantiaService.Get(id);
            var chamadoViewModel = chamado != null ? GarantiaMapper.ToViewModel(chamado) : new GarantiaViewModel();
            return View(chamadoViewModel);
        }

        // POST: Garantia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GarantiaViewModel garantiaViewModel)
        {
            if (ModelState.IsValid)
            {
                var chamado = GarantiaMapper.ToEntity(garantiaViewModel);
                garantiaService.Edit(chamado);
                return RedirectToAction(nameof(Index));
            }
            return View(garantiaViewModel);
        }

        // GET: Garantia/Delete/5
        public ActionResult Delete(uint id)
        {
            var chamado = garantiaService.Get(id);
            var chamadoViewModel = chamado != null ? GarantiaMapper.ToViewModel(chamado) : new GarantiaViewModel();
            return View(chamadoViewModel);
        }

        // POST: Garantia/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(GarantiaViewModel garantiaViewModel)
        {
            garantiaService.Delete((uint)garantiaViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
