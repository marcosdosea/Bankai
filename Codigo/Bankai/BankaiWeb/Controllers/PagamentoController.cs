using System;
using Microsoft.AspNetCore.Mvc;
using Core.Service;
using BankaiWeb.Models;
using BankaiWeb.Mappers;

namespace BankaiWeb.Controllers
{
    public class PagamentoController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        public IActionResult Index()
        {
            var pagamentos = _pagamentoService.getAll();
            var viewModel = PagamentoMapper.ToViewModel(pagamentos);
            return View(viewModel);
        }

        public IActionResult Details(uint id)
        {
            var pagamento = _pagamentoService.get(id);
            if (pagamento == null)
            {
                return NotFound();
            }
            var viewModel = PagamentoMapper.ToViewModel(pagamento);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult RealizarPagamento(int idAgendamento, decimal valor = 0)
        {
            var viewModel = new PagamentoViewModel
            {
                CodigoAgendamento = idAgendamento,
                ValorTransacao = valor
                FormaPagamento = "Pix", // Valor padrão
                StatusPagamento = "Aguardando Processamento" // Valor padrão
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RealizarPagamento(PagamentoViewModel pagamentoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(pagamentoViewModel);
            }
            
            try
            {
                var pagamento = PagamentoMapper.ToEntity(pagamentoViewModel);
                var pagamentoProcessado = _pagamentoService.RealizarPagamento(pagamento);

                return RedirectToAction(nameof(Details), new { id = pagamentoProcessado.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Ocorreu um erro ao processar o pagamento: {ex.Message}");
                return View(pagamentoViewModel);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PagamentoViewModel pagamentoViewModel)
        {
            if (ModelState.IsValid)
            {
                var pagamento = PagamentoMapper.ToEntity(pagamentoViewModel);
                _pagamentoService.Create(pagamento);
                return RedirectToAction(nameof(Index));
            }
            return View(pagamentoViewModel);
        }

        public IActionResult Edit(int id)
        {
            var pagamento = _pagamentoService.get((uint)id);
            if (pagamento == null)
            {
                return NotFound();
            }
            var viewModel = PagamentoMapper.ToViewModel(pagamento);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PagamentoViewModel pagamentoViewModel)
        {
            if (id != (uint)pagamentoViewModel.Codigo)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var pagamento = PagamentoMapper.ToEntity(pagamentoViewModel);
                _pagamentoService.Edit(pagamento);
                return RedirectToAction(nameof(Index));
            }
            return View(pagamentoViewModel);
        }

        public IActionResult Delete(uint id)
        {
            var pagamento = _pagamentoService.get((uint)id);
            if (pagamento == null)
            {
                return NotFound();
            }
            var viewModel = PagamentoMapper.ToViewModel(pagamento);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(uint id)
        {
            var pagamento = _pagamentoService.get((uint)id);
            if (pagamento == null)
            {
                return NotFound();
            }
            _pagamentoService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
