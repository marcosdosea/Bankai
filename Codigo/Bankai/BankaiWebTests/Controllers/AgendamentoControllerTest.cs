using BankaiWeb.Mappers;
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

            agendamentoServiceMock.Verify(
                service => service.Create(It.IsAny<Agendamento>()), Times.Once);
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

            agendamentoServiceMock.Verify(
                service => service.Edit(It.IsAny<Agendamento>()), Times.Once);
        }

        [TestMethod]
        public void Mapper_DeveCompilarMapeamentoDeAgendamento()
        {
            var config = new TypeAdapterConfig();

            config.Scan(typeof(AgendamentoServicoProfile).Assembly);

            config.Compile(
                typeof(AgendamentoViewModel),
                typeof(Agendamento)
            );

            config.Compile(
                typeof(Agendamento),
                typeof(AgendamentoViewModel)
            );
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
                TipoCobranca = "F",
                Data = DateTime.Today.AddDays(5),
                HorarioInicio = DateTime.Today.AddHours(9),
                HorarioFim = DateTime.Today.AddHours(13)
            };
        }

        private static AgendamentoViewModel CriarModeloDeAgendamentoExistente()
        {
            return new AgendamentoViewModel
            {
                IdCliente = 3,
                IdProfissional = 5,
                TipoCobranca = "F",
                Data = DateTime.Today.AddDays(2),
                HorarioInicio = DateTime.Today.AddHours(10),
                HorarioFim = DateTime.Today.AddHours(15)
                
            };
        }

        private static Agendamento ObterAgendamentoAlvo()
        {
            return new Agendamento
            {
                Id = 1,
                IdCliente = 3,
                IdProfissional = 5,
                TipoCobranca = "F",
                Data = DateTime.Today.AddDays(3),
                HorarioInicio = DateTime.Today.AddHours(10),
                HorarioFim = DateTime.Today.AddHours(15),
            };
        }

        private static IEnumerable<AgendamentoDTO> ObterAgendamentosDtoDeTeste()
        {
            return
            [
                new AgendamentoDTO { Id = 1, IdCliente = 3, IdProfissional = 5, StatusAgendamento = "EmExecucao" },
                new AgendamentoDTO { Id = 3, IdCliente = 2, IdProfissional = 3, StatusAgendamento = "EmExecucao" }
            ];
        }
    }
}