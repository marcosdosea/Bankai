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
                TipoCobranca = "F",
                Data = DateTime.Today.AddDays(4),
                HorarioInicio = DateTime.Today.AddHours(15).AddDays(4),
                HorarioFim = DateTime.Today.AddHours(17).AddDays(4),
                StatusAgendamento = "Novo",
                Foto = null
            },
            new Agendamento
            {
                Id = 2,
                IdCliente = 6,
                IdProfissional = 1,
                TipoCobranca = "O",
                Data = DateTime.Today.AddDays(3),
                HorarioInicio = DateTime.Today.AddHours(9),
                HorarioFim = DateTime.Today.AddHours(15),
                StatusAgendamento = "Concluido",
                },
            new Agendamento
            {
                Id = 3,
                IdCliente = 2,
                IdProfissional = 3,
                TipoCobranca = "F",
                Data = DateTime.Today.AddDays(1),
                HorarioInicio = DateTime.Today.AddHours(13),
                HorarioFim = DateTime.Today.AddHours(16),
                StatusAgendamento = "Execucao",
            });
           

        context.Disponibilidades.Add(
            new Disponibilidade
            {
                Id = 1,
                IdPessoa = 7,
                DiaSemana = "D",
                StatusDisponibilidade = "D",
                HorarioInicio = TimeSpan.FromHours(8),
                HorarioFim = TimeSpan.FromHours(18)
            });

        context.Disponibilidades.Add(
            new Disponibilidade
            {
                Id = 3,
                IdPessoa = 7,
                DiaSemana = "QF",
                StatusDisponibilidade = "D",
                HorarioInicio = TimeSpan.FromHours(8),
                HorarioFim = TimeSpan.FromHours(18)
            });

        context.Disponibilidades.Add(
            new Disponibilidade
            {
                Id = 2,
                IdPessoa = 5,
                DiaSemana = "S",
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
        var agendamento = CriarAgendamento();

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
        Assert.AreEqual(agendamentoEsperado.Data, agendamentoCriado.Data);
        Assert.AreEqual(agendamentoEsperado.HorarioInicio, agendamentoCriado.HorarioInicio);
        Assert.AreEqual(agendamentoEsperado.HorarioFim, agendamentoCriado.HorarioFim);
        Assert.AreEqual(agendamentoEsperado.TipoCobranca, agendamentoCriado.TipoCobranca);

    }

    [TestMethod]
    public void Create_QuandoHoraFimEhMenorQueHoraInicio_DeveRecusarAgendamento()
    {
        var agendamento = new Agendamento
        {
            Id = 4,
            IdCliente = 8,
            IdProfissional = 7,
            TipoCobranca = "F",
            Data = DateTime.Today,
            HorarioInicio = DateTime.Today.AddHours(14),
            HorarioFim = DateTime.Today.AddHours(10),
            Foto = null
        };

        var excecao = Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            servicoAgendamento.Create(agendamento))
            .GetAwaiter()
            .GetResult();

        Assert.AreEqual("O horário final deve ser maior que o inicial.", excecao.Message);
        Assert.IsNull(servicoAgendamento.Get(4));
    }

    [TestMethod]
    public void Edit_EditarAgendamentoExistente_DeveAtualizarAgendamento()
    {
        var agendamento = servicoAgendamento.Get(1)!;
        agendamento.TipoCobranca = "O";
        agendamento.Comentario = "Comentário atualizado";
        servicoAgendamento.Edit(agendamento);
        var agendamentoAtualizado = servicoAgendamento.Get(1)!;
        Assert.AreEqual("O", agendamentoAtualizado.TipoCobranca);
        Assert.AreEqual("Comentário atualizado", agendamentoAtualizado.Comentario);
    }

    [TestMethod]
    public void Edit_QuandoAgendamentoNovo_DeveAtualizarSeusDados()
    {
        // Arrange (objeto novo, como o controller envia: não é a instância rastreada pelo contexto)
        var edicao = CriarEdicao(1);

        // Act
        servicoAgendamento.Edit(edicao);

        // Assert
        var atualizado = servicoAgendamento.Get(1)!;

        Assert.AreEqual(edicao.Data, atualizado.Data);
        Assert.AreEqual(edicao.HorarioInicio, atualizado.HorarioInicio);
        Assert.AreEqual(edicao.HorarioFim, atualizado.HorarioFim);
        Assert.AreEqual("O", atualizado.TipoCobranca);
        Assert.AreEqual("Levar escada", atualizado.Comentario);
        Assert.AreEqual("Novo", atualizado.StatusAgendamento);
    }

    [TestMethod]
    public void Edit_QuandoNovoHorarioEstaForaDaDisponibilidade_DeveRecusarEdicao()
    {
        // Arrange (profissional 5 atende das 8h às 18h)
        var sabado = ProximaOcorrencia(DayOfWeek.Saturday);
        var edicao = CriarEdicao(1);
        edicao.HorarioInicio = sabado.AddHours(19);
        edicao.HorarioFim = sabado.AddHours(21);

        // Act
        var excecao = Assert.ThrowsExactly<InvalidOperationException>(() =>
            servicoAgendamento.Edit(edicao));

        // Assert
        Assert.AreEqual("Profissional indisponível neste dia/horário.", excecao.Message);
    }

    [TestMethod]
    public void Delete_DeveCancelarAgendamentoNovo_DeveMarcarComoCancelado()
    {
        servicoAgendamento.Delete(1);
        var agendamentoCancelado = servicoAgendamento.Get(1)!;
        Assert.AreEqual("Cancelado", agendamentoCancelado.StatusAgendamento);
    }

    [TestMethod]
    public void Delete_QuandoAgendamentoCancelado_DeveLiberarOHorarioParaNovoAgendamento()
    {
        // Arrange (o agendamento 1 ocupa o profissional 5 no sábado, das 15h às 17h)
        var sabado = ProximaOcorrencia(DayOfWeek.Saturday);
        servicoAgendamento.Delete(1);

        var novo = new Agendamento
        {
            IdCliente = 8,
            IdProfissional = 5,
            TipoCobranca = "F",
            Data = sabado,
            HorarioInicio = sabado.AddHours(15),
            HorarioFim = sabado.AddHours(17)
        };

        // Act
        var id = servicoAgendamento
            .Create(novo)
            .GetAwaiter()
            .GetResult();

        // Assert
        var criado = servicoAgendamento.Get(id)!;

        Assert.AreEqual(8, criado.IdCliente);
        Assert.AreEqual("Novo", criado.StatusAgendamento);
    }

    [TestMethod]
    public void Delete_QuandoAgendamentoEmExecucao_DeveRecusarCancelamento()
    {
        // Act
        var excecao = Assert.ThrowsExactly<InvalidOperationException>(() =>
            servicoAgendamento.Delete(3));

        // Assert
        Assert.AreEqual("Só é possível cancelar agendamentos que não foram executados.", excecao.Message);
        Assert.AreEqual("Execucao", servicoAgendamento.Get(3)!.StatusAgendamento);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        var agendamento = servicoAgendamento.Get(999);
        Assert.IsNull(agendamento);
    }

    [TestMethod]
    public void Get_DeveRetornarAgendamentoPeloId()
    {
        var agendamento = servicoAgendamento.Get(1)!;

        Assert.AreEqual(1, agendamento.Id);
        Assert.AreEqual(3, agendamento.IdCliente);
        Assert.AreEqual(5, agendamento.IdProfissional);
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
        var hoje = DateTime.Today;
        var diasAteQuarta = ((int)DayOfWeek.Wednesday - (int)hoje.DayOfWeek + 7) % 7;
        var dt = hoje.AddDays(diasAteQuarta);
        return new Agendamento
        {
            Id = 4,
            IdCliente = 6,
            IdProfissional = 7,
            Data = dt,
            HorarioInicio = dt.AddHours(9),
            HorarioFim = dt.AddHours(12),
            TipoCobranca = "F",
        };
    }
    private static DateTime ProximaOcorrencia(DayOfWeek dia)
    {
        var hoje = DateTime.Today;
        var dias = 1 + ((int)dia - (int)hoje.DayOfWeek + 6) % 7;

        return hoje.AddDays(dias);
    }
    private static Agendamento CriarEdicao(int id)
    {
        var sabado = ProximaOcorrencia(DayOfWeek.Saturday);

        return new Agendamento
        {
            Id = id,
            IdCliente = 3,
            IdProfissional = 5,
            TipoCobranca = "O",
            Comentario = "Levar escada",
            Data = sabado,
            HorarioInicio = sabado.AddHours(10),
            HorarioFim = sabado.AddHours(12)
        };
    }
}