using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service.Tests;

[TestClass]
public class ServicoServiceTests
{
    private BankaiContext contexto = null!;
    private IServicoService servicoService = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Bankai_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<BankaiContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new BankaiContext(opcoes);

        contexto.Servicos.AddRange(
            new Servico
            {
                Id = 1,
                Nome = "Instalação Elétrica Residencial",
                Descricao = "Instalação de tomadas, interruptores e fiação"
            },
            new Servico
            {
                Id = 2,
                Nome = "Reparo Hidráulico",
                Descricao = "Troca de torneiras, reparo de vazamentos e canos"
            },
            new Servico
            {
                Id = 3,
                Nome = "Pintura de Parede",
                Descricao = "Pintura interna e externa com preparação de superfície"
            });

        contexto.SaveChanges();

        servicoService = new ServicoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDoServicoCriado()
    {
        // Arrange
        var servico = new Servico
        {
            Id = 4,
            Nome = "Manutenção de Ar Condicionado",
            Descricao = "Limpeza de filtros e verificação de gás refrigerante"
        };

        // Act
        var idRetornado = servicoService.Create(servico);

        // Assert
        Assert.AreEqual(4, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarServico()
    {
        // Arrange
        var servicoEsperado = new Servico
        {
            Id = 4,
            Nome = "Manutenção de Ar Condicionado",
            Descricao = "Limpeza de filtros e verificação de gás refrigerante"
        };

        // Act
        servicoService.Create(servicoEsperado);

        // Assert
        var servicoCriado = servicoService.Get(4);

        Assert.IsNotNull(servicoCriado);
        Assert.AreEqual(servicoEsperado.Id, servicoCriado.Id);
        Assert.AreEqual(servicoEsperado.Nome, servicoCriado.Nome);
        Assert.AreEqual(servicoEsperado.Descricao, servicoCriado.Descricao);
    }

    [TestMethod]
    public void Delete_QuandoServicoExiste_DeveRemoverServico()
    {
        // Act
        servicoService.Delete(2);

        // Assert
        var servicoExcluido = servicoService.Get(2);
        var servicosRestantes = servicoService.GetAll().ToList();

        Assert.IsNull(servicoExcluido);
        Assert.AreEqual(2, servicosRestantes.Count);
        Assert.IsFalse(servicosRestantes.Any(s => s.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoServicoNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var servicosIniciais = servicoService.GetAll().ToList();

        // Act
        servicoService.Delete(999);

        // Assert
        var servicosFinais = servicoService.GetAll().ToList();

        Assert.AreEqual(servicosIniciais.Count, servicosFinais.Count);
        CollectionAssert.AreEquivalent(
            servicosIniciais.Select(s => s.Id).ToList(),
            servicosFinais.Select(s => s.Id).ToList());
    }

    [TestMethod]
    public void Edit_QuandoServicoExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var servico = servicoService.Get(3);

        Assert.IsNotNull(servico);

        servico!.Nome = "Pintura Decorativa e Textura";
        servico.Descricao = "Aplicação de textura grafiato e pintura acrílica";

        // Act
        servicoService.Edit(servico);

        // Assert
        var servicoAtualizado = servicoService.Get(3);

        Assert.IsNotNull(servicoAtualizado);
        Assert.AreEqual("Pintura Decorativa e Textura", servicoAtualizado.Nome);
        Assert.AreEqual("Aplicação de textura grafiato e pintura acrílica", servicoAtualizado.Descricao);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarServicoCorrespondente()
    {
        // Act
        var servico = servicoService.Get(1);

        // Assert
        Assert.IsNotNull(servico);
        Assert.AreEqual(1, servico.Id);
        Assert.AreEqual("Instalação Elétrica Residencial", servico.Nome);
        Assert.AreEqual("Instalação de tomadas, interruptores e fiação", servico.Descricao);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var servico = servicoService.Get(999);

        // Assert
        Assert.IsNull(servico);
    }

    [TestMethod]
    public void GetAll_QuandoExistemServicos_DeveRetornarTodosOsServicos()
    {
        // Act
        var servicos = servicoService.GetAll().ToList();

        // Assert
        Assert.IsNotNull(servicos);
        Assert.AreEqual(3, servicos.Count);

        CollectionAssert.AreEquivalent(
            new int[] { 1, 2, 3 },
            servicos.Select(s => s.Id).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaServicos_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Servicos.RemoveRange(contexto.Servicos);
        contexto.SaveChanges();

        // Act
        var servicos = servicoService.GetAll().ToList();

        // Assert
        Assert.IsNotNull(servicos);
        Assert.AreEqual(0, servicos.Count);
    }

    [TestMethod]
    public void GetByNome_QuandoExisteCorrespondencia_DeveRetornarServicosCorrespondentes()
    {
        // Act
        var servicos = servicoService.GetByNome("Instalação").ToList();

        // Assert
        Assert.AreEqual(1, servicos.Count);
        Assert.AreEqual("Instalação Elétrica Residencial", servicos[0].Nome);
    }

    [TestMethod]
    public void GetByNome_QuandoNomeCorrespondeAoInicio_DeveRetornarTodosOsServicosCorrespondentes()
    {
        // Arrange
        contexto.Servicos.Add(new Servico
        {
            Id = 4,
            Nome = "Instalação de Câmeras de Segurança",
            Descricao = "Instalação de CFTV e configuração remota"
        });

        contexto.SaveChanges();

        // Act
        var servicos = servicoService.GetByNome("Instalação").ToList();

        // Assert
        Assert.AreEqual(2, servicos.Count);
        Assert.IsTrue(servicos.All(s => s.Nome.StartsWith("Instalação")));
    }

    [TestMethod]
    public void GetByNome_QuandoNaoExisteCorrespondencia_DeveRetornarColecaoVazia()
    {
        // Act
        var servicos = servicoService.GetByNome("Marcenaria").ToList();

        // Assert
        Assert.IsNotNull(servicos);
        Assert.AreEqual(0, servicos.Count);
    }

    [TestMethod]
    public void GetByNome_QuandoNomeEstaVazio_DeveRetornarTodosOsServicos()
    {
        // Act
        var servicos = servicoService.GetByNome(string.Empty).ToList();

        // Assert
        Assert.AreEqual(3, servicos.Count);
    }
}

