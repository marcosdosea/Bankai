using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankaiWeb.Controllers.Tests;

[TestClass]
public class OrcamentoControllerTests
{
    private Mock<IOrcamentoService> orcamentoServiceMock = null!;
    private OrcamentoController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        orcamentoServiceMock = new Mock<IOrcamentoService>();

        orcamentoServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterOrcamentosDeTeste());

        orcamentoServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterOrcamentoAlvo());

        orcamentoServiceMock
            .Setup(service => service.Get(2))
            .Returns(ObterOrcamentoExistente());

        orcamentoServiceMock
            .Setup(service => service.Get(999))
            .Returns((Orcamento?)null);

        controller = new OrcamentoController(orcamentoServiceMock.Object);
    }

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodosOsOrcamentos()
    {
        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var orcamentos = Assert.IsInstanceOfType<IEnumerable<OrcamentoViewModel>>(
            viewResult.ViewData.Model);

        Assert.AreEqual(3, orcamentos.Count());

        var primeiroOrcamento = orcamentos.First();

        Assert.AreEqual(500.00m, primeiroOrcamento.ValorProposto);
        Assert.AreEqual((uint)1, primeiroOrcamento.Id);

        orcamentoServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemOrcamentos_DeveRetornarViewComColecaoVazia()
    {
        // Arrange
        orcamentoServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Orcamento>());

        // Act
        var resultado = controller.Index();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var orcamentos = Assert.IsInstanceOfType<IEnumerable<OrcamentoViewModel>>(
            viewResult.ViewData.Model);

        Assert.IsEmpty(orcamentos);

        orcamentoServiceMock.Verify(
            service => service.GetAll(),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoOrcamentoExiste_DeveRetornarViewComOrcamento()
    {
        // Act
        var resultado = controller.Details(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var orcamento = Assert.IsInstanceOfType<OrcamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual((uint)1, orcamento.Id);
        Assert.AreEqual(500.00m, orcamento.ValorProposto);
        Assert.AreEqual(7, orcamento.Validade);
        Assert.AreEqual("Orçamento para reforma de quadro residencial", orcamento.Descricao);

        orcamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Details_QuandoOrcamentoNaoExiste_DeveRetornarViewComModeloVazio()
    {
        // Act
        var resultado = controller.Details(999);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsNull(viewResult.ViewData.Model);

        orcamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        // Act
        var resultado = controller.Create();

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsInstanceOfType<OrcamentoViewModel>(viewResult.ViewData.Model);
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarOrcamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeNovoOrcamento();
        Orcamento? orcamentoCriado = null;
        uint idAgendamentoVinculado = 0;

        orcamentoServiceMock
            .Setup(service => service.Create(It.IsAny<Orcamento>(), It.IsAny<uint>()))
            .Callback<Orcamento, uint>((orcamento, idAgendamento) =>
            {
                orcamentoCriado = orcamento;
                idAgendamentoVinculado = idAgendamento;
            });

        // Act
        var resultado = controller.Create(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(orcamentoCriado);
        Assert.AreEqual(modelo.ValorProposto, orcamentoCriado.ValorProposto);
        Assert.AreEqual(modelo.Validade, orcamentoCriado.Validade);
        Assert.AreEqual(modelo.Descricao, orcamentoCriado.Descricao);
        Assert.AreEqual(modelo.IdAgendamento, idAgendamentoVinculado);

        orcamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Orcamento>(), modelo.IdAgendamento),
            Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarOrcamento()
    {
        // Arrange
        controller.ModelState.AddModelError(
            nameof(OrcamentoViewModel.ValorProposto),
            "O valor proposto é obrigatório.");

        // Act
        var resultado = controller.Create(CriarModeloDeNovoOrcamento());

        // Assert
        Assert.IsInstanceOfType<ViewResult>(resultado);

        orcamentoServiceMock.Verify(
            service => service.Create(It.IsAny<Orcamento>(), It.IsAny<uint>()),
            Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEOrcamentoExiste_DeveRetornarViewComOrcamento()
    {
        // Act
        var resultado = controller.Edit(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var orcamento = Assert.IsInstanceOfType<OrcamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(500.00m, orcamento.ValorProposto);
        Assert.AreEqual(7, orcamento.Validade);
        Assert.AreEqual("Orçamento para reforma de quadro residencial", orcamento.Descricao);

        orcamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEOrcamentoNaoExiste_DeveRetornarViewComModeloVazio()
    {
        // Act
        var resultado = controller.Edit(999);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsNull(viewResult.ViewData.Model);

        orcamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarOrcamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeOrcamentoExistente();
        Orcamento? orcamentoEditado = null;

        orcamentoServiceMock
            .Setup(service => service.Edit(It.IsAny<Orcamento>()))
            .Callback<Orcamento>(orcamento => orcamentoEditado = orcamento);

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(orcamentoEditado);
        Assert.AreEqual((int)modelo.Id, orcamentoEditado.Id);
        Assert.AreEqual(modelo.ValorProposto, orcamentoEditado.ValorProposto);
        Assert.AreEqual(modelo.Validade, orcamentoEditado.Validade);
        Assert.AreEqual(modelo.Descricao, orcamentoEditado.Descricao);

        orcamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Orcamento>()),
            Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarOrcamento()
    {
        var modelo = CriarModeloDeOrcamentoExistente();

        // Arrange
        controller.ModelState.AddModelError(
            nameof(OrcamentoViewModel.Descricao),
            "A descrição é obrigatória.");

        // Act
        var resultado = controller.Edit(modelo.Id, modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        orcamentoServiceMock.Verify(
            service => service.Edit(It.IsAny<Orcamento>()),
            Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEOrcamentoExiste_DeveRetornarViewComOrcamento()
    {
        // Act
        var resultado = controller.Delete(1);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var orcamento = Assert.IsInstanceOfType<OrcamentoViewModel>(
            viewResult.ViewData.Model);

        Assert.AreEqual(500.00m, orcamento.ValorProposto);
        Assert.AreEqual("Orçamento para reforma de quadro residencial", orcamento.Descricao);

        orcamentoServiceMock.Verify(
            service => service.Get(1),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEOrcamentoNaoExiste_DeveRetornarViewComModeloVazio()
    {
        // Act
        var resultado = controller.Delete(999);

        // Assert
        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsNull(viewResult.ViewData.Model);

        orcamentoServiceMock.Verify(
            service => service.Get(999),
            Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirOrcamentoERedirecionarParaIndex()
    {
        // Arrange
        var modelo = CriarModeloDeOrcamentoExistente();

        // Act
        var resultado = controller.Delete(modelo);

        // Assert
        VerificarRedirecionamentoParaIndex(resultado);

        orcamentoServiceMock.Verify(
            service => service.Delete(modelo.Id),
            Times.Once);
    }

    private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
    {
        var redirectResult =
            Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(
            nameof(OrcamentoController.Index),
            redirectResult.ActionName);
    }

    private static OrcamentoViewModel CriarModeloDeNovoOrcamento()
    {
        return new OrcamentoViewModel
        {
            Id = 1,
            IdAgendamento = 1,
            ValorProposto = 500.00m,
            Validade = 7,
            Descricao = "Orçamento para reforma de quadro residencial",
            MateriaisAdicionais = null
        };
    }

    private static OrcamentoViewModel CriarModeloDeOrcamentoExistente()
    {
        return new OrcamentoViewModel
        {
            Id = 2,
            IdAgendamento = 1,
            ValorProposto = 600.00m,
            Validade = 14,
            Descricao = "Orçamento para instalação de iluminação",
            MateriaisAdicionais = null
        };
    }

    private static Orcamento ObterOrcamentoAlvo()
    {
        return new Orcamento
        {
            Id = 1,
            ValorProposto = 500.00m,
            Validade = 7,
            Descricao = "Orçamento para reforma de quadro residencial",
            MateriaisAdicionais = null
        };
    }

    private static Orcamento ObterOrcamentoExistente()
    {
        return new Orcamento
        {
            Id = 2,
            ValorProposto = 600.00m,
            Validade = 14,
            Descricao = "Orçamento para instalação de iluminação",
            MateriaisAdicionais = null
        };
    }

    private static IEnumerable<Orcamento> ObterOrcamentosDeTeste()
    {
        return
        [
            new Orcamento
            {
                Id = 1,
                ValorProposto = 500.00m,
                Validade = 7,
                Descricao = "Orçamento para reforma de quadro residencial",
                MateriaisAdicionais = null
            },
            new Orcamento
            {
                Id = 2,
                ValorProposto = 600.00m,
                Validade = 14,
                Descricao = "Orçamento para instalação de iluminação",
                MateriaisAdicionais = null
            },
            new Orcamento
            {
                Id = 3,
                ValorProposto = 700.00m,
                Validade = 5,
                Descricao = "Orçamento para reparo de tomada",
                MateriaisAdicionais = null
            }
        ];
    }
}