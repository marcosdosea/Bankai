using BankaiWeb.Models;
using Core;
using Core.Service;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{

    public class AgendamentoController : Controller
    {
        private readonly IAgendamentoService _agendamentoService;
        private readonly IMapper _mapper;
        public AgendamentoController(IAgendamentoService agendamentoService, IMapper mapper)
        {
            _agendamentoService = agendamentoService;
            _mapper = mapper;
        }

        public ActionResult Index(int idPessoa)
        {
            //trabalha com o DTO
            var listaAgendamentos = _agendamentoService.GetAllByUser(idPessoa);
            return View(listaAgendamentos);
        }

        // GET: AgendamentoController/Details
        public ActionResult Details(int id)
        {
            Agendamento agendamento = _agendamentoService.Get(id);
            if(agendamento == null)
            {
                return NotFound();
            }
            AgendamentoViewModel agendamentoModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            return View(agendamentoModel);
        }

        //GET: AgendamentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        //POST: AgendamentoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AgendamentoViewModel agendamentoViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(agendamentoViewModel);
            }

            var agendamento = _mapper.Map<Agendamento>(agendamentoViewModel);

            try
            {
                await _agendamentoService.Create(agendamento);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(agendamentoViewModel);
            }

            return RedirectToAction(nameof(Index));
        }
        // GET: AgendamentoController/Edit
        public ActionResult Edit(int id)
        {
            Agendamento agendamento = _agendamentoService.Get(id);
            AgendamentoViewModel agendamentoViewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            return View(agendamentoViewModel);
        }
        // POST: AgendamentoController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AgendamentoViewModel agendamentoViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(agendamentoViewModel);
            }

            var agendamento = _mapper.Map<Agendamento>(agendamentoViewModel);
            agendamento.Id = id;

            try
            {
                 _agendamentoService.Edit(agendamento);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(agendamentoViewModel);
            }

            return RedirectToAction(nameof(Index));
            
        }

        // GET: AgendamentoController/Delete
        public ActionResult Delete(int id)
        {
            Agendamento agendamento = _agendamentoService.Get(id);
            AgendamentoViewModel agendamentoViewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            return View(agendamentoViewModel);
        }

        // POST: AgendamentoController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, AgendamentoViewModel agendamentoViewModel)
        {
            try
            {
                _agendamentoService.Delete(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(agendamentoViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AgendamentoController/Avaliar
        public ActionResult Avaliar(int id)
        {
            Agendamento agendamento = _agendamentoService.Get(id);
            AgendamentoViewModel agendamentoViewModel = _mapper.Map<AgendamentoViewModel>(agendamento);
            return View(agendamentoViewModel);
        }

        // POST: AgendamentoController/Avaliar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Avaliar(int id, AgendamentoViewModel agendamentoViewModel)
        {
            if (string.IsNullOrEmpty(agendamentoViewModel.Nota) || ModelState["Nota"]?.Errors.Count > 0)
            {
                ModelState.AddModelError("Nota", "Informe uma nota válida.");
                return View("Details", agendamentoViewModel);
            }

            try
            {
                _agendamentoService.Avaliar(id, agendamentoViewModel.Nota, agendamentoViewModel.Comentario);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Details", agendamentoViewModel);
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
