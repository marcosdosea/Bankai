using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace BankaiWeb.Controllers
{
    public class PessoaController : Controller
    {
        private readonly IPessoaService _pessoaService;

        public PessoaController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        public ActionResult Index()
        {
            var pessoas = _pessoaService.GetAll();
            var pessoasViewModel = PessoaMapper.ToViewModel(pessoas);
            return View(pessoasViewModel);
        }

        public ActionResult Details(int id)
        {
            var pessoa = _pessoaService.get(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            var pessoaViewModel = PessoaMapper.ToViewModel(pessoa);
            return View(pessoaViewModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PessoaViewModel pessoaViewModel)
        {
            if (ModelState.IsValid)
            {
                var pessoa = PessoaMapper.ToEntity(pessoaViewModel);
                _pessoaService.Create(pessoa);
                return RedirectToAction(nameof(Index));
            }
            return View(pessoaViewModel);
        }

        public ActionResult Edit(int id)
        {
            var pessoa = _pessoaService.get(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            var pessoaViewModel = PessoaMapper.ToViewModel(pessoa);
            return View(pessoaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, PessoaViewModel pessoaViewModel)
        {
            if (id != pessoaViewModel.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var pessoa = PessoaMapper.ToEntity(pessoaViewModel);
                _pessoaService.Edit(pessoa);
                return RedirectToAction(nameof(Index));
            }
            return View(pessoaViewModel);
        }

        public ActionResult Delete(int id)
        {
            var pessoa = _pessoaService.get(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            var pessoaViewModel = PessoaMapper.ToViewModel(pessoa);
            return View(pessoaViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Delete))]
        public ActionResult DeleteConfirmed(int id)
        {
            var pessoa = _pessoaService.get(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            _pessoaService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}