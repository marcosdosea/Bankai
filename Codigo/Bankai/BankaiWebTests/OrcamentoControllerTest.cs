using BankaiWeb.Models;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankaiWeb.Controllers.Tests
{
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
                .Setup(service => service.Get(1, 1))
                .Returns(ObterOrcamentoAlvo());

            orcamentoServiceMock
                .Setup(service => service.Get(2, 2))
                .Returns(ObterSegundoOrcamento());

            orcamentoServiceMock
                .Setup(service => service.Get(999, 999))
                .Returns((OrcamentoDTO?)null);

            orcamentoServiceMock
                .Setup(service => service.CalcularTaxaPlataforma(It.IsAny<decimal>()))
                .Returns<decimal>(valor => valor * 0.15m);

            orcamentoServiceMock
                .Setup(service => service.CalcularValorLiquido(It.IsAny<decimal>()))
                .Returns<decimal>(valor => valor * 0.85m);

            orcamentoServiceMock
                .Setup(service => service.EnviarOrcamento(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>()))
                .Returns(true);

            controller = new OrcamentoController(orcamentoServiceMock.Object);
        }

        [TestMethod]
        public void Index_QuandoChamado_DeveRetornarViewComListaDeOrcamentos()
        {
            // Act
            var resultado = controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var orcamentos = Assert.IsInstanceOfType<IEnumerable<OrcamentoViewModel>>(
                viewResult.ViewData.Model);

            Assert.AreEqual(2, orcamentos.Count());

            var primeiroOrcamento = orcamentos.First();
            Assert.AreEqual(1, primeiroOrcamento.IdAgendamento);
            Assert.AreEqual(1, primeiroOrcamento.IdServico);
            Assert.AreEqual("Formatação de PC", primeiroOrcamento.NomeServico);
            Assert.AreEqual("Carlos Silva", primeiroOrcamento.NomeCliente);
            Assert.AreEqual(150.00m, primeiroOrcamento.ValorProposto);

            orcamentoServiceMock.Verify(service => service.GetAll(), Times.Once);
        }

        [TestMethod]
        public void Index_QuandoNaoExistemOrcamentos_DeveRetornarViewComColecaoVazia()
        {
            // Arrange
            orcamentoServiceMock
                .Setup(service => service.GetAll())
                .Returns(Enumerable.Empty<OrcamentoDTO>());

            // Act
            var resultado = controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var orcamentos = Assert.IsInstanceOfType<IEnumerable<OrcamentoViewModel>>(
                viewResult.ViewData.Model);

            Assert.IsFalse(orcamentos.Any());

            orcamentoServiceMock.Verify(service => service.GetAll(), Times.Once);
        }

        [TestMethod]
        public void Orcar_QuandoRequisicaoGetEOrcamentoExiste_DeveRetornarViewComTaxasCalculadas()
        {
            // Act
            var resultado = controller.Orcar(1, 1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var viewModel = Assert.IsInstanceOfType<OrcamentoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, viewModel.IdAgendamento);
            Assert.AreEqual(1, viewModel.IdServico);
            Assert.AreEqual(150.00m, viewModel.ValorProposto);
            Assert.AreEqual(22.50m, viewModel.TaxaPlataforma);
            Assert.AreEqual(127.50m, viewModel.ValorLiquido);

            orcamentoServiceMock.Verify(service => service.Get(1, 1), Times.Once);
            orcamentoServiceMock.Verify(service => service.CalcularTaxaPlataforma(150.00m), Times.Once);
            orcamentoServiceMock.Verify(service => service.CalcularValorLiquido(150.00m), Times.Once);
        }

        [TestMethod]
        public void Orcar_QuandoRequisicaoGetEOrcamentoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Orcar(999, 999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            orcamentoServiceMock.Verify(service => service.Get(999, 999), Times.Once);
        }

        [TestMethod]
        public void Orcar_QuandoModeloValido_DeveEnviarOrcamentoERedirecionarParaDetails()
        {
            // Arrange
            var modelo = CriarModeloDeOrcamento();

            // Act
            var resultado = controller.Orcar(modelo);

            // Assert
            VerificarRedirecionamentoParaDetails(resultado, modelo.IdAgendamento, modelo.IdServico);

            orcamentoServiceMock.Verify(
                service => service.EnviarOrcamento(modelo.IdAgendamento, modelo.IdServico, modelo.ValorProposto),
                Times.Once);
        }

        [TestMethod]
        public void Orcar_QuandoModeloInvalido_NaoDeveEnviarOrcamentoECalcularValoresERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeOrcamento();
            controller.ModelState.AddModelError(nameof(OrcamentoViewModel.ValorProposto), "O valor do orçamento é obrigatório.");

            // Act
            var resultado = controller.Orcar(modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var modeloRetornado = Assert.IsInstanceOfType<OrcamentoViewModel>(viewResult.ViewData.Model);

            Assert.AreEqual(200.00m, modeloRetornado.ValorProposto);
            Assert.AreEqual(30.00m, modeloRetornado.TaxaPlataforma);
            Assert.AreEqual(170.00m, modeloRetornado.ValorLiquido);

            orcamentoServiceMock.Verify(
                service => service.EnviarOrcamento(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<decimal>()),
                Times.Never);
            orcamentoServiceMock.Verify(service => service.CalcularTaxaPlataforma(200.00m), Times.Once);
            orcamentoServiceMock.Verify(service => service.CalcularValorLiquido(200.00m), Times.Once);
        }

        [TestMethod]
        public void Orcar_QuandoServiceLancaExcecao_DeveAdicionarErroNoModelStateERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeOrcamento();
            var mensagemErro = "Não é permitido alterar orçamento já aprovado.";

            orcamentoServiceMock
                .Setup(service => service.EnviarOrcamento(modelo.IdAgendamento, modelo.IdServico, modelo.ValorProposto))
                .Throws(new InvalidOperationException(mensagemErro));

            // Act
            var resultado = controller.Orcar(modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState[string.Empty]?.Errors.Any(e => e.ErrorMessage == mensagemErro));

            var modeloRetornado = Assert.IsInstanceOfType<OrcamentoViewModel>(viewResult.ViewData.Model);
            Assert.AreEqual(30.00m, modeloRetornado.TaxaPlataforma);
            Assert.AreEqual(170.00m, modeloRetornado.ValorLiquido);
        }

        [TestMethod]
        public void Details_QuandoOrcamentoExiste_DeveRetornarViewComViewModel()
        {
            // Act
            var resultado = controller.Details(1, 1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var viewModel = Assert.IsInstanceOfType<OrcamentoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, viewModel.IdAgendamento);
            Assert.AreEqual(1, viewModel.IdServico);
            Assert.AreEqual("Formatação de PC", viewModel.NomeServico);
            Assert.AreEqual("Carlos Silva", viewModel.NomeCliente);
            Assert.AreEqual("João Técnico", viewModel.NomeProfissional);
            Assert.AreEqual(150.00m, viewModel.ValorProposto);

            orcamentoServiceMock.Verify(service => service.Get(1, 1), Times.Once);
        }

        [TestMethod]
        public void Details_QuandoOrcamentoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Details(999, 999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            orcamentoServiceMock.Verify(service => service.Get(999, 999), Times.Once);
        }

        private static void VerificarRedirecionamentoParaDetails(IActionResult resultado, int idAgendamento, int idServico)
        {
            var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
            Assert.IsNull(redirectResult.ControllerName);
            Assert.AreEqual(nameof(OrcamentoController.Details), redirectResult.ActionName);
            Assert.IsNotNull(redirectResult.RouteValues);
            Assert.AreEqual(idAgendamento, redirectResult.RouteValues["idAgendamento"]);
            Assert.AreEqual(idServico, redirectResult.RouteValues["idServico"]);
        }

        private static OrcamentoViewModel CriarModeloDeOrcamento()
        {
            return new OrcamentoViewModel
            {
                IdAgendamento = 1,
                IdServico = 1,
                NomeServico = "Formatação de PC",
                DescricaoServico = "Computador lento reiniciando sozinho",
                NomeCliente = "Carlos Silva",
                NomeProfissional = "João Técnico",
                ValorProposto = 200.00m,
                StatusServico = "Pendente Orçamento"
            };
        }

        private static OrcamentoDTO ObterOrcamentoAlvo()
        {
            return new OrcamentoDTO
            {
                IdAgendamento = 1,
                IdServico = 1,
                NomeServico = "Formatação de PC",
                DescricaoServico = "Computador lento reiniciando sozinho",
                IdCliente = 10,
                NomeCliente = "Carlos Silva",
                IdProfissional = 20,
                NomeProfissional = "João Técnico",
                TipoCobranca = "O",
                StatusAgendamento = "A",
                StatusServico = "Pendente Orçamento",
                ValorProposto = 150.00m,
                TaxaPlataforma = 22.50m,
                ValorLiquido = 127.50m
            };
        }

        private static OrcamentoDTO ObterSegundoOrcamento()
        {
            return new OrcamentoDTO
            {
                IdAgendamento = 2,
                IdServico = 2,
                NomeServico = "Instalação de Rede Wi-Fi",
                DescricaoServico = "Configuração de roteador e repetidores",
                IdCliente = 11,
                NomeCliente = "Mariana Costa",
                IdProfissional = 20,
                NomeProfissional = "João Técnico",
                TipoCobranca = "O",
                StatusAgendamento = "A",
                StatusServico = "Orçado",
                ValorProposto = 300.00m,
                TaxaPlataforma = 45.00m,
                ValorLiquido = 255.00m
            };
        }

        private static IEnumerable<OrcamentoDTO> ObterOrcamentosDeTeste()
        {
            return
            [
                ObterOrcamentoAlvo(),
                ObterSegundoOrcamento()
            ];
        }
    }
}