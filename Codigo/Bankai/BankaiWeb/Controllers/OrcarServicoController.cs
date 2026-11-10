using BankaiWeb.Mapper;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{
    public class OrcarServicoController : Controller
    {

        private readonly IOrcarServicoService orcarServicoService;

        public OrcarServicoController(IOrcarServicoService orcarServicoService)
        {
            this.orcarServicoService = orcarServicoService;
        }


        // GET: OrcarServicoController
        public ActionResult Index()
        {
            var orcamentos = orcarServicoService.GetAll();
            var orcamentosViewModel = OrcarServicoMapper.ToViewModel(orcamentos);
            return View(orcamentosViewModel);
        }

        // GET: OrcarServicoController/Details/5
        public ActionResult Details(uint id)
        {
            var orcamento = orcarServicoService.Get(id);
            var orcarServicoViewModel = orcamento != null ? OrcarServicoMapper.ToViewModel(orcamento) : new OrcarServicoViewModel();
            return View(orcarServicoViewModel);
        }

        // GET: OrcarServicoController/Create
        public ActionResult Create()
        {
            var orcarServicoViewModel = new OrcarServicoViewModel();
            orcarServicoViewModel.ValidadeDias = 30; // Valor padrão de 30 dias
            return View(orcarServicoViewModel);
        }

        // POST: OrcarServicoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrcarServicoViewModel orcarServicoViewModel)
        {
            if (ModelState.IsValid)
            {
                var orcamento = OrcarServicoMapper.ToEntity(orcarServicoViewModel);
                orcarServicoService.Create(orcamento);
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: OrcarServicoController/Edit/5
        public ActionResult Edit(uint id)
        {
            var orcamento = orcarServicoService.Get(id);
            var orcarServicoViewModel = orcamento != null ? OrcarServicoMapper.ToViewModel(orcamento) : new OrcarServicoViewModel();
            return View(orcarServicoViewModel);
        }

        // POST: OrcarServicoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrcarServicoViewModel orcarServicoViewModel)
        {
            if (ModelState.IsValid)
            {
                var orcamento = OrcarServicoMapper.ToEntity(orcarServicoViewModel);
                orcarServicoService.Edit(orcamento);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: OrcarServicoController/Delete/5
        public ActionResult Delete(uint id)
        {
            var orcamento = orcarServicoService.Get(id);
            var orcarServicoViewModel = orcamento != null ? OrcarServicoMapper.ToViewModel(orcamento) : new OrcarServicoViewModel();
            return View(orcarServicoViewModel);
        }

        // POST: OrcarServicoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(OrcarServicoViewModel orcarServicoViewModel)
        {
            orcarServicoService.Delete(orcarServicoViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}
