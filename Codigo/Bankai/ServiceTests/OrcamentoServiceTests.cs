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

        // Seed de Pessoas (Cliente e Profissional)
        var cliente = new Pessoa
        {
            Id = 1,
            Nome = "Carlos Albuquerque",
            CpfCnpj = "11122233344",
            Email = "carlos@cliente.com",
            EhProfissional = false,
            Cep = "49000000",
            Rua = "Rua A",
            Bairro = "Bairro B",
            Cidade = "Aracaju",
            Estado = "SE",
            NumeroCasa = "100"
        };

        var profissional1 = new Pessoa
        {
            Id = 2,
            Nome = "Marcos Silva Eletricista",
            CpfCnpj = "55566677788",
            Email = "marcos@pro.com",
            EhProfissional = true,
            Cep = "49000001",
            Rua = "Rua B",
            Bairro = "Bairro C",
            Cidade = "Aracaju",
            Estado = "SE",
            NumeroCasa = "200"
        };

        var profissional2 = new Pessoa
        {
            Id = 3,
            Nome = "Ricardo Engenharia Elétrica",
            CpfCnpj = "99988877766",
            Email = "ricardo@pro.com",
            EhProfissional = true,
            Cep = "49000002",
            Rua = "Rua C",
            Bairro = "Bairro D",
            Cidade = "Aracaju",
            Estado = "SE",
            NumeroCasa = "300"
        };

        // Seed de Servicos
        var servicoEletrico = new Servico
        {
            Id = 1,
            Nome = "Reforma de Quadro Residencial",
            Descricao = "Substituição completa do barramento e disjuntores"
        };

        var servicoTrocaChuveiro = new Servico
        {
            Id = 2,
            Nome = "Troca de Chuveiro",
            Descricao = "Troca e fixação da resistência e fiação"
        };

        // Agendamento 1: Tipo Orçamento ('O') com profissional 2
        var agendamentoOrcamento = new Agendamento
        {
            Id = 1,
            IdProfissional = 2,
            IdCliente = 1,
            DescricaoServico = "Quadro de disjuntores desarmando frequentemente",
            TipoCobranca = "O", // Orçamento
            StatusAgendamento = "Execucao",
            Nota = "B",
            DiaSemana = "SF",
            HorarioInicio = new TimeSpan(14, 0, 0),
            HorarioFim = new TimeSpan(16, 0, 0)
        };

        var itemOrcamento = new Agendamentoservico
        {
            IdAgendamento = 1,
            IdServico = 1,
            ValorProposto = 0.00m,
            StatusServico = "E"
        };

        // Agendamento 2: Tipo Fixo ('F')
        var agendamentoFixo = new Agendamento
        {
            Id = 2,
            IdProfissional = 2,
            IdCliente = 1,
            DescricaoServico = "Troca simples de chuveiro elétrico",
            TipoCobranca = "F", // Fixo
            StatusAgendamento = "Execucao",
            Nota = "B",
            DiaSemana = "TF",
            HorarioInicio = new TimeSpan(10, 0, 0),
            HorarioFim = new TimeSpan(11, 0, 0)
        };

        var itemFixo = new Agendamentoservico
        {
            IdAgendamento = 2,
            IdServico = 2,
            ValorProposto = 120.00m,
            StatusServico = "E"
        };

        contexto.Pessoas.AddRange(cliente, profissional1, profissional2);
        contexto.Servicos.AddRange(servicoEletrico, servicoTrocaChuveiro);
        contexto.Agendamentos.AddRange(agendamentoOrcamento, agendamentoFixo);
        contexto.Agendamentoservicos.AddRange(itemOrcamento, itemFixo);
        contexto.SaveChanges();

        orcamentoService = new OrcamentoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void EnviarOrcamento_QuandoDadosValidos_DeveAtualizarValorPropostoERetornarTrue()
    {
        // Act
        var resultado = orcamentoService.EnviarOrcamento(1, 1, 450.00m);

        // Assert
        Assert.IsTrue(resultado);
        var orcamento = orcamentoService.Get(1, 1);
        Assert.IsNotNull(orcamento);
        Assert.AreEqual(450.00m, orcamento.ValorProposto);
        Assert.AreEqual(67.50m, orcamento.TaxaPlataforma);
        Assert.AreEqual(382.50m, orcamento.ValorLiquido);
    }

    [TestMethod]
    public void EnviarOrcamento_QuandoValorZeroOuNegativo_DeveLancarArgumentException()
    {
        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => orcamentoService.EnviarOrcamento(1, 1, 0.00m));
    }

    [TestMethod]
    public void EnviarOrcamento_QuandoNaoExisteAgendamento_DeveRetornarFalse()
    {
        // Act
        var resultado = orcamentoService.EnviarOrcamento(999, 1, 300.00m);

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void EnviarOrcamento_QuandoTipoCobrancaNaoForOrcamento_DeveLancarInvalidOperationException()
    {
        // Act & Assert (Agendamento 2 é tipo 'F' - Fixo)
        Assert.ThrowsExactly<InvalidOperationException>(() => orcamentoService.EnviarOrcamento(2, 2, 200.00m));
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
    public void Get_QuandoExiste_DeveRetornarOrcamentoDTOCorrespondente()
    {
        // Act
        var orcamento = orcamentoService.Get(1, 1);

        // Assert
        Assert.IsNotNull(orcamento);
        Assert.AreEqual(1, orcamento.IdAgendamento);
        Assert.AreEqual(1, orcamento.IdServico);
        Assert.AreEqual("Reforma de Quadro Residencial", orcamento.NomeServico);
        Assert.AreEqual("Carlos Albuquerque", orcamento.NomeCliente);
    }

    [TestMethod]
    public void Get_QuandoNaoExiste_DeveRetornarNulo()
    {
        // Act
        var orcamento = orcamentoService.Get(999, 999);

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
        Assert.AreEqual("O", orcamentos[0].TipoCobranca);
    }

    [TestMethod]
    public void GetByProfissional_QuandoProfissionalTemOrcamentos_DeveRetornarRegistros()
    {
        // Act
        var orcamentos = orcamentoService.GetByProfissional(2).ToList();

        // Assert
        Assert.AreEqual(1, orcamentos.Count);
        Assert.AreEqual(2, orcamentos[0].IdProfissional);
    }
}
