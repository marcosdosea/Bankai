using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class AgendamentoService : IAgendamentoService
{
    private const string EmExecucao = "Execucao";
    private const string Concluido = "Concluido";
    private const string Cancelado = "Cancelado";

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
        ExigirStatus(atual, EmExecucao, "Só é possível editar agendamentos em execução.");

        _context.Update(agendamento);
        Validar(atual).Wait();
        _context.SaveChanges();
    }

    /// <summary>
    /// Deleta/ cancela um agendamento
    /// </summary>
    /// <param name="idAgendamento"></param>
    public void Delete(int idAgendamento)
    {
        var ag = ObterOuFalhar(idAgendamento);
        ExigirStatus(ag, EmExecucao, "Só é possível cancelar agendamentos em execução.");
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

        // Regra 3: profissional disponível nesse dia/horário
        var diaSemana = ObterDiaSemanaString(agendamento.Data.DayOfWeek);

        var disponivel = await _context.Disponibilidades.AnyAsync(d =>
            d.IdPessoa == agendamento.IdProfissional &&
            d.StatusDisponibilidade == "D" &&
            d.DiaSemana == diaSemana &&
            d.HorarioInicio <= agendamento.HorarioInicio.TimeOfDay &&
            d.HorarioFim >= agendamento.HorarioFim.TimeOfDay);

        if (!disponivel)
            throw new InvalidOperationException("Profissional indisponível neste dia/horário.");

        // Regra 4: conflito com outro agendamento
        var conflito = await _context.Agendamentos.AnyAsync(a =>
            a.Id != agendamento.Id &&
            a.Data == agendamento.Data &&
            a.StatusAgendamento != Cancelado &&
            (a.IdProfissional == agendamento.IdProfissional || a.IdCliente == agendamento.IdCliente) &&
            a.HorarioInicio < agendamento.HorarioFim &&
            a.HorarioFim > agendamento.HorarioInicio);

        if (conflito)
            throw new InvalidOperationException("Já existe um agendamento neste horário.");
    }

    // ---------- auxiliares ----------

    //verifica se o agendamento existe, caso contrário lança exceção
    private Agendamento ObterOuFalhar(int id)
    {
        return _context.Agendamentos.FirstOrDefault(a => a.Id == id)
        ?? throw new KeyNotFoundException("Agendamento não encontrado.");
    }

    //vefica se o agendamento está no status esperado, caso contrário lança exceção
    private static void ExigirStatus(Agendamento ag, string esperado, string msg)
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
}