using Core;
using Core.Service;
using Service;
using Microsoft.EntityFrameworkCore;

[TestClass]
public class EspecialidadeServiceTest
{
    private BankaiContext context = null;
    private IEspecialidadeService service = null;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<BankaiContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        this.context = new BankaiContext(options);
        this.service = new EspecialidadeService(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Database.EnsureDeleted();
        context.Dispose();
    }

    [TestMethod]
    public void Create_DadosValidos_RetornaIdEspecialidade()
    {
        // Arrange
        var especialidade = new Especialidade
        {
            Nome = "Encanador"
        };

        // Act
        var id = this.service.Create(especialidade);

        // Assert
        Assert.IsTrue(id > 0);
        Assert.AreEqual(1, this.context.Especialidades.Count());
    }

    [TestMethod]
    public void Get_IdExistente_RetornaObjetoEspecialidade()
    {
        // Arrange
        var especialidade = new Especialidade { Nome = "Eletricista" };
        var id = this.service.Create(especialidade);

        // Act
        var resultado = this.service.Get(id);

        // Assert
        Assert.IsNotNull(resultado);
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
    public void Edit_DadosValidos_AtualizaEspecialidade()
    {
        // Arrange
        var especialidade = new Especialidade { Nome = "Pintor Inicial" };
        var id = this.service.Create(especialidade);

        // Limpa o rastreador de instâncias do EF para permitir a atualização sem conflitos de contexto
        this.context.ChangeTracker.Clear();

        var especialidadeParaEditar = this.service.Get(id);
        Assert.IsNotNull(especialidadeParaEditar);
        especialidadeParaEditar.Nome = "Pintor Profissional";

        // Act
        this.service.Edit(especialidadeParaEditar);

        // Assert
        var atualizado = this.service.Get(id);
        Assert.IsNotNull(atualizado);
        Assert.AreEqual("Pintor Profissional", atualizado.Nome);
    }

    [TestMethod]
    public void Delete_IdExistente_RemoveDoBanco()
    {
        // Arrange
        var especialidade = new Especialidade { Nome = "Marceneiro" };
        var id = this.service.Create(especialidade);

        // Act
        this.service.Delete(id);

        // Assert
        Assert.IsNull(this.service.Get(id));
        Assert.AreEqual(0, this.context.Especialidades.Count());
    }

    [TestMethod]
    public void GetAll_CadastrosExistentes_RetornaColecaoCompleta()
    {
        // Arrange
        this.service.Create(new Especialidade { Nome = "Jardineiro" });
        this.service.Create(new Especialidade { Nome = "Gesseiro" });

        // Act
        var lista = this.service.GetAll();

        // Assert
        Assert.IsNotNull(lista);
        Assert.AreEqual(2, lista.Count());
    }

    [TestMethod]
    public void GetByNome_FiltroValido_RetornaRegistrosFiltrados()
    {
        // Arrange
        this.service.Create(new Especialidade { Nome = "Eletricista Residencial" });
        this.service.Create(new Especialidade { Nome = "Eletricista Industrial" });
        this.service.Create(new Especialidade { Nome = "Pedreiro" });

        // Act
        var resultado = this.service.GetByNome("Eletricista");

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Count());
    }
}
