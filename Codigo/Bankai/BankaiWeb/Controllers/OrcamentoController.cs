using BankaiWeb.Mappers;
using BankaiWeb.Models;
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

        // GET: OrcamentoController
        public IActionResult Index()
        {
            var orcamentosDTO = orcamentoService.GetAll();
            var orcamentosViewModel = OrcamentoMapper.ToViewModel(orcamentosDTO);
            return View(orcamentosViewModel);
        }

        // GET: OrcamentoController/Orcar?idAgendamento=1&idServico=1
        public IActionResult Orcar(int idAgendamento, int idServico)
        {
            var orcamentoDTO = orcamentoService.Get(idAgendamento, idServico);
            if (orcamentoDTO == null)
            {
                return NotFound();
            }

            var viewModel = OrcamentoMapper.ToViewModel(orcamentoDTO);
            viewModel.TaxaPlataforma = orcamentoService.CalcularTaxaPlataforma(viewModel.ValorProposto);
            viewModel.ValorLiquido = orcamentoService.CalcularValorLiquido(viewModel.ValorProposto);

            return View(viewModel);
        }

        // POST: OrcamentoController/Orcar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Orcar(OrcamentoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    orcamentoService.EnviarOrcamento(viewModel.IdAgendamento, viewModel.IdServico, viewModel.ValorProposto);
                    return RedirectToAction(nameof(Details), new { idAgendamento = viewModel.IdAgendamento, idServico = viewModel.IdServico });
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            viewModel.TaxaPlataforma = orcamentoService.CalcularTaxaPlataforma(viewModel.ValorProposto);
            viewModel.ValorLiquido = orcamentoService.CalcularValorLiquido(viewModel.ValorProposto);
            return View(viewModel);
        }

        // GET: OrcamentoController/Details?idAgendamento=1&idServico=1
        public IActionResult Details(int idAgendamento, int idServico)
        {
            var orcamentoDTO = orcamentoService.Get(idAgendamento, idServico);
            if (orcamentoDTO == null)
            {
                return NotFound();
            }

            var viewModel = OrcamentoMapper.ToViewModel(orcamentoDTO);
            return View(viewModel);
        }
    }
}

