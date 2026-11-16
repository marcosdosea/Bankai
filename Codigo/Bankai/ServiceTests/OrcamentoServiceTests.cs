using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
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

        contexto.Orcamentos.AddRange(
            new Orcamento
            {
                Id = 1,
                ValorProposto = 500.00m,
                Validade = 7,
                MateriaisAdicionais = null,
                Descricao = "Orçamento para reforma de quadro residencial"
            },
             new Orcamento
             {
                 Id = 2,
                 ValorProposto = 600.00m,
                 Validade = 14,
                 MateriaisAdicionais = null,
                 Descricao = "Orçamento para instalação de iluminação"
             }
        );

        // Agendamento 1: Tipo Orçamento ('O') 
        contexto.Agendamentos.AddRange(
            new Agendamento
            {
                Id = 1,
                IdProfissional = 2,
                IdCliente = 1,
                TipoCobranca = "O", // Orçamento
                StatusAgendamento = "Execucao",
                HorarioInicio = new DateTime(2024, 6, 10, 9, 0, 0),
                HorarioFim = new DateTime(2024, 6, 10, 11, 0, 0),
                Data = new DateTime(2024, 6, 10),
                Comentario = "Agendamento para orçamento de quadro residencial",
                IdOrcamento = 1
            },

            // Agendamento 2: Tipo Fixo ('F')
            new Agendamento
            {
                Id = 2,
                IdProfissional = 2,
                IdCliente = 1,
                TipoCobranca = "F", // Fixo
                StatusAgendamento = "Execucao",
                HorarioInicio = new DateTime(2024, 6, 11, 14, 0, 0),
                HorarioFim = new DateTime(2024, 6, 11, 16, 0, 0),
                Data = new DateTime(2024, 6, 11),
                Comentario = "Agendamento para serviço fixo instalação de iluminação",
                IdOrcamento = 2
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
    public void EnviarOrcamento_QuandoDadosValidos_DeveRetornarTrue()
    {
        // Act
        var resultado = orcamentoService.EnviarOrcamento(1, 1, 500, 7, "Orçamento para reforma de quadro residencial", null!);

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void EnviarOrcamento_QuandoValorZeroOuNegativo_DeveLancarArgumentException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => orcamentoService.EnviarOrcamento(1, 1, 0, 7, "Orçamento para reforma de quadro residencial", null!));
        Assert.ThrowsExactly<ArgumentException>(() => orcamentoService.EnviarOrcamento(1, 1, 700, 0, "Orçamento para reforma de quadro residencial", null!));
    }

    [TestMethod]
    public void EnviarOrcamento_QuandoNaoExisteAgendamento_DeveRetornarFalse()
    {
        // Act
        var resultado = orcamentoService.EnviarOrcamento(1, 999, 300, 7, "Orçamento para reforma de quadro residencial", null!);

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EnviarOrcamento_QuandoTipoCobrancaNaoForOrcamento_DeveLancarInvalidOperationException()
    {
        // Act & Assert (Agendamento 2 é tipo 'F' - Fixo)
        Assert.ThrowsExactly<InvalidOperationException>(() => orcamentoService.EnviarOrcamento(2, 2, 200, 7, "Orçamento para reforma de quadro residencial", null!));
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
        Assert.AreEqual(500.00M, orcamento.ValorProposto);
        Assert.AreEqual(7, orcamento.Validade);
        Assert.AreEqual("Orçamento para reforma de quadro residencial", orcamento.Descricao);
        Assert.AreEqual(null, orcamento.MateriaisAdicionais);
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
    public void GetAll_QuandoChamado_DeveRetornarApenasAgendamentosTipoOrcamento()
    {
        // Act
        var orcamentos = orcamentoService.GetAll().ToList();

        // Assert
        Assert.AreEqual(1, orcamentos.Count);
    }   
}
