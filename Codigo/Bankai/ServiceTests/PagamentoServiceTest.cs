using System;
using System.Linq;
using Core;
using Core.Service;
using Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Service.Tests
{
    [TestClass]
    public class PagamentoServiceTest
    {
        private BankaiContext context = null!;
        private IPagamentoService service = null!;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankaiContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            this.context = new BankaiContext(options);

            // Popula um Agendamento válido conforme o Dicionário de Dados para vincular aos pagamentos
            this.context.Agendamentos.Add(new Agendamento
            {
                Id = 1,
                IdCliente = 1,
                IdProfissional = 1,
                Comentario = "Manutenção Elétrica Residencial",
                TipoCobranca = "Fixo",
                StatusAgendamento = "Aceito"
            });
            this.context.SaveChanges();

            this.service = new PagamentoService(this.context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.context.Database.EnsureDeleted();
            this.context.Dispose();
        }

        [TestMethod]
        public void RealizarPagamento_ViaPixValido_AprovaTransacaoERetornaPagamento()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Pix",
                ValorTransacao = 150.00m
            };

            // Act
            var resultado = this.service.RealizarPagamento(pagamento, transacaoCompleta: true);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Aprovado", resultado.StatusPagamento);
            Assert.AreEqual("Pix", resultado.FormaPagamento);
            Assert.AreEqual(1, this.context.Pagamentos.Count());
        }

        [TestMethod]
        public void RealizarPagamento_ViaCartaoCreditoValido_AprovaTransacao()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Cartao de credito",
                ValorTransacao = 280.50m
            };

            // Act
            var resultado = this.service.RealizarPagamento(pagamento, transacaoCompleta: true);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Aprovado", resultado.StatusPagamento);
            Assert.AreEqual("Cartao de credito", resultado.FormaPagamento);
        }

        [TestMethod]
        public void RealizarPagamento_DadosEmBrancoOuInvalidos_LancaExcecaoCSU06()
        {
            // Arrange
            var pagamentoInvalido = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "", // Campo obrigatório em branco
                ValorTransacao = 0m
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                this.service.RealizarPagamento(pagamentoInvalido));

            Assert.AreEqual("Por favor, preencha todos os campos obrigatórios corretamente.", ex.Message);
        }

        [TestMethod]
        public void RealizarPagamento_TransacaoRecusada_RegistraComoRecusadoELancaExcecao()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Cartao de credito",
                ValorTransacao = 500.00m
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                this.service.RealizarPagamento(pagamento, transacaoCompleta: false));

            Assert.AreEqual("Pagamento não autorizado pela instituição financeira.", ex.Message);
            var registroBanco = this.context.Pagamentos.FirstOrDefault();
            Assert.IsNotNull(registroBanco);
            Assert.AreEqual("Recusado", registroBanco.StatusPagamento);
        }

        [TestMethod]
        public void Create_DadosValidos_RetornaCodigoPagamento()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Pix",
                StatusPagamento = "Pendente",
                ValorTransacao = 100.00m
            };

            // Act
            var id = this.service.Create(pagamento);

            // Assert
            Assert.IsTrue(id > 0);
            Assert.AreEqual(1, this.context.Pagamentos.Count());
        }

        [TestMethod]
        public void Get_CodigoExistente_RetornaObjetoPagamento()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Pix",
                StatusPagamento = "Aprovado",
                ValorTransacao = 90.00m
            };
            var Id = this.service.Create(pagamento);

            // Act
            var resultado = this.service.get((uint)Id);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(90.00m, resultado.ValorTransacao);
        }

        [TestMethod]
        public void Get_CodigoInexistente_RetornaNulo()
        {
            // Act
            var resultado = this.service.get(999);

            // Assert
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Edit_DadosValidos_AtualizaStatusPagamento()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Pix",
                StatusPagamento = "Pendente",
                ValorTransacao = 120.00m
            };
            var id = this.service.Create(pagamento);

            this.context.ChangeTracker.Clear();

            var pagamentoParaEditar = this.service.get((uint)id);
            Assert.IsNotNull(pagamentoParaEditar);
            pagamentoParaEditar.StatusPagamento = "Aprovado";

            // Act
            this.service.Edit(pagamentoParaEditar);

            // Assert
            var atualizado = this.service.get((uint)id);
            Assert.IsNotNull(atualizado);
            Assert.AreEqual("Aprovado", atualizado.StatusPagamento);
        }

        [TestMethod]
        public void Delete_CodigoExistente_RemoveDoBanco()
        {
            // Arrange
            var pagamento = new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Pix",
                StatusPagamento = "Pendente",
                ValorTransacao = 75.00m
            };
            var id = this.service.Create(pagamento);

            // Act
            this.service.Delete((uint)id);

            // Assert
            Assert.IsNull(this.service.get((uint)id));
            Assert.AreEqual(0, this.context.Pagamentos.Count());
        }

        [TestMethod]
        public void GetAll_CadastrosExistentes_RetornaColecaoCompleta()
        {
            // Arrange
            this.service.Create(new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Pix",
                StatusPagamento = "Aprovado",
                ValorTransacao = 50.00m
            });
            this.service.Create(new Pagamento
            {
                IdAgendamento = 1,
                FormaPagamento = "Cartao de credito",
                StatusPagamento = "Aprovado",
                ValorTransacao = 80.00m
            });

            // Act
            var lista = this.service.getAll();

            // Assert
            Assert.IsNotNull(lista);
            Assert.AreEqual(2, lista.Count());
        }
    }
}