using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class AgendamentoServiceTests
{
    private BankaiContext context = null!;
    private IAgendamentoService servicoAgendamento = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var options = new DbContextOptionsBuilder<BankaiContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new BankaiContext(options);

        context.Pessoas.AddRange(
            CriarPessoa(1, "Pessoa 1", false),
            CriarPessoa(2, "Pessoa 2", false),
            CriarPessoa(3, "Pessoa 3", false),
            CriarPessoa(5, "Pessoa 5", true),
            CriarPessoa(6, "Pessoa 6", false),
            CriarPessoa(7, "Pessoa 7", true),
            CriarPessoa(8, "Pessoa 8", false));

        context.Agendamentos.AddRange(
            new Agendamento {
                Id = 1,
                IdCliente = 3,
                IdProfissional = 5,
                DescricaoServico = "Serviço 1",
                TipoCobranca = "F",
                DiaSemana = "QF",
                HorarioInicio = TimeSpan.FromHours(10),
                HorarioFim = TimeSpan.FromHours(15),
                StatusAgendamento = "E",
                Nota = "MR" },
            new Agendamento
            {
                Id = 2,
                IdCliente = 6,
                IdProfissional = 1,
                DescricaoServico = "Serviço 2",
                TipoCobranca = "O",
                DiaSemana = "SXF",
                HorarioInicio = TimeSpan.FromHours(9),
                HorarioFim = TimeSpan.FromHours(10),
                StatusAgendamento = "C",
                Nota = "B" },
            new Agendamento
            {
                Id = 3,
                IdCliente = 2,
                IdProfissional = 3,
                DescricaoServico = "Serviço 3",
                TipoCobranca = "F",
                DiaSemana = "Q",
                HorarioInicio = TimeSpan.FromHours(13),
                HorarioFim = TimeSpan.FromHours(16),
                StatusAgendamento = "E",
                Nota = "MB"
            });
           

        context.Disponibilidades.Add(
            new Disponibilidade
            {
                Id = 1,
                IdPessoa = 7,
                DiaSemana = "S",
                StatusDisponibilidade = "D",
                HorarioInicio = TimeSpan.FromHours(8),
                HorarioFim = TimeSpan.FromHours(18)
            });

        context.Disponibilidades.Add(
            new Disponibilidade
            {
                Id = 2,
                IdPessoa = 5,
                DiaSemana = "QF",
                StatusDisponibilidade = "D",
                HorarioInicio = TimeSpan.FromHours(8),
                HorarioFim = TimeSpan.FromHours(18)
            });

        context.SaveChanges();
        servicoAgendamento = new AgendamentoService(context);
    }

    [TestCleanup]
    public void Finalizar()
    {
        context.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarId()
    {
        var agendamento = new Agendamento
        {
            Id = 4,
            IdCliente = 8,
            IdProfissional = 7,
            DescricaoServico = "Serviço 4",
            TipoCobranca = "F",
            DiaSemana = "S",
            HorarioInicio = TimeSpan.FromHours(9),
            HorarioFim = TimeSpan.FromHours(10),
            StatusAgendamento = "",
            Nota = ""
        };

        var idRetornado = servicoAgendamento
            .Create(agendamento)
            .GetAwaiter()
            .GetResult();
        
        Assert.AreEqual(4, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarAgendamento()
    {
        // Arrange
        var agendamentoEsperado = CriarAgendamento();

        // Act
        servicoAgendamento
            .Create(agendamentoEsperado)
            .GetAwaiter()
            .GetResult();

        // Assert
        var agendamentoCriado = servicoAgendamento.Get(agendamentoEsperado.Id);

        Assert.IsNotNull(agendamentoCriado);
        Assert.AreEqual(agendamentoEsperado.Id, agendamentoCriado.Id);
        Assert.AreEqual(agendamentoEsperado.IdCliente, agendamentoCriado.IdCliente);
        Assert.AreEqual(agendamentoEsperado.IdProfissional, agendamentoCriado.IdProfissional);
        Assert.AreEqual(agendamentoEsperado.HorarioInicio, agendamentoCriado.HorarioInicio);
        Assert.AreEqual(agendamentoEsperado.HorarioFim, agendamentoCriado.HorarioFim);
        Assert.AreEqual(agendamentoEsperado.DiaSemana, agendamentoCriado.DiaSemana);
        Assert.AreEqual(agendamentoEsperado.DescricaoServico, agendamentoCriado.DescricaoServico);
        Assert.AreEqual(agendamentoEsperado.TipoCobranca, agendamentoCriado.TipoCobranca);
        Assert.AreEqual("E", agendamentoCriado.StatusAgendamento);
        Assert.AreEqual(agendamentoEsperado.Nota, agendamentoCriado.Nota);
    }

    [TestMethod]
    public void Create_DeveArmazenarAgendamentoEmExecucao()
    {
        var agendamento = new Agendamento
        {
            Id = 4,
            IdCliente = 8,
            IdProfissional = 7,
            DescricaoServico = "Serviço 4",
            TipoCobranca = "F",
            DiaSemana = "S",
            HorarioInicio = TimeSpan.FromHours(9),
            HorarioFim = TimeSpan.FromHours(10),
            StatusAgendamento = "",
            Nota = ""
        };

        // Act
        servicoAgendamento
            .Create(agendamento)
            .GetAwaiter()
            .GetResult();

        // Assert
        var agendamentoCriado = servicoAgendamento.Get(4)!;

        Assert.AreEqual(4, agendamentoCriado.Id);
        Assert.AreEqual(8, agendamentoCriado.IdCliente);
        Assert.AreEqual(7, agendamentoCriado.IdProfissional);
        Assert.AreEqual("Serviço 4", agendamentoCriado.DescricaoServico);
        Assert.AreEqual("E", agendamentoCriado.StatusAgendamento);
    }

    [TestMethod]
    public void Create_QuandoHoraFimEhMenorQueHoraInicio_DeveRecusarAgendamento()
    {
        var agendamento = new Agendamento
        {
            Id = 4,
            IdCliente = 8,
            IdProfissional = 7,
            DescricaoServico = "Horário inválido",
            TipoCobranca = "F",
            DiaSemana = "S",
            HorarioInicio = TimeSpan.FromHours(14),
            HorarioFim = TimeSpan.FromHours(10),
            StatusAgendamento = "",
            Nota = ""
        };

        servicoAgendamento
            .Create(agendamento)
            .GetAwaiter()
            .GetResult();

        var agendamentoCriado = servicoAgendamento.Get(4)!;

        Assert.IsLessThan(
            agendamentoCriado.HorarioInicio,
            agendamentoCriado.HorarioFim,
            "HoraFim não pode ser menor que HoraInicio.");
    }

    [TestMethod]
    public void Edit_DeveAtualizarAgendamentoEmExecucao()
    {
        var agendamento = servicoAgendamento.Get(1)!;
        agendamento.DescricaoServico = "Descrição atualizada";
        agendamento.TipoCobranca = "O";
        agendamento.DiaSemana = "QF";
        agendamento.HorarioInicio = TimeSpan.FromHours(11);
        agendamento.HorarioFim = TimeSpan.FromHours(14);

        servicoAgendamento.Edit(agendamento);

        var agendamentoAtualizado = servicoAgendamento.Get(1)!;

        Assert.AreEqual("Descrição atualizada", agendamentoAtualizado.DescricaoServico);
        Assert.AreEqual("O", agendamentoAtualizado.TipoCobranca);
        Assert.AreEqual(TimeSpan.FromHours(11), agendamentoAtualizado.HorarioInicio);
        Assert.AreEqual(TimeSpan.FromHours(14), agendamentoAtualizado.HorarioFim);
    }

    [TestMethod]
    public void Delete_DeveCancelarAgendamentoEmExecucao()
    {
        servicoAgendamento.Delete(1);

        var agendamentoCancelado = servicoAgendamento.Get(1)!;

        Assert.AreEqual("CA", agendamentoCancelado.StatusAgendamento);
    }


    [TestMethod]
    public void Avaliar_DeveGravarNotaEComentarioNoAgendamentoConcluido()
    {
        servicoAgendamento.Avaliar(2, "B", "Bom atendimento");

        var agendamentoAvaliado = servicoAgendamento.Get(2)!;

        Assert.AreEqual("B", agendamentoAvaliado.Nota);
        Assert.AreEqual("Bom atendimento", agendamentoAvaliado.Comentario);
    }

    [TestMethod]
    public void Avaliar_QuandoNotaNaoFoiInformada_DeveRecusarAvaliacao()
    {
        servicoAgendamento.Avaliar(2, "", null);

        var agendamentoAvaliado = servicoAgendamento.Get(2)!;

        Assert.IsFalse(
            !string.IsNullOrEmpty(agendamentoAvaliado.Nota),
            "Nota não pode ser vazia.");
    }

    [TestMethod]
    public void Get_DeveRetornarAgendamentoPeloId()
    {
        var agendamento = servicoAgendamento.Get(1)!;

        Assert.AreEqual(1, agendamento.Id);
        Assert.AreEqual(3, agendamento.IdCliente);
        Assert.AreEqual(5, agendamento.IdProfissional);
        Assert.AreEqual("Serviço 1", agendamento.DescricaoServico);
    }

    [TestMethod]
    public void GetAllByUser_DeveRetornarAgendamentosDoClienteOuProfissional()
    {
        var agendamentos = servicoAgendamento.GetAllByUser(3).ToList();

        Assert.HasCount(2, agendamentos);
        Assert.AreEqual(3, agendamentos[0].Id);
        Assert.AreEqual(1, agendamentos[1].Id);
    }

    [TestMethod]
    public void GetAllByUser_DeveRetornarColecaoVaziaParaPessoaSemAgendamentos()
    {
        var agendamentos = servicoAgendamento.GetAllByUser(999).ToList();

        Assert.IsNotNull(agendamentos);
        Assert.IsEmpty(agendamentos);
    }

    private static Pessoa CriarPessoa(int id, string nome, bool ehProfissional)
    {
        return new Pessoa
        {
            Id = id,
            Nome = nome,
            CpfCnpj = $"000000000{id}",
            Email = $"pessoa{id}@teste.com",
            EhProfissional = ehProfissional,
            Cep = "00000000",
            Rua = "Rua de teste",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Estado = "SP",
            NumeroCasa = "1"
        };
    }

    private static Agendamento CriarAgendamento()
    {
        return new Agendamento
        {
            Id = 4,
            IdCliente = 6,
            IdProfissional = 7,
            DiaSemana = "S",
            HorarioInicio = TimeSpan.FromHours(9),
            HorarioFim = TimeSpan.FromHours(12),
            DescricaoServico = "Trocar lampada",
            TipoCobranca = "F",
            StatusAgendamento = "",
            Nota = ""
        };
    }
}