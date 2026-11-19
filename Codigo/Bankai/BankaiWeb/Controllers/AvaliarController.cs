using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{
    public class AvaliarController : Controller
    {
        private readonly IAvaliarService avaliarService;

        public AvaliarController(IAvaliarService avaliarService)
        {
            this.avaliarService = avaliarService;
        }

        // GET: AvaliarController
        public ActionResult Index()
        {
            var avaliacoes = avaliarService.GetAll();
            var avaliacoesViewModel = AvaliarMapper.ToViewModel(avaliacoes);
            return View(avaliacoesViewModel);
        }

        // GET: AvaliarController/Details/5
        public ActionResult Details(int id)
        {
            var avaliacao = avaliarService.Get(id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            var avaliarViewModel = AvaliarMapper.ToViewModel(avaliacao);
            return View(avaliarViewModel);
        }

        // GET: AvaliarController/Create
        public ActionResult Create(int idAgendamento = 0)
        {
            var avaliarViewModel = new AvaliarViewModel
            {
                IdAgendamento = idAgendamento
            };
            return View(avaliarViewModel);
        }

        // POST: AvaliarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AvaliarViewModel avaliarViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var avaliacao = AvaliarMapper.ToEntity(avaliarViewModel);
                    avaliarService.Create(avaliacao);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is KeyNotFoundException)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(avaliarViewModel);
        }

        // GET: AvaliarController/Edit/5
        public ActionResult Edit(int id)
        {
            var avaliacao = avaliarService.Get(id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            var avaliarViewModel = AvaliarMapper.ToViewModel(avaliacao);
            return View(avaliarViewModel);
        }

        // POST: AvaliarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AvaliarViewModel avaliarViewModel)
        {
            if (id != avaliarViewModel.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var avaliacao = AvaliarMapper.ToEntity(avaliarViewModel);
                    avaliarService.Edit(avaliacao);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is KeyNotFoundException)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(avaliarViewModel);
        }

        // GET: AvaliarController/Delete/5
        public ActionResult Delete(int id)
        {
            var avaliacao = avaliarService.Get(id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            var avaliarViewModel = AvaliarMapper.ToViewModel(avaliacao);
            return View(avaliarViewModel);
        }

        // POST: AvaliarController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public ActionResult DeleteConfirmed(int id)
        {
            var avaliacao = avaliarService.Get(id);
            if (avaliacao == null)
            {
                return NotFound();
            }

            avaliarService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

