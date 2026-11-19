using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{
    public class OrcamentoController : Controller
    {
        private readonly IOrcamentoService orcamentoService;

        public OrcamentoController(IOrcamentoService orcamentoService)
        {
            this.orcamentoService = orcamentoService;
        }

        
        public ActionResult Index()
        {
            var orcamentos = orcamentoService.GetAll();
            var orcamentosViewModel = OrcamentoMapper.ToViewModel(orcamentos);
            return View(orcamentosViewModel);
        }

        
        public ActionResult Details(uint id)
        {
            var orcamento = orcamentoService.Get(id);
            var orcamentoViewModel = OrcamentoMapper.ToViewModel(orcamento!);

            if (orcamento != null)
            {
                orcamentoViewModel.TaxaPlataforma = orcamentoService.CalcularTaxaPlataforma(orcamento.ValorProposto);
                orcamentoViewModel.ValorLiquido = orcamentoService.CalcularValorLiquido(orcamento.ValorProposto);
            }

            return View(orcamentoViewModel);
        }

        
        public ActionResult Create()
        {
            var orcamentoViewModel = new OrcamentoViewModel();
            return View(orcamentoViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OrcamentoViewModel orcamentoViewModel)
        {
            if (ModelState.IsValid)
            {
                if (orcamentoViewModel.ArquivoMateriais != null && orcamentoViewModel.ArquivoMateriais.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    orcamentoViewModel.ArquivoMateriais.CopyTo(memoryStream);
                    orcamentoViewModel.MateriaisAdicionais = memoryStream.ToArray();
                }

                var orcamento = OrcamentoMapper.ToEntity(orcamentoViewModel);
                orcamentoService.Create(orcamento, orcamentoViewModel.IdAgendamento);
                return RedirectToAction(nameof(Index));
            }
            return View(orcamentoViewModel);
        }


        public ActionResult Edit(uint id)
        {
            var orcamento = orcamentoService.Get(id);
            var orcamentoViewModel = OrcamentoMapper.ToViewModel(orcamento!);
            return View(orcamentoViewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(uint id, OrcamentoViewModel orcamentoViewModel)
        {
            ModelState.Remove(nameof(orcamentoViewModel.IdAgendamento));

            if (ModelState.IsValid)
            {
                Orcamento orcamento = OrcamentoMapper.ToEntity(orcamentoViewModel);
                orcamentoService.Edit(orcamento);
            }
            return RedirectToAction(nameof(Index));
        }

        
        public ActionResult Delete(uint id)
        {
            var orcamento = orcamentoService.Get(id);
            var orcamentoViewModel = OrcamentoMapper.ToViewModel(orcamento!);
            return View(orcamentoViewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(OrcamentoViewModel orcamentoViewModel)
        {
            orcamentoService.Delete(orcamentoViewModel.Id);
            return RedirectToAction(nameof(Index));
        }
    }
}