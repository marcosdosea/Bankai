using BankaiWeb.Mappers;
using BankaiWeb.Models;
using Core;
using Core.DTO;
using Core.Service;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
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
                .Returns((Agendamento)null!);

            agendamentoServiceMock
                .Setup(service => service.ObterDataNaSemanaAtual("QIF"))
                .Returns(new DateTime(2026, 11, 19));

            controller = new AgendamentoController(agendamentoServiceMock.Object, mapper);
        }
        [TestCleanup]
        public void Limpar()
        {
            agendamentoServiceMock?.Reset();
        }

        [TestMethod]
        public void Mapper_DeveMapearViewModelParaAgendamento()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(typeof(AgendamentoMapper).Assembly);
            var mapper = new Mapper(config);

            var modelo = CriarModeloDeAgendamentoExistente();

            var agendamento = mapper.Map<Agendamento>(modelo);

            Assert.IsNotNull(agendamento);

            Assert.AreEqual(modelo.IdCliente, agendamento.IdCliente);
            Assert.AreEqual(modelo.IdProfissional, agendamento.IdProfissional);
        }

        [TestMethod]
        public void Mapper_DeveMapearDiaSemanaDaDataParaViewModel()
        {
            var config = new TypeAdapterConfig();
            config.Scan(typeof(AgendamentoMapper).Assembly);
            var mapper = new Mapper(config);

            var agendamento = ObterAgendamentoAlvo();
            agendamento.Data = new DateTime(2026, 11, 19); // quinta-feira

            var viewModel = mapper.Map<AgendamentoViewModel>(agendamento);

            Assert.AreEqual("QIF", viewModel.DiaSemana);
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
        public async Task Create_QuandoModeloValido_DeveConverterDiaSemanaEmDataEMapearHorarios()
        {
            var dataEsperada = new DateTime(2026, 11, 19);
            var modelo = CriarModeloDeNovoAgendamento();
            Agendamento? criado = null;

            agendamentoServiceMock
                .Setup(service => service.Create(It.IsAny<Agendamento>()))
                .Callback<Agendamento>(a => criado = a)
                .ReturnsAsync(10);

            var resultado = await controller.Create(modelo);

            VerificarRedirecionamentoParaIndex(resultado);
            Assert.IsNotNull(criado);
            Assert.AreEqual(dataEsperada, criado.Data);
            Assert.AreEqual(dataEsperada.AddHours(8), criado.HorarioInicio);
            Assert.AreEqual(dataEsperada.AddHours(13), criado.HorarioFim);

            agendamentoServiceMock.Verify(s => s.ObterDataNaSemanaAtual("QIF"), Times.Once);
        }

        [TestMethod]
        public async Task Create_QuandoModelStateInvalido_DeveRetornarViewSemCriar()
        {
            controller.ModelState.AddModelError("IdCliente", "O cliente é obrigatório");

            var resultado = await controller.Create(CriarModeloDeNovoAgendamento());

            Assert.IsInstanceOfType<ViewResult>(resultado);
            agendamentoServiceMock.Verify(s => s.Create(It.IsAny<Agendamento>()), Times.Never);
        }

        [TestMethod]
        public async Task Create_QuandoServiceLancaRegraDeNegocio_DeveRetornarViewComErro()
        {
            agendamentoServiceMock
                .Setup(s => s.Create(It.IsAny<Agendamento>()))
                .ThrowsAsync(new InvalidOperationException("Profissional indisponível neste dia/horário."));

            var resultado = await controller.Create(CriarModeloDeNovoAgendamento());

            Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        [TestMethod]
        public async Task Create_QuandoEnviaFoto_DeveGravarBytesNaEntidade()
        {
            var bytes = new byte[] { 1, 2, 3 };
            var modelo = CriarModeloDeNovoAgendamento();

            var stream = new MemoryStream(bytes);
            modelo.FotoUpload = new FormFile(stream, 0, bytes.Length, "FotoUpload", "foto.png");
            Agendamento? criado = null;

            agendamentoServiceMock
                .Setup(s => s.Create(It.IsAny<Agendamento>()))
                .Callback<Agendamento>(a => criado = a)
                .ReturnsAsync(10);

            Assert.IsNotNull(modelo.FotoUpload, "FotoUpload está nulo antes do Create");
            Assert.AreEqual(3, modelo.FotoUpload.Length, "FormFile com tamanho errado");

            await controller.Create(modelo);


            Assert.IsNotNull(modelo.Foto, "O controller não preencheu Foto no ViewModel");
            Assert.IsNotNull(criado);
            Assert.IsNotNull(criado.Foto, "O mapper não copiou Foto para a entidade");
        }

        [TestMethod]
        public void Edit_QuandoServiceLancaRegraDeNegocio_DeveRetornarViewComErro()
        {
            agendamentoServiceMock
                .Setup(s => s.Edit(It.IsAny<Agendamento>()))
                .Throws(new InvalidOperationException("Só é possível agendar para a semana atual."));

            var resultado = controller.Edit(1, CriarModeloDeAgendamentoExistente());

            Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.IsFalse(controller.ModelState.IsValid);
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
        public void Edit_QuandoAgendamentoNaoExiste_DeveRetornarNotFound()
        {
            Assert.IsInstanceOfType<NotFoundResult>(controller.Edit(999));
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
        public void Delete_QuandoAgendamentoNaoExiste_DeveRetornarNotFound()
        {
            Assert.IsInstanceOfType<NotFoundResult>(controller.Delete(999));
        }

        [TestMethod]
        public void Details_QuandoAgendamentoNaoExiste_DeveRetornarNotFound()
        {
            Assert.IsInstanceOfType<NotFoundResult>(controller.Details(999));
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
                DiaSemana = "QIF",
                HorarioInicio = TimeSpan.FromHours(8),
                HorarioFim = TimeSpan.FromHours(13)
            };
        }

        private static AgendamentoViewModel CriarModeloDeAgendamentoExistente()
        {
            return new AgendamentoViewModel
            {
                IdCliente = 3,
                IdProfissional = 5,
                TipoCobranca = "F",
                DiaSemana = "QIF",
                HorarioInicio = TimeSpan.FromHours(13),
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