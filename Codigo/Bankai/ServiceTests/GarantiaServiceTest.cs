using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace ServiceTest
{
    [TestClass]
    public class GarantiaServiceTest
    {
        private BankaiContext context = null!;
        private GarantiaService garantiaService = null!;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<BankaiContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            context = new BankaiContext(options);
            garantiaService = new GarantiaService(context);

            var agendamento = new Agendamento
            {
                Id = 1,
                Comentario = "Troca de Resistência de Chuveiro",
                TipoCobranca = "F",
                StatusAgendamento = "Concluido"
            };
            context.Add(agendamento);

            var chamado = new Chamadogarantium
            {
                Id = 1,
                IdAgendamento = 1,
                Descricao = "Chuveiro voltou a queimar a resistência durante o uso normal",
                DataChamado = DateTime.Now,
                StatusGarantia = "Aberto"
            };
            context.Add(chamado);
            context.SaveChanges();

            context.ChangeTracker.Clear();
        }

        [TestMethod]
        public void Create_DadosValidos_RetornaIdValido()
        {
            // Arrange
            var novoChamado = new Chamadogarantium
            {
                IdAgendamento = 1,
                Descricao = "Vazamento de água na conexão elétrica",
                DataChamado = DateTime.Now,
                StatusGarantia = "Aberto"
            };

            // Act
            var id = garantiaService.Create(novoChamado);

            // Assert
            Assert.AreNotEqual(0u, id);
            var chamadoInserido = garantiaService.Get(id);
            Assert.IsNotNull(chamadoInserido);
            Assert.AreEqual("Vazamento de água na conexão elétrica", chamadoInserido.Descricao);
        }

        [TestMethod]
        public void Create_DescricaoVazia_LancaArgumentException()
        {
            // Arrange
            var chamadoInvalido = new Chamadogarantium
            {
                IdAgendamento = 1,
                Descricao = "",
                DataChamado = DateTime.Now
            };

            // Act & Assert
            try
            {
                garantiaService.Create(chamadoInvalido);
                Assert.Fail("Deveria ter lançado ArgumentException, mas não lançou.");
            }
            catch (ArgumentException)
            {
                // O teste passa com sucesso se capturar a ArgumentException!
            }
        }

        [TestMethod]
        public void Get_IdExistente_RetornaObjeto()
        {
            // Act
            var chamado = garantiaService.Get(1u);

            // Assert
            Assert.IsNotNull(chamado);
            Assert.AreEqual<uint>(1u, (uint)chamado!.Id);
        }

        [TestMethod]
        public void GetAll_RetornaColecaoPreenchida()
        {
            // Act
            var lista = garantiaService.GetAll();

            // Assert
            Assert.AreEqual(1, lista.Count());
        }

        [TestMethod]
        public void Edit_AlteracaoValida_AtualizaRegistro()
        {
            // Arrange
            var chamado = garantiaService.Get(1u)!;
            chamado.Descricao = "Nova descrição atualizada do problema";

            // Act
            garantiaService.Edit(chamado);

            // Assert
            var chamadoAtualizado = garantiaService.Get(1u);
            Assert.IsNotNull(chamadoAtualizado);
            Assert.AreEqual("Nova descrição atualizada do problema", chamadoAtualizado.Descricao);
        }

        [TestMethod]
        public void Delete_IdExistente_RemoveRegistro()
        {
            // Act
            garantiaService.Delete(1u);

            // Assert
            var chamado = garantiaService.Get(1u);
            Assert.IsNull(chamado);
        }
    }
}