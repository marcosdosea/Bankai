using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankaiWeb.Controllers.Tests
{
    [TestClass]
    public class AvaliarControllerTest
    {
        private Mock<IAvaliarService> avaliarServiceMock = null!;
        private AvaliarController controller = null!;

        [TestInitialize]
        public void Inicializar()
        {
            avaliarServiceMock = new Mock<IAvaliarService>();

            avaliarServiceMock
                .Setup(service => service.GetAll())
                .Returns(ObterAvaliacoesDeTeste());

            avaliarServiceMock
                .Setup(service => service.Get(1))
                .Returns(ObterAvaliacaoAlvo());

            avaliarServiceMock
                .Setup(service => service.Get(2))
                .Returns(ObterSegundaAvaliacao());

            avaliarServiceMock
                .Setup(service => service.Get(999))
                .Returns((Avaliacao?)null);

            controller = new AvaliarController(avaliarServiceMock.Object);
        }

        [TestMethod]
        public void Index_QuandoChamado_DeveRetornarViewComListaDeAvaliacoes()
        {
            // Act
            var resultado = controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var avaliacoes = Assert.IsInstanceOfType<IEnumerable<AvaliarViewModel>>(
                viewResult.ViewData.Model);

            Assert.AreEqual(2, avaliacoes.Count());

            var primeiraAvaliacao = avaliacoes.First();
            Assert.AreEqual(1, primeiraAvaliacao.Id);
            Assert.AreEqual(5, primeiraAvaliacao.Pontualidade);
            Assert.AreEqual(4, primeiraAvaliacao.LimpezaLocal);
            Assert.AreEqual(5, primeiraAvaliacao.QualidadeReparo);
            Assert.AreEqual("Excelente atendimento.", primeiraAvaliacao.Depoimento);
            Assert.AreEqual(10, primeiraAvaliacao.IdAgendamento);

            avaliarServiceMock.Verify(service => service.GetAll(), Times.Once);
        }

        [TestMethod]
        public void Index_QuandoNaoExistemAvaliacoes_DeveRetornarViewComColecaoVazia()
        {
            // Arrange
            avaliarServiceMock
                .Setup(service => service.GetAll())
                .Returns(Enumerable.Empty<Avaliacao>());

            // Act
            var resultado = controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var avaliacoes = Assert.IsInstanceOfType<IEnumerable<AvaliarViewModel>>(
                viewResult.ViewData.Model);

            Assert.IsFalse(avaliacoes.Any());

            avaliarServiceMock.Verify(service => service.GetAll(), Times.Once);
        }

        [TestMethod]
        public void Details_QuandoAvaliacaoExiste_DeveRetornarViewComAvaliacao()
        {
            // Act
            var resultado = controller.Details(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var avaliacao = Assert.IsInstanceOfType<AvaliarViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, avaliacao.Id);
            Assert.AreEqual(5, avaliacao.Pontualidade);
            Assert.AreEqual(4, avaliacao.LimpezaLocal);
            Assert.AreEqual(5, avaliacao.QualidadeReparo);
            Assert.AreEqual("Excelente atendimento.", avaliacao.Depoimento);
            Assert.AreEqual(10, avaliacao.IdAgendamento);

            avaliarServiceMock.Verify(service => service.Get(1), Times.Once);
        }

        [TestMethod]
        public void Details_QuandoAvaliacaoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Details(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            avaliarServiceMock.Verify(service => service.Get(999), Times.Once);
        }

        [TestMethod]
        public void Create_QuandoRequisicaoGet_DeveRetornarViewComNovoModelo()
        {
            // Act
            var resultado = controller.Create(15);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var modelo = Assert.IsInstanceOfType<AvaliarViewModel>(viewResult.ViewData.Model);
            Assert.AreEqual(0, modelo.Id);
            Assert.AreEqual(15, modelo.IdAgendamento);
        }

        [TestMethod]
        public void Create_QuandoModeloValido_DeveCriarAvaliacaoERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeNovaAvaliacao();
            Avaliacao? avaliacaoCriada = null;

            avaliarServiceMock
                .Setup(service => service.Create(It.IsAny<Avaliacao>()))
                .Callback<Avaliacao>(avaliacao => avaliacaoCriada = avaliacao)
                .Returns(3);

            // Act
            var resultado = controller.Create(modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            Assert.IsNotNull(avaliacaoCriada);
            Assert.AreEqual(modelo.Pontualidade, avaliacaoCriada.Pontualidade);
            Assert.AreEqual(modelo.LimpezaLocal, avaliacaoCriada.LimpezaLocal);
            Assert.AreEqual(modelo.QualidadeReparo, avaliacaoCriada.QualidadeReparo);
            Assert.AreEqual(modelo.Depoimento, avaliacaoCriada.Depoimento);
            Assert.AreEqual(modelo.IdAgendamento, avaliacaoCriada.IdAgendamento);

            avaliarServiceMock.Verify(service => service.Create(It.IsAny<Avaliacao>()), Times.Once);
        }

        [TestMethod]
        public void Create_QuandoModeloInvalido_NaoDeveCriarAvaliacaoERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeNovaAvaliacao();
            controller.ModelState.AddModelError(
                nameof(AvaliarViewModel.Pontualidade),
                "Por favor, selecione uma nota para avaliar o serviço.");

            // Act
            var resultado = controller.Create(modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.AreEqual(modelo, viewResult.ViewData.Model);

            avaliarServiceMock.Verify(service => service.Create(It.IsAny<Avaliacao>()), Times.Never);
        }

        [TestMethod]
        public void Create_QuandoServicoLancaExcecaoDeNegocio_DeveAdicionarErroAoModelStateERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeNovaAvaliacao();
            avaliarServiceMock
                .Setup(service => service.Create(It.IsAny<Avaliacao>()))
                .Throws(new InvalidOperationException("Este agendamento já foi avaliado."));

            // Act
            var resultado = controller.Create(modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.AreEqual(modelo, viewResult.ViewData.Model);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState[string.Empty]!.Errors
                .Any(e => e.ErrorMessage == "Este agendamento já foi avaliado."));
        }

        [TestMethod]
        public void Edit_QuandoRequisicaoGetEAvaliacaoExiste_DeveRetornarViewComAvaliacao()
        {
            // Act
            var resultado = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var avaliacao = Assert.IsInstanceOfType<AvaliarViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, avaliacao.Id);
            Assert.AreEqual(5, avaliacao.Pontualidade);
            Assert.AreEqual("Excelente atendimento.", avaliacao.Depoimento);

            avaliarServiceMock.Verify(service => service.Get(1), Times.Once);
        }

        [TestMethod]
        public void Edit_QuandoRequisicaoGetEAvaliacaoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Edit(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            avaliarServiceMock.Verify(service => service.Get(999), Times.Once);
        }

        [TestMethod]
        public void Edit_QuandoIdDivergente_DeveRetornarBadRequest()
        {
            // Arrange
            var modelo = CriarModeloDeAvaliacaoExistente();

            // Act
            var resultado = controller.Edit(999, modelo);

            // Assert
            Assert.IsInstanceOfType<BadRequestResult>(resultado);
            avaliarServiceMock.Verify(service => service.Edit(It.IsAny<Avaliacao>()), Times.Never);
        }

        [TestMethod]
        public void Edit_QuandoModeloValido_DeveAtualizarAvaliacaoERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeAvaliacaoExistente();
            Avaliacao? avaliacaoEditada = null;

            avaliarServiceMock
                .Setup(service => service.Edit(It.IsAny<Avaliacao>()))
                .Callback<Avaliacao>(avaliacao => avaliacaoEditada = avaliacao);

            // Act
            var resultado = controller.Edit(modelo.Id, modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            Assert.IsNotNull(avaliacaoEditada);
            Assert.AreEqual(modelo.Id, avaliacaoEditada.Id);
            Assert.AreEqual(modelo.Pontualidade, avaliacaoEditada.Pontualidade);
            Assert.AreEqual(modelo.LimpezaLocal, avaliacaoEditada.LimpezaLocal);
            Assert.AreEqual(modelo.QualidadeReparo, avaliacaoEditada.QualidadeReparo);
            Assert.AreEqual(modelo.Depoimento, avaliacaoEditada.Depoimento);
            Assert.AreEqual(modelo.IdAgendamento, avaliacaoEditada.IdAgendamento);

            avaliarServiceMock.Verify(service => service.Edit(It.IsAny<Avaliacao>()), Times.Once);
        }

        [TestMethod]
        public void Edit_QuandoModeloInvalido_NaoDeveAtualizarAvaliacaoERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeAvaliacaoExistente();
            controller.ModelState.AddModelError(
                nameof(AvaliarViewModel.QualidadeReparo),
                "Por favor, selecione uma nota para avaliar o serviço.");

            // Act
            var resultado = controller.Edit(modelo.Id, modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.AreEqual(modelo, viewResult.ViewData.Model);

            avaliarServiceMock.Verify(service => service.Edit(It.IsAny<Avaliacao>()), Times.Never);
        }

        [TestMethod]
        public void Edit_QuandoServicoLancaExcecaoDeNegocio_DeveAdicionarErroAoModelStateERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeAvaliacaoExistente();
            avaliarServiceMock
                .Setup(service => service.Edit(It.IsAny<Avaliacao>()))
                .Throws(new KeyNotFoundException("Avaliação não encontrada."));

            // Act
            var resultado = controller.Edit(modelo.Id, modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.AreEqual(modelo, viewResult.ViewData.Model);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsTrue(controller.ModelState[string.Empty]!.Errors
                .Any(e => e.ErrorMessage == "Avaliação não encontrada."));
        }

        [TestMethod]
        public void Delete_QuandoRequisicaoGetEAvaliacaoExiste_DeveRetornarViewComAvaliacao()
        {
            // Act
            var resultado = controller.Delete(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var avaliacao = Assert.IsInstanceOfType<AvaliarViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, avaliacao.Id);
            Assert.AreEqual("Excelente atendimento.", avaliacao.Depoimento);

            avaliarServiceMock.Verify(service => service.Get(1), Times.Once);
        }

        [TestMethod]
        public void Delete_QuandoRequisicaoGetEAvaliacaoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Delete(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            avaliarServiceMock.Verify(service => service.Get(999), Times.Once);
        }

        [TestMethod]
        public void DeleteConfirmed_QuandoAvaliacaoExiste_DeveExcluirERedirecionarParaIndex()
        {
            // Act
            var resultado = controller.DeleteConfirmed(1);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);
            avaliarServiceMock.Verify(service => service.Delete(1), Times.Once);
        }

        [TestMethod]
        public void DeleteConfirmed_QuandoAvaliacaoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.DeleteConfirmed(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            avaliarServiceMock.Verify(service => service.Delete(It.IsAny<int>()), Times.Never);
        }

        private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
        {
            var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
            Assert.IsNull(redirectResult.ControllerName);
            Assert.AreEqual(nameof(AvaliarController.Index), redirectResult.ActionName);
        }

        private static AvaliarViewModel CriarModeloDeNovaAvaliacao()
        {
            return new AvaliarViewModel
            {
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 4,
                Depoimento = "Serviço muito bem executado.",
                IdAgendamento = 12
            };
        }

        private static AvaliarViewModel CriarModeloDeAvaliacaoExistente()
        {
            return new AvaliarViewModel
            {
                Id = 1,
                Pontualidade = 5,
                LimpezaLocal = 4,
                QualidadeReparo = 5,
                Depoimento = "Excelente atendimento.",
                IdAgendamento = 10
            };
        }

        private static Avaliacao ObterAvaliacaoAlvo()
        {
            return new Avaliacao
            {
                Id = 1,
                Pontualidade = 5,
                LimpezaLocal = 4,
                QualidadeReparo = 5,
                Depoimento = "Excelente atendimento.",
                IdAgendamento = 10
            };
        }

        private static Avaliacao ObterSegundaAvaliacao()
        {
            return new Avaliacao
            {
                Id = 2,
                Pontualidade = 4,
                LimpezaLocal = 4,
                QualidadeReparo = 4,
                Depoimento = "Bom profissional.",
                IdAgendamento = 11
            };
        }

        private static IEnumerable<Avaliacao> ObterAvaliacoesDeTeste()
        {
            return
            [
                ObterAvaliacaoAlvo(),
                ObterSegundaAvaliacao()
            ];
        }
    }
}

