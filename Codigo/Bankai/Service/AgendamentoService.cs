using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class AgendamentoService : IAgendamentoService
{
    private const string EmExecucao = "E";
    private const string Concluido = "C";
    private const string Cancelado = "CA";

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
        agendamento.StatusAgendamento = EmExecucao;

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

        atual.DiaSemana = agendamento.DiaSemana;
        atual.HorarioInicio = agendamento.HorarioInicio;
        atual.HorarioFim = agendamento.HorarioFim;
        atual.DescricaoServico = agendamento.DescricaoServico;
        atual.TipoCobranca = agendamento.TipoCobranca;

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
    /// Salva avaliação no banco de dados
    /// </summary>
    /// <param name="idAgendamento"></param>
    /// <param name="nota"></param>
    /// <param name="comentario"></param>
    public void Avaliar(int idAgendamento, string nota, string? comentario)
    {
        var ag = ObterOuFalhar(idAgendamento);
        ExigirStatus(ag, Concluido, "Só é possível avaliar agendamentos concluídos.");

        ag.Nota = nota.ToString();
        ag.Comentario = comentario;
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
            DescricaoServico = a.DescricaoServico,
            TipoCobranca = a.TipoCobranca,
            StatusAgendamento = a.StatusAgendamento,
            DiaSemana = a.DiaSemana,
            HorarioInicio = a.HorarioInicio,
            HorarioFim = a.HorarioFim,
            Nota = a.Nota,
            Comentario = a.Comentario
        })
        .ToList();
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
        var disponivel = await _context.Disponibilidades.AnyAsync(d =>
            d.IdPessoa == agendamento.IdProfissional &&
            d.DiaSemana == agendamento.DiaSemana &&
            d.StatusDisponibilidade == "D" &&
            d.HorarioInicio <= agendamento.HorarioInicio &&
            d.HorarioFim >= agendamento.HorarioFim);

        if (!disponivel)
            throw new InvalidOperationException("Profissional indisponível neste dia/horário.");

        // Regra 4: conflito com outro agendamento
        var conflito = await _context.Agendamentos.AnyAsync(a =>
            a.Id != agendamento.Id &&
            a.DiaSemana == agendamento.DiaSemana &&
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
}