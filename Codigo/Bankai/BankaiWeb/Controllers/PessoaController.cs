using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public ActionResult Create([FromServices] IEspecialidadeService especialidadeService)
        {
            ViewBag.Especialidades = new SelectList(especialidadeService.GetAll(), "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PessoaViewModel pessoaViewModel, [FromServices] IEspecialidadeService especialidadeService)
        {
            if (ModelState.IsValid)
            {
                var pessoa = PessoaMapper.ToEntity(pessoaViewModel);

                // Remove máscaras para caber exatamente no CHAR(14), CHAR(11) e CHAR(8) do banco
                pessoa.CpfCnpj = pessoa.CpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();
                pessoa.Telefone = (pessoa.Telefone ?? string.Empty).Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
                pessoa.Cep = (pessoa.Cep ?? string.Empty).Replace("-", "").Trim();

                // Garante strings vazias caso o banco tenha colunas NOT NULL
                pessoa.Rua ??= string.Empty;
                pessoa.Bairro ??= string.Empty;
                pessoa.Cidade ??= string.Empty;
                pessoa.Estado ??= string.Empty;
                pessoa.NumeroCasa ??= string.Empty;

                if (pessoaViewModel.ArquivoRegistroTecnico != null && pessoaViewModel.ArquivoRegistroTecnico.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    pessoaViewModel.ArquivoRegistroTecnico.CopyTo(memoryStream);
                    pessoa.RegistroTecnico = memoryStream.ToArray();
                }

                _pessoaService.Create(pessoa);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Especialidades = new SelectList(especialidadeService.GetAll(), "Id", "Nome", pessoaViewModel.IdEspecialidade);
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

                if (pessoaViewModel.ArquivoRegistroTecnico != null && pessoaViewModel.ArquivoRegistroTecnico.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    pessoaViewModel.ArquivoRegistroTecnico.CopyTo(memoryStream);
                    pessoa.RegistroTecnico = memoryStream.ToArray();
                }

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