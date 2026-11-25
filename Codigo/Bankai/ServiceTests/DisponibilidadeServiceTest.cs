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
    private BankaiContext context = null!;
    private IDisponibilidadeService service = null!;

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
            DiaSemana = "SF",
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
    public void Create_BloqueioExcepcionalComDataEMotivo_ArmazenaNovosCampos()
    {
        // Arrange
        var dataBloqueio = DateTime.Today.AddDays(10);
        var bloqueio = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = ObterSiglaDia(dataBloqueio.DayOfWeek),
            HorarioInicio = TimeSpan.Zero,
            HorarioFim = new TimeSpan(23, 59, 0),
            StatusDisponibilidade = "ND",
            DataIndisponibilizade = dataBloqueio,
            DescricaoIndisponibilidade = "Atendimento de obra particular"
        };

        // Act
        var id = this.service.Create(bloqueio);
        var salvo = this.service.Get(id);

        // Assert
        Assert.IsNotNull(salvo);
        Assert.AreEqual(dataBloqueio.Date, salvo.DataIndisponibilizade);
        Assert.AreEqual("Atendimento de obra particular", salvo.DescricaoIndisponibilidade);
        Assert.AreEqual("ND", salvo.StatusDisponibilidade);
    }

    [TestMethod]
    public void Create_DiaSemanaForaDoEnum_LancaArgumentException()
    {
        var disponibilidade = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = "Segunda-Feira",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(18, 0, 0),
            StatusDisponibilidade = "D"
        };

        Assert.ThrowsExactly<ArgumentException>(() => this.service.Create(disponibilidade));
    }

    [TestMethod]
    public void Create_BloqueioComDataPassada_LancaInvalidOperationException()
    {
        var dataPassada = DateTime.Today.AddDays(-1);
        var bloqueio = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = ObterSiglaDia(dataPassada.DayOfWeek),
            HorarioInicio = TimeSpan.Zero,
            HorarioFim = new TimeSpan(23, 59, 0),
            StatusDisponibilidade = "ND",
            DataIndisponibilizade = dataPassada,
            DescricaoIndisponibilidade = "Folga"
        };

        Assert.ThrowsExactly<InvalidOperationException>(() => this.service.Create(bloqueio));
    }

    [TestMethod]
    public void Create_BloqueioSemMotivo_LancaArgumentException()
    {
        var dataFutura = DateTime.Today.AddDays(5);
        var bloqueio = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = ObterSiglaDia(dataFutura.DayOfWeek),
            HorarioInicio = TimeSpan.Zero,
            HorarioFim = new TimeSpan(23, 59, 0),
            StatusDisponibilidade = "ND",
            DataIndisponibilizade = dataFutura,
            DescricaoIndisponibilidade = "   "
        };

        Assert.ThrowsExactly<ArgumentException>(() => this.service.Create(bloqueio));
    }

    [TestMethod]
    public void Create_BloqueioComDiaSemanaDivergenteDaData_LancaArgumentException()
    {
        var dataFutura = DateTime.Today.AddDays(5);
        var siglaDivergente = dataFutura.DayOfWeek == DayOfWeek.Sunday ? "SF" : "D";
        var bloqueio = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = siglaDivergente,
            HorarioInicio = TimeSpan.Zero,
            HorarioFim = new TimeSpan(23, 59, 0),
            StatusDisponibilidade = "ND",
            DataIndisponibilizade = dataFutura,
            DescricaoIndisponibilidade = "Viagem particular"
        };

        Assert.ThrowsExactly<ArgumentException>(() => this.service.Create(bloqueio));
    }

    [TestMethod]
    public void Create_BloqueioDuplicadoNaMesmaData_LancaInvalidOperationException()
    {
        var dataFutura = DateTime.Today.AddDays(7);
        var sigla = ObterSiglaDia(dataFutura.DayOfWeek);

        this.service.Create(new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = sigla,
            HorarioInicio = TimeSpan.Zero,
            HorarioFim = new TimeSpan(23, 59, 0),
            StatusDisponibilidade = "ND",
            DataIndisponibilizade = dataFutura,
            DescricaoIndisponibilidade = "Viagem"
        });

        var duplicado = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = sigla,
            HorarioInicio = TimeSpan.Zero,
            HorarioFim = new TimeSpan(23, 59, 0),
            StatusDisponibilidade = "ND",
            DataIndisponibilizade = dataFutura,
            DescricaoIndisponibilidade = "Outro compromisso"
        };

        Assert.ThrowsExactly<InvalidOperationException>(() => this.service.Create(duplicado));
    }

    [TestMethod]
    public void Get_IdExistente_RetornaObjetoDisponibilidade()
    {
        // Arrange
        var disponibilidade = new Disponibilidade
        {
            IdPessoa = 1,
            DiaSemana = "TF",
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
            DiaSemana = "QF",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(12, 0, 0),
            StatusDisponibilidade = "D"
        };
        var id = this.service.Create(disponibilidade);

        this.context.ChangeTracker.Clear();

        var disponibilidadeParaEditar = this.service.Get(id);
        Assert.IsNotNull(disponibilidadeParaEditar);

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
            DiaSemana = "QF",
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
            DiaSemana = "QF",
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
            DiaSemana = "QIF",
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
        this.service.Create(new Disponibilidade { IdPessoa = 1, DiaSemana = "SF", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });
        this.service.Create(new Disponibilidade { IdPessoa = 2, DiaSemana = "TF", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });

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
        this.service.Create(new Disponibilidade { IdPessoa = 1, DiaSemana = "SF", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });
        this.service.Create(new Disponibilidade { IdPessoa = 1, DiaSemana = "TF", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });
        this.service.Create(new Disponibilidade { IdPessoa = 2, DiaSemana = "SF", HorarioInicio = new TimeSpan(8, 0, 0), HorarioFim = new TimeSpan(18, 0, 0), StatusDisponibilidade = "D" });

        // Act
        var resultado = this.service.GetByIdPessoa(1);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Count());
        Assert.IsTrue(resultado.All(d => d.IdPessoa == 1));
    }

    private static string ObterSiglaDia(DayOfWeek dia) => dia switch
    {
        DayOfWeek.Sunday => "D",
        DayOfWeek.Monday => "SF",
        DayOfWeek.Tuesday => "TF",
        DayOfWeek.Wednesday => "QF",
        DayOfWeek.Thursday => "QIF",
        DayOfWeek.Friday => "SXF",
        DayOfWeek.Saturday => "S",
        _ => "D"
    };
}