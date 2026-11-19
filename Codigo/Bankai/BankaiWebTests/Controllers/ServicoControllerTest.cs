using BankaiWeb.Models;
using Core;
using Core.DTO;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankaiWeb.Controllers.Tests
{
    [TestClass]
    public class ServicoControllerTests
    {
        private Mock<IServicoService> servicoServiceMock = null!;
        private ServicoController controller = null!;

        [TestInitialize]
        public void Inicializar()
        {
            servicoServiceMock = new Mock<IServicoService>();

            servicoServiceMock
                .Setup(service => service.GetAll())
                .Returns(ObterServicosDeTeste());

            servicoServiceMock
                .Setup(service => service.Get(1))
                .Returns(ObterServicoAlvo());

            servicoServiceMock
                .Setup(service => service.Get(2))
                .Returns(ObterSegundoServico());

            servicoServiceMock
                .Setup(service => service.Get(999))
                .Returns((Servico?)null);

            servicoServiceMock
                .Setup(service => service.ObterEspecialidades())
                .Returns([new Especialidade { Id = 1, Nome = "Tecnologia" }]);

            controller = new ServicoController(servicoServiceMock.Object);
        }

        [TestMethod]
        public void Index_QuandoChamado_DeveRetornarViewComListaDeServicos()
        {
            // Act
            var resultado = controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var servicos = Assert.IsInstanceOfType<IEnumerable<ServicoViewModel>>(
                viewResult.ViewData.Model);

            Assert.AreEqual(2, servicos.Count());

            var primeiroServico = servicos.First();
            Assert.AreEqual(1, primeiroServico.Id);
            Assert.AreEqual("Formatação e Limpeza", primeiroServico.Nome);
            Assert.AreEqual("Limpeza interna e formatação completa do sistema operacional", primeiroServico.Descricao);
            Assert.AreEqual("Tecnologia", primeiroServico.NomeEspecialidade);

            servicoServiceMock.Verify(service => service.GetAll(), Times.Once);
        }

        [TestMethod]
        public void Index_QuandoNaoExistemServicos_DeveRetornarViewComColecaoVazia()
        {
            // Arrange
            servicoServiceMock
                .Setup(service => service.GetAll())
                .Returns(Enumerable.Empty<Servico>());

            // Act
            var resultado = controller.Index();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var servicos = Assert.IsInstanceOfType<IEnumerable<ServicoViewModel>>(
                viewResult.ViewData.Model);

            Assert.IsFalse(servicos.Any());

            servicoServiceMock.Verify(service => service.GetAll(), Times.Once);
        }

        [TestMethod]
        public void Details_QuandoServicoExiste_DeveRetornarViewComServico()
        {
            // Act
            var resultado = controller.Details(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var servico = Assert.IsInstanceOfType<ServicoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, servico.Id);
            Assert.AreEqual("Formatação e Limpeza", servico.Nome);
            Assert.AreEqual("Limpeza interna e formatação completa do sistema operacional", servico.Descricao);

            servicoServiceMock.Verify(service => service.Get(1), Times.Once);
        }

        [TestMethod]
        public void Details_QuandoServicoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Details(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            servicoServiceMock.Verify(service => service.Get(999), Times.Once);
        }

        [TestMethod]
        public void Create_QuandoRequisicaoGet_DeveRetornarViewComNovoModelo()
        {
            // Act
            var resultado = controller.Create();

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var modelo = Assert.IsInstanceOfType<ServicoViewModel>(viewResult.ViewData.Model);
            Assert.AreEqual(0, modelo.Id);
        }

        [TestMethod]
        public void Create_QuandoModeloValido_DeveCriarServicoERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeNovoServico();
            Servico? servicoCriado = null;

            servicoServiceMock
                .Setup(service => service.Create(It.IsAny<Servico>()))
                .Callback<Servico>(servico => servicoCriado = servico)
                .Returns(3);

            // Act
            var resultado = controller.Create(modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            Assert.IsNotNull(servicoCriado);
            Assert.AreEqual(modelo.IdEspecialidade, servicoCriado.IdEspecialidade);
            Assert.AreEqual(modelo.Nome, servicoCriado.Nome);
            Assert.AreEqual(modelo.Descricao, servicoCriado.Descricao);

            servicoServiceMock.Verify(service => service.Create(It.IsAny<Servico>()), Times.Once);
        }

        [TestMethod]
        public void Create_QuandoModeloInvalido_NaoDeveCriarServicoERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeNovoServico();
            controller.ModelState.AddModelError(nameof(ServicoViewModel.Nome), "O campo Nome é obrigatório");

            // Act
            var resultado = controller.Create(modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.AreEqual(modelo, viewResult.ViewData.Model);

            servicoServiceMock.Verify(service => service.Create(It.IsAny<Servico>()), Times.Never);
        }

        [TestMethod]
        public void Edit_QuandoRequisicaoGetEServicoExiste_DeveRetornarViewComServico()
        {
            // Act
            var resultado = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var servico = Assert.IsInstanceOfType<ServicoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, servico.Id);
            Assert.AreEqual("Formatação e Limpeza", servico.Nome);

            servicoServiceMock.Verify(service => service.Get(1), Times.Once);
        }

        [TestMethod]
        public void Edit_QuandoRequisicaoGetEServicoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Edit(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            servicoServiceMock.Verify(service => service.Get(999), Times.Once);
        }

        [TestMethod]
        public void Edit_QuandoModeloValido_DeveAtualizarServicoERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeServicoExistente();
            Servico? servicoEditado = null;

            servicoServiceMock
                .Setup(service => service.Edit(It.IsAny<Servico>()))
                .Callback<Servico>(servico => servicoEditado = servico);

            // Act
            var resultado = controller.Edit(modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            Assert.IsNotNull(servicoEditado);
            Assert.AreEqual(modelo.Id, servicoEditado.Id);
            Assert.AreEqual(modelo.IdEspecialidade, servicoEditado.IdEspecialidade);
            Assert.AreEqual(modelo.Nome, servicoEditado.Nome);
            Assert.AreEqual(modelo.Descricao, servicoEditado.Descricao);

            servicoServiceMock.Verify(service => service.Edit(It.IsAny<Servico>()), Times.Once);
        }

        [TestMethod]
        public void Edit_QuandoModeloInvalido_NaoDeveAtualizarServicoERetornarView()
        {
            // Arrange
            var modelo = CriarModeloDeServicoExistente();
            controller.ModelState.AddModelError(nameof(ServicoViewModel.Nome), "O campo Nome é obrigatório");

            // Act
            var resultado = controller.Edit(modelo);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            Assert.AreEqual(modelo, viewResult.ViewData.Model);

            servicoServiceMock.Verify(service => service.Edit(It.IsAny<Servico>()), Times.Never);
        }

        [TestMethod]
        public void Delete_QuandoRequisicaoGetEServicoExiste_DeveRetornarViewComServico()
        {
            // Act
            var resultado = controller.Delete(1);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var servico = Assert.IsInstanceOfType<ServicoViewModel>(
                viewResult.ViewData.Model);

            Assert.AreEqual(1, servico.Id);
            Assert.AreEqual("Formatação e Limpeza", servico.Nome);

            servicoServiceMock.Verify(service => service.Get(1), Times.Once);
        }

        [TestMethod]
        public void Delete_QuandoRequisicaoGetEServicoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.Delete(999);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            servicoServiceMock.Verify(service => service.Get(999), Times.Once);
        }

        [TestMethod]
        public void Delete_QuandoRequisicaoPost_DeveExcluirServicoERedirecionarParaIndex()
        {
            // Arrange
            var modelo = CriarModeloDeServicoExistente();

            // Act
            var resultado = controller.Delete(modelo);

            // Assert
            VerificarRedirecionamentoParaIndex(resultado);

            servicoServiceMock.Verify(service => service.Delete(modelo.Id), Times.Once);
        }

        [TestMethod]
        public void Buscar_QuandoChamadoComFiltros_DeveRetornarViewComServicosEEspecialidades()
        {
            // Arrange
            var data = new DateTime(2026, 11, 20);
            servicoServiceMock
                .Setup(s => s.ObterEspecialidades())
                .Returns([new Especialidade { Id = 1, Nome = "Elétrica" }]);
            servicoServiceMock
                .Setup(s => s.BuscarServicos("Chuveiro", 1, data))
                .Returns([
                    new ServicoBuscaDTO
                    {
                        Id = 1,
                        Nome = "Troca de Chuveiro",
                        Descricao = "Troca de resistência",
                        IdEspecialidade = 1,
                        NomeEspecialidade = "Elétrica",
                        QuantidadeProfissionais = 2,
                        MenorPreco = 120.00m
                    }
                ]);

            // Act
            var resultado = controller.Buscar("Chuveiro", 1, data);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var model = Assert.IsInstanceOfType<BuscarServicoViewModel>(viewResult.ViewData.Model);
            Assert.AreEqual("Chuveiro", model.Termo);
            Assert.AreEqual(1, model.IdEspecialidade);
            Assert.AreEqual(data, model.DataDesejada);
            Assert.IsInstanceOfType<IEnumerable<EspecialidadeViewModel>>(model.Especialidades);
            Assert.AreEqual(1, model.Especialidades.Count());
            Assert.AreEqual(1, model.Resultados.Count());
            servicoServiceMock.Verify(s => s.ObterEspecialidades(), Times.Once);
            servicoServiceMock.Verify(s => s.BuscarServicos("Chuveiro", 1, data), Times.Once);
        }

        [TestMethod]
        public void ProfissionaisDisponiveis_QuandoServicoExiste_DeveRetornarViewComProfissionais()
        {
            // Arrange
            var data = new DateTime(2026, 11, 20);
            servicoServiceMock
                .Setup(s => s.BuscarProfissionaisPorServico(1, data, "menor_preco", true, false, 200m))
                .Returns([
                    new ProfissionalDisponivelDTO
                    {
                        IdProfissional = 10,
                        IdServico = 1,
                        NomeProfissional = "Marcos Silva",
                        Verificado = true,
                        Preco = 120.00m,
                        NotaMedia = 4.9m,
                        TotalAvaliacoes = 15,
                        DisponivelNaData = true
                    }
                ]);

            // Act
            var resultado = controller.ProfissionaisDisponiveis(1, data, "menor_preco", true, false, 200m);

            // Assert
            var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
            var model = Assert.IsInstanceOfType<ProfissionaisDisponiveisViewModel>(viewResult.ViewData.Model);
            Assert.AreEqual(1, model.IdServico);
            Assert.AreEqual("Formatação e Limpeza", model.NomeServico);
            Assert.AreEqual("menor_preco", model.OrdenarPor);
            Assert.IsTrue(model.ApenasVerificados);
            Assert.AreEqual(1, model.Profissionais.Count());
        }

        [TestMethod]
        public void ProfissionaisDisponiveis_QuandoServicoNaoExiste_DeveRetornarNotFound()
        {
            // Act
            var resultado = controller.ProfissionaisDisponiveis(999, null);

            // Assert
            Assert.IsInstanceOfType<NotFoundResult>(resultado);
            servicoServiceMock.Verify(s => s.Get(999), Times.Once);
            servicoServiceMock.Verify(
                s => s.BuscarProfissionaisPorServico(
                    It.IsAny<int>(), It.IsAny<DateTime?>(), It.IsAny<string?>(),
                    It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<decimal?>()),
                Times.Never);
        }

        private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
        {
            var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
            Assert.IsNull(redirectResult.ControllerName);
            Assert.AreEqual(nameof(ServicoController.Index), redirectResult.ActionName);
        }

        private static ServicoViewModel CriarModeloDeNovoServico()
        {
            return new ServicoViewModel
            {
                IdEspecialidade = 1,
                Nome = "Troca de Tela",
                Descricao = "Substituição completa de display e touch screen"
            };
        }

        private static ServicoViewModel CriarModeloDeServicoExistente()
        {
            return new ServicoViewModel
            {
                Id = 1,
                IdEspecialidade = 1,
                Nome = "Formatação e Limpeza",
                Descricao = "Limpeza interna e formatação completa do sistema operacional"
            };
        }

        private static Servico ObterServicoAlvo()
        {
            return new Servico
            {
                Id = 1,
                IdEspecialidade = 1,
                Nome = "Formatação e Limpeza",
                Descricao = "Limpeza interna e formatação completa do sistema operacional"
            };
        }

        private static Servico ObterSegundoServico()
        {
            return new Servico
            {
                Id = 2,
                IdEspecialidade = 1,
                Nome = "Instalação de Redes",
                Descricao = "Configuração de roteadores, cabeamento estruturado e switches"
            };
        }

        private static IEnumerable<Servico> ObterServicosDeTeste()
        {
            return
            [
                ObterServicoAlvo(),
                ObterSegundoServico()
            ];
        }
    }
}
