using BankaiWeb.Models;
using Core;
using Core.DTO;
using Core.Service;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankaiWeb.Controllers.Tests
{

    [TestClass]
    public class AgendamentoControllerTests
    {
        private Mock<IAgendamentoService> agendamentoServiceMock = null!;
        private AgendamentoController controller = null!;

        [TestInitialize]
        public void Inicializar()
        {
            var config = new TypeAdapterConfig();
            config.Scan(typeof(AgendamentoViewModel).Assembly);
            var mapper = new Mapper(config);

            agendamentoServiceMock = new Mock<IAgendamentoService>();

            agendamentoServiceMock
                .Setup(service => service.GetAllByUser(3))
                .Returns(ObterAgendamentosDtoDeTeste());

            agendamentoServiceMock
                .Setup(service => service.Get(1))
                .Returns(ObterAgendamentoAlvo());

            agendamentoServiceMock
                .Setup(service => service.Get(999))
                .Returns(ObterAgendamentoAlvo());

            agendamentoServiceMock
                .Setup(service => service.Create(It.IsAny<Agendamento>()))
                .ReturnsAsync(10);

            controller = new AgendamentoController(agendamentoServiceMock.Object, mapper);
        }

        [TestMethod]
        public void Index_QuandoPessoaTemAgendamentos_DeveRetornarViewComLista()
        {
            // Act
            var resultado = controller.Index(3);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var agendamentos = Assert.IsInstanceOfType<IEnumerable<AgendamentoDTO>>(
                viewResult.ViewData.Model);

            Assert.HasCount(2, agendamentos);

            agendamentoServiceMock.Verify(service => service.GetAllByUser(3), Times.Once);
        }

        [TestMethod]
        public void Details_QuandoAgendamentoExiste_DeveRetornarViewComViewModel()
        {
            // Act
            var resultado = controller.Details(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var agendamento = Assert.IsInstanceOfType<AgendamentoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, agendamento.Id);
            Assert.AreEqual("Serviço 1", agendamento.DescricaoServico);

            agendamentoServiceMock.Verify(service => service.Get(1), Times.Once);
        }

        [TestMethod]
        public void Create_QuandoRequisicaoGet_DeveRetornarView()
        {
            // Act
            var resultado = controller.Create();

            // Assert
            Assert.IsInstanceOfType<ViewResult>(resultado);
        }

        [TestMethod]
        public void Create_QuandoModeloValido_DeveCriarAgendamentoERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeNovoAgendamento();
            Agendamento? agendamentoCriado = null;

            agendamentoServiceMock
                .Setup(service => service.Create(It.IsAny<Agendamento>()))
                .Callback<Agendamento>(agendamento => agendamentoCriado = agendamento)
                .ReturnsAsync(10);

            // Act
            var resultado = controller.Create(modelo).GetAwaiter().GetResult();

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            Assert.IsNotNull(agendamentoCriado);
            Assert.AreEqual(modelo.IdCliente, agendamentoCriado.IdCliente);
            Assert.AreEqual(modelo.IdProfissional, agendamentoCriado.IdProfissional);
            Assert.AreEqual(modelo.DescricaoServico, agendamentoCriado.DescricaoServico);

            agendamentoServiceMock.Verify(
                service => service.Create(It.IsAny<Agendamento>()), Times.Once);
        }

        [TestMethod]
        public void Create_QuandoModeloInvalido_DeveRetornarViewENaoCriarAgendamento()
        {
            // Arrange
            controller.ModelState.AddModelError(
                nameof(AgendamentoViewModel.DescricaoServico),
                "A descrição do serviço é obrigatória");

            // Act
            var resultado = controller.Create(CriarModeloDeNovoAgendamento()).GetAwaiter().GetResult();

            // Assert
            Assert.IsInstanceOfType<ViewResult>(resultado);

            agendamentoServiceMock.Verify(
                service => service.Create(It.IsAny<Agendamento>()), Times.Never);
        }

        [TestMethod]
        public void Edit_QuandoRequisicaoGet_DeveRetornarViewComViewModel()
        {
            // Act
            var resultado = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var agendamento = Assert.IsInstanceOfType<AgendamentoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, agendamento.Id);
            Assert.AreEqual("Serviço 1", agendamento.DescricaoServico);
        }

        [TestMethod]
        public void Edit_QuandoModeloValido_DeveAtualizarERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeAgendamentoExistente();
            Agendamento? agendamentoEditado = null;

            agendamentoServiceMock
                .Setup(service => service.Edit(It.IsAny<Agendamento>()))
                .Callback<Agendamento>(agendamento => agendamentoEditado = agendamento);

            // Act
            var resultado = controller.Edit(1, modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            Assert.IsNotNull(agendamentoEditado);
            Assert.AreEqual(1, agendamentoEditado.Id);
            Assert.AreEqual(modelo.DescricaoServico, agendamentoEditado.DescricaoServico);

            agendamentoServiceMock.Verify(
                service => service.Edit(It.IsAny<Agendamento>()), Times.Once);
        }

        [TestMethod]
        public void Edit_QuandoModeloInvalido_DeveRetornarViewENaoAtualizar()
        {
            // Arrange
            controller.ModelState.AddModelError(
                nameof(AgendamentoViewModel.DescricaoServico),
                "A descrição do serviço é obrigatória");

            // Act
            var resultado = controller.Edit(1, CriarModeloDeAgendamentoExistente());

            // Assert
            Assert.IsInstanceOfType<ViewResult>(resultado);

            agendamentoServiceMock.Verify(
                service => service.Edit(It.IsAny<Agendamento>()), Times.Never);
        }

        [TestMethod]
        public void Delete_QuandoRequisicaoGet_DeveRetornarViewComViewModel()
        {
            // Act
            var resultado = controller.Delete(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var agendamento = Assert.IsInstanceOfType<AgendamentoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, agendamento.Id);
        }

        [TestMethod]
        public void Delete_QuandoRequisicaoPost_DeveCancelarERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeAgendamentoExistente();

            // Act
            var resultado = controller.Delete(1, modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            agendamentoServiceMock.Verify(service => service.Delete(1), Times.Once);
        }

        [TestMethod]
        public void Avaliar_QuandoRequisicaoGet_DeveRetornarViewComViewModel()
        {
            // Act
            var resultado = controller.Avaliar(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.IsInstanceOfType<AgendamentoViewModel>(viewResult.ViewData.Model);
        }

        [TestMethod]
        public void Avaliar_QuandoNotaInformada_DeveAvaliarERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeAgendamentoExistente();
            modelo.Nota = "B";
            modelo.Comentario = "Bom atendimento";

            // Act
            var resultado = controller.Avaliar(1, modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            agendamentoServiceMock.Verify(
                service => service.Avaliar(1, "B", "Bom atendimento"), Times.Once);
        }

        [TestMethod]
        public void Avaliar_QuandoNotaVazia_DeveRetornarViewENaoAvaliar()
        {
            // Arrange
            var modelo = CriarModeloDeAgendamentoExistente();
            modelo.Nota = null;

            // Act
            var resultado = controller.Avaliar(1, modelo);

            // Assert
            Assert.IsInstanceOfType<ViewResult>(resultado);

            agendamentoServiceMock.Verify(
                service => service.Avaliar(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string?>()),
                Times.Never);
        }

        private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
        {
            var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

            Assert.IsNull(redirectResult.ControllerName);
            Assert.AreEqual(nameof(AgendamentoController.Index), redirectResult.ActionName);
        }

        private static AgendamentoViewModel CriarModeloDeNovoAgendamento()
        {
            return new AgendamentoViewModel
            {
                IdCliente = 8,
                IdProfissional = 7,
                DescricaoServico = "Serviço 4",
                TipoCobranca = "F",
                DiaSemana = "S",
                HorarioInicio = TimeSpan.FromHours(9),
                HorarioFim = TimeSpan.FromHours(10)
            };
        }

        private static AgendamentoViewModel CriarModeloDeAgendamentoExistente()
        {
            return new AgendamentoViewModel
            {
                Id = 1,
                IdCliente = 3,
                IdProfissional = 5,
                DescricaoServico = "Serviço 1",
                TipoCobranca = "F",
                DiaSemana = "QF",
                HorarioInicio = TimeSpan.FromHours(10),
                HorarioFim = TimeSpan.FromHours(15)
            };
        }

        private static Agendamento ObterAgendamentoAlvo()
        {
            return new Agendamento
            {
                Id = 1,
                IdCliente = 3,
                IdProfissional = 5,
                DescricaoServico = "Serviço 1",
                TipoCobranca = "F",
                DiaSemana = "QF",
                HorarioInicio = TimeSpan.FromHours(10),
                HorarioFim = TimeSpan.FromHours(15),
                StatusAgendamento = "E"
            };
        }

        private static IEnumerable<AgendamentoDTO> ObterAgendamentosDtoDeTeste()
        {
            return
            [
                new AgendamentoDTO { Id = 1, IdCliente = 3, IdProfissional = 5, StatusAgendamento = "E" },
            new AgendamentoDTO { Id = 3, IdCliente = 2, IdProfissional = 3, StatusAgendamento = "E" }
            ];
        }
    }
}