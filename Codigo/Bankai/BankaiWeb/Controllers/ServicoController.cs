using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace BankaiWeb.Controllers
{
    public class ServicoController : Controller
    {
        private readonly IServicoService servicoService;

        public ServicoController(IServicoService servicoService)
        {
            this.servicoService = servicoService;
        }

        // GET: Servico/Buscar
        public ActionResult Buscar(string? termo, int? idEspecialidade, DateTime? dataDesejada)
        {
            var especialidades = servicoService.ObterEspecialidades();
            var resultados = servicoService.BuscarServicos(termo, idEspecialidade, dataDesejada);

            var viewModel = new BuscarServicoViewModel
            {
                Termo = termo,
                IdEspecialidade = idEspecialidade,
                DataDesejada = dataDesejada,
                Especialidades = EspecialidadeMapper.ToViewModel(especialidades),
                Resultados = ServicoMapper.ToResultadoViewModel(resultados)
            };

            return View(viewModel);
        }

        // GET: Servico/ProfissionaisDisponiveis/5
        public ActionResult ProfissionaisDisponiveis(
            int idServico,
            DateTime? dataDesejada,
            string? ordenarPor = "melhor_avaliado",
            bool apenasVerificados = false,
            bool apenasDisponiveisNaData = false,
            decimal? precoMaximo = null)
        {
            var servico = servicoService.Get(idServico);
            if (servico == null)
            {
                return NotFound();
            }

            var profissionais = servicoService.BuscarProfissionaisPorServico(
                idServico,
                dataDesejada,
                ordenarPor,
                apenasVerificados,
                apenasDisponiveisNaData,
                precoMaximo);

            var viewModel = new ProfissionaisDisponiveisViewModel
            {
                IdServico = servico.Id,
                NomeServico = servico.Nome,
                DescricaoServico = servico.Descricao,
                DataDesejada = dataDesejada,
                OrdenarPor = string.IsNullOrWhiteSpace(ordenarPor) ? "melhor_avaliado" : ordenarPor,
                ApenasVerificados = apenasVerificados,
                ApenasDisponiveisNaData = apenasDisponiveisNaData,
                PrecoMaximo = precoMaximo,
                Profissionais = ServicoMapper.ToProfissionalResultadoViewModel(profissionais)
            };

            return View(viewModel);
        }

        // GET: ServicoController
        public ActionResult Index()
        {
            var servicos = servicoService.GetAll();
            var especialidades = servicoService.ObterEspecialidades()
                .ToDictionary(e => e.Id, e => e.Nome);

            var servicosViewModel = ServicoMapper.ToViewModel(servicos).Select(vm =>
            {
                if (string.IsNullOrWhiteSpace(vm.NomeEspecialidade)
                    && especialidades.TryGetValue(vm.IdEspecialidade, out var nomeEsp))
                {
                    vm.NomeEspecialidade = nomeEsp;
                }
                return vm;
            }).ToList();

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
            CarregarEspecialidades();
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
            CarregarEspecialidades(servicoViewModel.IdEspecialidade);
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
            CarregarEspecialidades(servicoViewModel.IdEspecialidade);
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
            CarregarEspecialidades(servicoViewModel.IdEspecialidade);
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

        private void CarregarEspecialidades(int? idEspecialidadeSelecionada = null)
        {
            var especialidades = servicoService.ObterEspecialidades();
            ViewBag.Especialidades = new SelectList(especialidades, "Id", "Nome", idEspecialidadeSelecionada);
        }
    }
}
