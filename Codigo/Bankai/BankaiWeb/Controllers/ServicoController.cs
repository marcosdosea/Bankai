using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{
    public class ServicoController : Controller
    {
        private readonly IServicoService servicoService;

        public ServicoController(IServicoService servicoService)
        {
            this.servicoService = servicoService;
        }

        // GET: ServicoController
        public ActionResult Index()
        {
            var servicos = servicoService.GetAll();
            var servicosViewModel = ServicoMapper.ToViewModel(servicos);
            return View(servicosViewModel);
        }

        // GET: ServicoController/Details/5
        public ActionResult Details(int id)
        {
            var servico = servicoService.Get(id);
            if (servico == null)
            {
                return NotFound();
            }
            var servicoViewModel = ServicoMapper.ToViewModel(servico);
            return View(servicoViewModel);
        }

        // GET: ServicoController/Create
        public ActionResult Create()
        {
            var servicoViewModel = new ServicoViewModel();
            return View(servicoViewModel);
        }

        // POST: ServicoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServicoViewModel servicoViewModel)
        {
            if (ModelState.IsValid)
            {
                var servico = ServicoMapper.ToEntity(servicoViewModel);
                servicoService.Create(servico);
                return RedirectToAction(nameof(Index));
            }
            return View(servicoViewModel);
        }

        // GET: ServicoController/Edit/5
        public ActionResult Edit(int id)
        {
            var servico = servicoService.Get(id);
            if (servico == null)
            {
                return NotFound();
            }
            var servicoViewModel = ServicoMapper.ToViewModel(servico);
            return View(servicoViewModel);
        }

        // POST: ServicoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServicoViewModel servicoViewModel)
        {
            if (ModelState.IsValid)
            {
                var servico = ServicoMapper.ToEntity(servicoViewModel);
                servicoService.Edit(servico);
                return RedirectToAction(nameof(Index));
            }
            return View(servicoViewModel);
        }

        // GET: ServicoController/Delete/5
        public ActionResult Delete(int id)
        {
            var servico = servicoService.Get(id);
            if (servico == null)
            {
                return NotFound();
            }
            var servicoViewModel = ServicoMapper.ToViewModel(servico);
            return View(servicoViewModel);
        }

        // POST: ServicoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ServicoViewModel servicoViewModel)
        {
            servicoService.Delete(servicoViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}

