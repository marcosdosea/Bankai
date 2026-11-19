using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;
using System;
using System.Linq;

namespace Service.Tests;

[TestClass]
public class OrcamentoServiceTests
{
    private BankaiContext contexto = null!;
    private IOrcamentoService orcamentoService = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Bankai_Orcamento_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<BankaiContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new BankaiContext(opcoes);

        // Apenas 1 orçamento cadastrado inicialmente (vinculado ao Agendamento 1)
        contexto.Orcamentos.Add(
            new Orcamento
            {
                Id = 1,
                ValorProposto = 500.00m,
                Validade = 7,
                MateriaisAdicionais = null,
                Descricao = "Orçamento para reforma de quadro residencial"
            }
        );

        contexto.Agendamentos.AddRange(
            // Agendamento 1: Tipo Orçamento ('O') já com orçamento vinculado
            new Agendamento
            {
                Id = 1,
                IdProfissional = 2,
                IdCliente = 1,
                TipoCobranca = "O",
                StatusAgendamento = "Execucao",
                HorarioInicio = new DateTime(2024, 6, 10, 9, 0, 0),
                HorarioFim = new DateTime(2024, 6, 10, 11, 0, 0),
                Data = new DateTime(2024, 6, 10),
                Comentario = "Agendamento para orçamento de quadro residencial",
                IdOrcamento = 1
            },

            // Agendamento 2: Tipo Fixo ('F') - Não pode ter orçamento
            new Agendamento
            {
                Id = 2,
                IdProfissional = 2,
                IdCliente = 1,
                TipoCobranca = "F",
                StatusAgendamento = "Execucao",
                HorarioInicio = new DateTime(2024, 6, 11, 14, 0, 0),
                HorarioFim = new DateTime(2024, 6, 11, 16, 0, 0),
                Data = new DateTime(2024, 6, 11),
                Comentario = "Agendamento para serviço fixo instalação de iluminação",
                IdOrcamento = null
            },

            // Agendamento 3: Tipo Orçamento ('O') aguardando criação do orçamento
            new Agendamento
            {
                Id = 3,
                IdProfissional = 2,
                IdCliente = 1,
                TipoCobranca = "O",
                StatusAgendamento = "Pendente",
                HorarioInicio = new DateTime(2024, 6, 12, 10, 0, 0),
                HorarioFim = new DateTime(2024, 6, 12, 12, 0, 0),
                Data = new DateTime(2024, 6, 12),
                Comentario = "Agendamento para troca de fiação",
                IdOrcamento = null
            }
        );

        contexto.SaveChanges();

        orcamentoService = new OrcamentoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosValidos_DeveCriarOrcamentoEVincularAoAgendamento()
    {
        // Arrange
        var novoOrcamento = new Orcamento
        {
            ValorProposto = 350.00m,
            Validade = 10,
            Descricao = "Orçamento para troca de fiação",
            MateriaisAdicionais = null
        };

        // Act (vinculando ao Agendamento 3, que é do tipo 'O')
        var idCriado = orcamentoService.Create(novoOrcamento, 3);
        var agendamentoAtualizado = contexto.Agendamentos.Find(3);

        // Assert
        Assert.IsTrue(idCriado > 0);
        Assert.IsNotNull(agendamentoAtualizado);
        Assert.AreEqual((int)idCriado, agendamentoAtualizado.IdOrcamento);
    }

    [TestMethod]
    public void Create_QuandoValorOuValidadeZeroOuNegativo_DeveLancarArgumentException()
    {
        // Arrange
        var orcamentoValorInvalido = new Orcamento { ValorProposto = 0, Validade = 7, Descricao = "Teste" };
        var orcamentoValidadeInvalida = new Orcamento { ValorProposto = 500, Validade = 0, Descricao = "Teste" };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => orcamentoService.Create(orcamentoValorInvalido, 3));
        Assert.ThrowsExactly<ArgumentException>(() => orcamentoService.Create(orcamentoValidadeInvalida, 3));
    }

    [TestMethod]
    public void Create_QuandoNaoExisteAgendamento_DeveLancarArgumentException()
    {
        // Arrange
        var novoOrcamento = new Orcamento { ValorProposto = 300, Validade = 7, Descricao = "Teste" };

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => orcamentoService.Create(novoOrcamento, 999));
    }

    [TestMethod]
    public void Create_QuandoTipoCobrancaNaoForOrcamento_DeveLancarInvalidOperationException()
    {
        // Arrange
        var novoOrcamento = new Orcamento { ValorProposto = 200, Validade = 7, Descricao = "Teste" };

        // Act & Assert (Agendamento 2 é tipo 'F' - Fixo)
        Assert.ThrowsExactly<InvalidOperationException>(() => orcamentoService.Create(novoOrcamento, 2));
    }

    [TestMethod]
    public void CalcularTaxaPlataforma_QuandoValorInformado_DeveCalcular15Porcento()
    {
        // Act
        var taxa = orcamentoService.CalcularTaxaPlataforma(450.00m);

        // Assert
        Assert.AreEqual(67.50m, taxa);
    }

    [TestMethod]
    public void CalcularValorLiquido_QuandoValorInformado_DeveCalcular85Porcento()
    {
        // Act
        var liquido = orcamentoService.CalcularValorLiquido(450.00m);

        // Assert
        Assert.AreEqual(382.50m, liquido);
    }

    [TestMethod]
    public void Get_QuandoExiste_DeveRetornarOrcamentoCorrespondente()
    {
        // Act
        var orcamento = orcamentoService.Get(1);

        // Assert
        Assert.IsNotNull(orcamento);
        Assert.AreEqual(1, orcamento.Id);
        Assert.AreEqual(500.00m, orcamento.ValorProposto);
        Assert.AreEqual(7, orcamento.Validade);
        Assert.AreEqual("Orçamento para reforma de quadro residencial", orcamento.Descricao);
        Assert.IsNull(orcamento.MateriaisAdicionais);
    }

    [TestMethod]
    public void Get_QuandoNaoExiste_DeveRetornarNulo()
    {
        // Act
        var orcamento = orcamentoService.Get(999);

        // Assert
        Assert.IsNull(orcamento);
    }

    [TestMethod]
    public void GetAll_QuandoChamado_DeveRetornarTodosOrcamentosCadastrados()
    {
        // Act
        var orcamentos = orcamentoService.GetAll().ToList();

        // Assert
        Assert.AreEqual(1, orcamentos.Count);
    }
}