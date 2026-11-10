using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class OrcarServicoTests
{
    private BankaiContext contexto = null!;
    private IOrcarServicoService servicoOrcamento = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Bankai_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<BankaiContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new BankaiContext(opcoes);

        contexto.Agendamentoservicos.AddRange(
            new Agendamentoservico
            {
                IdAgendamento = 1,
                IdServico = 1,            
                StatusServico = "EmExecucao",
                ValorMaoDeObra = 500m,
                ValorMateriais = 500m,
              
            }, new Agendamentoservico 
            {
                IdAgendamento = 4,
                IdServico = 1,
                StatusServico = "EmExecucao",
                ValorMaoDeObra = 700m,
                ValorMateriais = 2000m
            });

        contexto.SaveChanges();

        servicoOrcamento = new OrcarServicoService(contexto);
    }



    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarValorBrutoMaiorQueValorMaoDeObra()
    {
        var orcamento = new Agendamentoservico
        {
            IdAgendamento = 4,
            IdServico = 1,
            StatusServico = "EmExecucao",
            ValorMaoDeObra = 500m,
            ValorMateriais = 500m
        };

        orcamento.CalcularOrcamento();

        var orcamentoBrutoRetornado = orcamento.OrcamentoBruto;
        Assert.IsTrue(orcamentoBrutoRetornado >= orcamento.ValorMaoDeObra);
    }

    [TestMethod]

    public void Create_QuandoDadosSaoValidos_ValorDeRepasseDeveSerMenorQueOrcamentoBruto()
    {
        var orcamento = new Agendamentoservico
        {
            IdAgendamento = 4,
            IdServico = 1,
            StatusServico = "EmExecucao",
            ValorMaoDeObra = 500m,
            ValorMateriais = 500m
        };

        orcamento.CalcularOrcamento();

        Assert.IsTrue(orcamento.RepasseLiquido < orcamento.OrcamentoBruto);


    }


    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var orcamento = servicoOrcamento.Get(999);

        // Assert
        Assert.IsNull(orcamento);
    }
    
    [TestMethod]
    public void Edit_QuandoValorMaoDeObraNaoEhZero_DeveAtualizarSeusDados()
    {
        // Arrange
        var orcamento = servicoOrcamento.Get(1);

        Assert.IsGreaterThan(0, orcamento.ValorMaoDeObra);  
    
    }


    [TestMethod]

    public void Delete_QuandoValorRepasseExiste_DeveRemoverOrcamento()
    {
        // Arrange
        var orcamento = servicoOrcamento.Get(4);
        
        orcamento.CalcularOrcamento();

        Assert.IsGreaterThan(0, orcamento.RepasseLiquido);
        // Act
        servicoOrcamento.Delete(4);
        // Assert
        var orcamentoDeletado = servicoOrcamento.Get(4);
        Assert.IsNull(orcamentoDeletado);
    }

    




}

    




