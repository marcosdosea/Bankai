using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankaiWeb.Controllers.Tests;

[TestClass]
public class PessoaControllerTests
{
    private Mock<IPessoaService> pessoaServiceMock = null!;
    private PessoaController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        pessoaServiceMock = new Mock<IPessoaService>();

        pessoaServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterPessoasDeTeste());

        pessoaServiceMock
            .Setup(service => service.get(1))
            .Returns(ObterPessoaAlvo());

        pessoaServiceMock
            .Setup(service => service.get(999))
            .Returns((Pessoa?)null);

        controller = new PessoaController(pessoaServiceMock.Object);
    }

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodasAsPessoas()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoas = Assert.IsInstanceOfType<IEnumerable<PessoaViewModel>>(
            viewResult.ViewData.Model);

        Assert.AreEqual(3, pessoas.Count());

        var primeiraPessoa = pessoas.First();

        Assert.AreEqual(primeiraPessoa.id, primeiraPessoa.id);
        Assert.AreEqual("João Silva", primeiraPessoa.Nome);

        pessoaServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemAutores_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        pessoaServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Pessoa>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoas = Assert.IsInstanceOfType<IEnumerable<PessoaViewModel>>(
            viewResult.ViewData.Model);

        Assert.IsEmpty(pessoas);

        pessoaServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoPessoaExiste_DeveRetornarViewComPessoa()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(pessoa.id, pessoa.id);
        Assert.AreEqual("João Silva", pessoa.Nome);
        Assert.AreEqual("12345678901", pessoa.CpfCnpj);
        Assert.AreEqual("joao@email.com", pessoa.Email);

        pessoaServiceMock.Verify(
            service => service.get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoPessoaNaoExiste_DeveRetornarViewComModeloVazio()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(pessoa.id, pessoa.id);
        Assert.IsTrue(string.IsNullOrEmpty(pessoa.Nome));

        pessoaServiceMock.Verify(
            service => service.get(999),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(pessoa.id, pessoa.id);
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarPessoaERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovaPessoa();
        Pessoa? pessoaCriada = null;

        pessoaServiceMock
            .Setup(service => service.Create(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(pessoa => pessoaCriada = pessoa);

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(pessoaCriada);
        Assert.AreEqual(pessoaCriada.Id, pessoaCriada.Id);
        Assert.AreEqual(modelo.Nome, pessoaCriada.Nome);
        Assert.AreEqual(modelo.CpfCnpj, pessoaCriada.CpfCnpj);
        Assert.AreEqual(modelo.Email, pessoaCriada.Email);
        Assert.AreEqual(modelo.EhProfissional, pessoaCriada.EhProfissional);

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarPessoa()
    {
        // Arrange
        controller.ModelState.AddModelError(
            nameof(PessoaViewModel.Nome),
            "O nome é obrigatório.");

        // Act
        var resultado = controller.Create(CriarModeloDeNovaPessoa());

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        pessoaServiceMock.Verify(
            service => service.Create(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEPessoaExiste_DeveRetornarViewComPessoa()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(pessoa.id, pessoa.id);
        Assert.AreEqual("João Silva", pessoa.Nome);
        Assert.AreEqual("12345678901", pessoa.CpfCnpj);

        pessoaServiceMock.Verify(
            service => service.get(1),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEPessoaNaoExiste_DeveRetornarViewComModeloVazio()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(pessoa.id, pessoa.id);
        Assert.IsTrue(string.IsNullOrEmpty(pessoa.Nome));

        pessoaServiceMock.Verify(
            service => service.get(999),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarPessoaERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDePessoaExistente();
        Pessoa? pessoaEditada = null;

        pessoaServiceMock
            .Setup(service => service.Edit(It.IsAny<Pessoa>()))
            .Callback<Pessoa>(pessoa => pessoaEditada = pessoa);

        // Act
        var resultado = controller.Edit(modelo.id, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(pessoaEditada);
        Assert.AreEqual(pessoaEditada.Id, pessoaEditada.Id);
        Assert.AreEqual(modelo.Nome, pessoaEditada.Nome);
        Assert.AreEqual(modelo.CpfCnpj, pessoaEditada.CpfCnpj);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarPessoa()
    {
        var modelo = CriarModeloDePessoaExistente();

        // Arrange
        controller.ModelState.AddModelError(
            nameof(PessoaViewModel.Nome),
            "O nome é obrigatório.");

        // Act
        var resultado = controller.Edit(modelo.id, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        pessoaServiceMock.Verify(
            service => service.Edit(It.IsAny<Pessoa>()),
            Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEPessoaExiste_DeveRetornarViewComPessoa()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(pessoa.id, pessoa.id);
        Assert.AreEqual("João Silva", pessoa.Nome);

        pessoaServiceMock.Verify(
            service => service.get(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEPessoaNaoExiste_DeveRetornarViewComModeloVazio()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var pessoa = Assert.IsInstanceOfType<PessoaViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(pessoa.id, pessoa.id);
        Assert.IsTrue(string.IsNullOrEmpty(pessoa.Nome));

        pessoaServiceMock.Verify(
            service => service.get(999),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirPessoaERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDePessoaExistente();

        // Act
        var resultado = controller.Delete(modelo.id);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        pessoaServiceMock.Verify(
            service => service.Delete(modelo.id),
            Times.Once);
    }

    private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
    {
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(
            nameof(PessoaController.Index),
            redirectResult.ActionName);
    }

    private static PessoaViewModel CriarModeloDeNovaPessoa()
    {
        return new PessoaViewModel
        {
            id = 1,
            CpfCnpj = "12345678910",
            Nome = "João Silva",
            Email = "joaosilva@gmail.com",
            Telefone = "79123456789",
            EhProfissional = false,
            Cep = "12345678",
            Rua = "Rua A",
            Bairro = "Bairro A",
            Cidade = "Cidade A",
            Estado = "Estado A",
            NumeroCasa = "123",
        };
    }

    private static PessoaViewModel CriarModeloDePessoaExistente()
    {
        return new PessoaViewModel
        {
            id = 2,
            CpfCnpj = "98765432100",
            Nome = "Maria Souza",
            Email = "mariasouza@gmail.com",
            Telefone = "79987654321",
            EhProfissional = true,
            Cep = "87654321",
            Rua = "Rua B",
            Bairro = "Bairro B",
            Cidade = "Cidade B",
            Estado = "Estado B",
            NumeroCasa = "456",
        };
    }

    private static Pessoa ObterPessoaAlvo()
    {
        return new Pessoa
        {
            Id = 3,
            CpfCnpj = "11122233344",
            Nome = "Carlos Oliveira",
            Email = "carlosoliveira@gmail.com",
            Telefone = "79999888777",
            EhProfissional = false,
            Cep = "54321098",
            Rua = "Rua C",
            Bairro = "Bairro C",
            Cidade = "Cidade C",
            Estado = "Estado C",
            NumeroCasa = "789",
        };
    }

    private static IEnumerable<Pessoa> ObterPessoasDeTeste()
    {
        return
        [
            new Pessoa
            {
                Id = 1,
                CpfCnpj = "12345678910",
                Nome = "João Silva",
                Email = "joaosilva@gmail.com",
                Telefone = "79123456789",
                EhProfissional = false,
                Cep = "12345678",
                Rua = "Rua A",
                Bairro = "Bairro A",
                Cidade = "Cidade A",
                Estado = "Estado A",
                NumeroCasa = "123",
            },
            new Pessoa
            {
                Id = 2,
                CpfCnpj = "98765432100",
                Nome = "Maria Souza",
                Email = "mariasouza@gmail.com",
                Telefone = "79987654321",
                EhProfissional = true,
                Cep = "87654321",
                Rua = "Rua B",
                Bairro = "Bairro B",
                Cidade = "Cidade B",
                Estado = "Estado B",
                NumeroCasa = "456",
            },
            new Pessoa
            {
                Id = 3,
                CpfCnpj = "11122233344",
                Nome = "Carlos Oliveira",
                Email = "carlosoliveira@gmail.com",
                Telefone = "79999888777",
                EhProfissional = false,
                Cep = "54321098",
                Rua = "Rua C",
                Bairro = "Bairro C",
                Cidade = "Cidade C",
                Estado = "Estado C",
                NumeroCasa = "789",
            }
        ];
    }
}