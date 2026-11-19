using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class AgendamentoService : IAgendamentoService
{
    private const string Novo = "Novo";
    private const string Cancelado = "Cancelado";
    private const string Execucao = "Execucao";
    private const string Concluido = "Concluido";

    private readonly BankaiContext _context;
    public AgendamentoService(BankaiContext context) => _context = context;

    /// <summary>
    /// Cria um agendamento no banco de dados
    /// </summary>
    /// <param name="agendamento"></param>
    /// <returns>id do agendamento</returns>
    public async Task<int> Create(Agendamento agendamento)
    {
        await Validar(agendamento);

        agendamento.StatusAgendamento = "Novo";

        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
        return agendamento.Id;
    }

    /// <summary>
    /// Modifica um agendamento no banco de dados
    /// </summary>
    /// <param name="agendamento"></param>
    public void Edit(Agendamento agendamento)
    {
        var atual = ObterOuFalhar(agendamento.Id);
        ExigirStatus(atual, "Só é possível editar agendamentos que ainda não foram iniciados.", Novo);

        //Esses são os unicos campos que podem ser alterados. A ver no futuro se isso muda.
        atual.Data = agendamento.Data;
        atual.HorarioInicio = agendamento.HorarioInicio;
        atual.HorarioFim = agendamento.HorarioFim;
        atual.TipoCobranca = agendamento.TipoCobranca;
        atual.Comentario = agendamento.Comentario;

        Validar(atual).GetAwaiter().GetResult();
        _context.SaveChanges();
    }

    /// <summary>
    /// Deleta/ cancela um agendamento
    /// </summary>
    /// <param name="idAgendamento"></param>
    public void Delete(int idAgendamento)
    {
        var ag = ObterOuFalhar(idAgendamento);
        ExigirStatus(ag, "Só é possível cancelar agendamentos que não foram executados.", Novo);
        ag.StatusAgendamento = Cancelado;
        _context.SaveChanges();
    }
       
    /// <summary>
    /// Consulta agendamento no banco de dados
    /// </summary>
    /// <param name="idAgendamento"></param>
    /// <returns>Dados agendamento</returns>
    public Agendamento Get(int idAgendamento)
    {
        return _context.Agendamentos.Find(idAgendamento);
    }

    public IEnumerable <AgendamentoDTO> GetAllByUser(int idPessoa)
    {
        //retorna o DTO
        return _context.Agendamentos
        .Where(a => a.IdCliente == idPessoa || a.IdProfissional == idPessoa)
        .OrderByDescending(a => a.Id)
        .Select(a => new AgendamentoDTO
        {
            Id = a.Id,
            IdCliente = a.IdCliente,
            NomeCliente = a.IdClienteNavigation.Nome,
            IdProfissional = a.IdProfissional,
            NomeProfissional = a.IdProfissionalNavigation.Nome,
            TipoCobranca = a.TipoCobranca,
            StatusAgendamento = a.StatusAgendamento,
            Data = a.Data,
            HorarioInicio = a.HorarioInicio,
            HorarioFim = a.HorarioFim,
            Comentario = a.Comentario
        });
    }

    public async Task Validar(Agendamento agendamento)
    {
        // Regra 1: cliente e profissional não podem ser a mesma pessoa
        if (agendamento.IdCliente == agendamento.IdProfissional)
            throw new InvalidOperationException("Cliente e profissional não podem ser a mesma pessoa.");

        // Regra 2: horário final maior que inicial
        if (agendamento.HorarioFim <= agendamento.HorarioInicio)
            throw new InvalidOperationException("O horário final deve ser maior que o inicial.");

        // Regra 3: horários devem estar no mesmo dia da data do agendamento
        var data = agendamento.Data.Date;

        if (agendamento.HorarioInicio.Date != data || agendamento.HorarioFim.Date != data)
            throw new InvalidOperationException("Os horários devem estar no dia da data do agendamento.");

        // Regra 4: não permitir agendamento em datas passadas e horários passados 
        if (agendamento.Data.Date < DateTime.Today ||
               (agendamento.Data.Date == DateTime.Today && agendamento.HorarioInicio < DateTime.Now))
            throw new InvalidOperationException("Não é permitido agendar em datas ou horários passados.");

        // Regra 4.1: só é possível agendar dentro da semana atual (segunda a domingo)
        var inicioSemana = InicioDaSemana(DateTime.Today);
        var fimSemana = inicioSemana.AddDays(6);

        if (agendamento.Data.Date < inicioSemana || agendamento.Data.Date > fimSemana)
            throw new InvalidOperationException("Só é possível agendar para a semana atual.");

        // Regra 5: profissional disponível nesse dia/horário
        var diaSemana = ObterDiaSemanaString(agendamento.Data.DayOfWeek);

        var disponivel = await _context.Disponibilidades.AnyAsync(d =>
            d.IdPessoa == agendamento.IdProfissional &&
            d.StatusDisponibilidade == "D" &&
            d.DiaSemana == diaSemana &&
            d.HorarioInicio <= agendamento.HorarioInicio.TimeOfDay &&
            d.HorarioFim >= agendamento.HorarioFim.TimeOfDay);

        if (!disponivel)
            throw new InvalidOperationException("Profissional indisponível neste dia/horário.");

        // Regra 6: conflito com outro agendamento
        var conflito = await _context.Agendamentos.AnyAsync(a =>
            a.Id != agendamento.Id &&
            a.Data == agendamento.Data &&
            a.StatusAgendamento != Cancelado && //caso um agendamento esteja cancelado no mesmo dia e hora, não há conflito
            (a.IdProfissional == agendamento.IdProfissional || a.IdCliente == agendamento.IdCliente) &&
            a.HorarioInicio < agendamento.HorarioFim &&
            a.HorarioFim > agendamento.HorarioInicio);

        if (conflito)
            throw new InvalidOperationException("Já existe um agendamento neste horário.");

    }

    // ---------- auxiliares ----------
    /// <summary>
    /// Marca um agendamento novo como em execução
    /// </summary>
    public void Iniciar(int idAgendamento)
    {
        var ag = ObterOuFalhar(idAgendamento);
        ExigirStatus(ag, "Só é possível iniciar agendamentos novos.", Novo);
        ag.StatusAgendamento = Execucao;
        _context.SaveChanges();
    }

    /// <summary>
    /// Marca um agendamento em execução como concluído
    /// </summary>
    public void Concluir(int idAgendamento)
    {
        var ag = ObterOuFalhar(idAgendamento);
        ExigirStatus(ag, "Só é possível concluir agendamentos em execução.", Execucao);
        ag.StatusAgendamento = Concluido;
        _context.SaveChanges();
    }

    //verifica se o agendamento existe, caso contrário lança exceção
    private Agendamento ObterOuFalhar(int id)
    {
        return _context.Agendamentos.FirstOrDefault(a => a.Id == id)
        ?? throw new KeyNotFoundException("Agendamento não encontrado.");
    }

    //vefica se o agendamento está no status esperado, caso contrário lança exceção
    private static void ExigirStatus(Agendamento ag, string msg, string esperado)
    {
        if (ag.StatusAgendamento != esperado)
            throw new InvalidOperationException(msg);
    }

    // retornar strings do dia da semana para comparação com a tabela de disponibilidade
    public static string ObterDiaSemanaString(DayOfWeek dia)
    {
        return dia switch
        {
            DayOfWeek.Sunday => "D",
            DayOfWeek.Monday => "SF",
            DayOfWeek.Tuesday => "TF",
            DayOfWeek.Wednesday => "QF",
            DayOfWeek.Thursday => "QIF",
            DayOfWeek.Friday => "SXF",
            DayOfWeek.Saturday => "S",
            _ => throw new ArgumentOutOfRangeException(nameof(dia), dia, null)
        };
    }
    // converte o código do dia ("SF", "TF"...) em DayOfWeek
    public static DayOfWeek ObterDayOfWeek(string codigo)
    {
        return codigo switch
        {
            "D" => DayOfWeek.Sunday,
            "SF" => DayOfWeek.Monday,
            "TF" => DayOfWeek.Tuesday,
            "QF" => DayOfWeek.Wednesday,
            "QIF" => DayOfWeek.Thursday,
            "SXF" => DayOfWeek.Friday,
            "S" => DayOfWeek.Saturday,
            _ => throw new InvalidOperationException("Dia da semana inválido.")
        };
    }

    // Segunda = 0 ... Domingo = 6
    private static int PosicaoNaSemana(DayOfWeek dia) => ((int)dia + 6) % 7;

    // segunda-feira da semana da data informada
    private static DateTime InicioDaSemana(DateTime data) =>
        data.Date.AddDays(-PosicaoNaSemana(data.DayOfWeek));

    /// <summary>
    /// Retorna a data do dia informado dentro da semana atual (segunda a domingo).
    /// Pode ser uma data passada; quem barra é o Validar.
    /// </summary>
    public DateTime ObterDataNaSemanaAtual(string diaSemana)
    {
        var alvo = ObterDayOfWeek(diaSemana);
        return InicioDaSemana(DateTime.Today).AddDays(PosicaoNaSemana(alvo));
    }
}