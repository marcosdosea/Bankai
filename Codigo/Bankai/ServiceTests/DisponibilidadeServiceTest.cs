using Core;
using Core.Service;
using Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

[TestClass]
public class DisponibilidadeServiceTest
{
    private BankaiContext context = null;
    private IDisponibilidadeService service = null;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<BankaiContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        this.context = new BankaiContext(options);
        this.service = new DisponibilidadeService(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [TestMethod]
    public void Create_DadosValidos_RetornaIdDisponibilidade()
    {
        // Arrange
        var disponibilidade = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = "Segunda-Feira",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(18, 0, 0),
            StatusDisponibilidade = "D"
        };

        // Act
        var id = this.service.Create(disponibilidade);

        // Assert
        Assert.IsTrue(id > 0);
        Assert.AreEqual(1, this.context.Disponibilidades.Count());
    }

    [TestMethod]
    public void Get_IdExistente_RetornaObjetoDisponibilidade()
    {
        // Arrange
        var disponibilidade = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = "Terça-Feira",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(18, 0, 0),
            StatusDisponibilidade = "D"
        };
        var id = this.service.Create(disponibilidade);

        // Act
        var resultado = this.service.Get(id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(id, (uint)resultado.Id);
    }

    [TestMethod]
    public void Get_IdInexistente_RetornaNullo()
    {
        // Act
        var resultado = this.service.Get(999);

        // Assert
        Assert.IsNull(resultado);
    }

    [TestMethod]
    public void Edit_DadosValidos_AtualizaDisponibilidade()
    {
        // Arrange
        var disponibilidade = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = "Quarta-Feira",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(12, 0, 0),
            StatusDisponibilidade = "D"
        };
        var id = this.service.Create(disponibilidade);

        // Limpa o rastreador de instâncias do EF para permitir a atualização sem conflitos de contexto
        this.context.ChangeTracker.Clear();

        var disponibilidadeParaEditar = this.service.Get(id);
        Assert.IsNotNull(disponibilidadeParaEditar);

        // Alterando o horário de fim de 12:00 para 18:00
        disponibilidadeParaEditar.HorarioFim = new TimeSpan(18, 0, 0);

        // Act
        this.service.Edit(disponibilidadeParaEditar);

        // Assert
        var atualizado = this.service.Get(id);
        Assert.IsNotNull(atualizado);
        Assert.AreEqual(new TimeSpan(18, 0, 0), atualizado.HorarioFim);
    }

    [TestMethod]
    public void Edit_OutraInstanciaComMesmoIdAtualizaDisponibilidadeRastreada()
    {
        var original = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = "Quarta-Feira",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(12, 0, 0),
            StatusDisponibilidade = "D"
        };
        var id = this.service.Create(original);
        var rastreada = this.service.Get(id);
        Assert.IsNotNull(rastreada);

        var disponibilidadeParaEditar = new Disponibilidade
        {
            Id = (int)id,
            IdPessoa = 1,
            DiaSemana = "Quarta-Feira",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(18, 0, 0),
            StatusDisponibilidade = "D"
        };

        this.service.Edit(disponibilidadeParaEditar);

        Assert.AreEqual(new TimeSpan(18, 0, 0), rastreada.HorarioFim);
        Assert.AreEqual(new TimeSpan(18, 0, 0), this.service.Get(id)!.HorarioFim);
    }

    [TestMethod]
    public void Delete_IdExistente_RemoveDoBanco()
    {
        // Arrange
        var disponibilidade = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = "Quinta-Feira",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(18, 0, 0),
            StatusDisponibilidade = "D"
        };
        var id = this.service.Create(disponibilidade);

        // Act
        this.service.Delete(id);

        // Assert
        Assert.IsNull(this.service.Get(id));
        Assert.AreEqual(0, this.context.Disponibilidades.Count());
    }

    [TestMethod]
    public void GetAll_CadastrosExistentes_RetornaColecaoCompleta()
    {
        // Arrange
        this.service.Create(new Disponibilidade { IdPessoa = 1, DiaSemana = "Segunda-Feira", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });
        this.service.Create(new Disponibilidade { IdPessoa = 2, DiaSemana = "Terça-Feira", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });

        // Act
        var lista = this.service.GetAll();

        // Assert
        Assert.IsNotNull(lista);
        Assert.AreEqual(2, lista.Count());
    }

    [TestMethod]
    public void GetByIdPessoa_FiltroValido_RetornaRegistrosFiltrados()
    {
        // Arrange
        this.service.Create(new Disponibilidade { IdPessoa = 1, DiaSemana = "Segunda-Feira", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });
        this.service.Create(new Disponibilidade { IdPessoa = 1, DiaSemana = "Terça-Feira", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });
        this.service.Create(new Disponibilidade { IdPessoa = 2, DiaSemana = "Segunda-Feira", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });

        // Act
        var resultado = this.service.GetByIdPessoa(1);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Count());
        Assert.IsTrue(resultado.All(d => d.IdPessoa == 1));
    }
}