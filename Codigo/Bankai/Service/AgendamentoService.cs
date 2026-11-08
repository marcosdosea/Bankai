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

    /// <summary>Cria um agendamento validando disponibilidade, conflito e serviços ofertados.</summary>
    public async Task<int> Create(Agendamento agendamento)
    {
        await Validar(agendamento);
        agendamento.StatusAgendamento = EmExecucao;

        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
        return agendamento.Id;
    }

    /// <summary>Edita um agendamento em execução.</summary>
    public void Edit(Agendamento agendamento)
    {
        var atual = ObterOuFalhar(agendamento.Id);
        ExigirStatus(atual, EmExecucao, "Só é possível editar agendamentos em execução.");

        atual.DiaSemana = agendamento.DiaSemana;
        atual.HorarioInicio = agendamento.HorarioInicio;
        atual.HorarioFim = agendamento.HorarioFim;
        atual.DescricaoServico = agendamento.DescricaoServico;
        atual.TipoCobranca = agendamento.TipoCobranca;

        _context.SaveChanges();
    }

    /// <summary>Cancelamento lógico (há FKs de Pagamento e ChamadoGarantia).</summary>
    public void Delete(int idAgendamento)
    {
        var ag = ObterOuFalhar(idAgendamento);
        ExigirStatus(ag, EmExecucao, "Só é possível cancelar agendamentos em execução.");
        ag.StatusAgendamento = Cancelado;
        _context.SaveChanges();
    }

    public void Avaliar(int idAgendamento, int nota, string? comentario)
    {
        var ag = ObterOuFalhar(idAgendamento);
        ExigirStatus(ag, Concluido, "Só é possível avaliar agendamentos concluídos.");

        ag.Nota = nota.ToString();  //Nota é ENUM no banco
        ag.Comentario = comentario;
        _context.SaveChanges();
    }

    public Agendamento Get(int idAgendamento)
    {

        return _context.Agendamentos.Find(idAgendamento)
        ?? throw new KeyNotFoundException("Agendamento não encontrado.");
    }
    public IEnumerable<Agendamento> GetByPessoa(int idPessoa)
    {
        return _context.Agendamentos
            .Where(a => a.IdCliente == idPessoa || a.IdProfissional == idPessoa)
            .OrderByDescending(a => a.Id)
            .ToList();
    }

    private async Task Validar(Agendamento agendamento)
    {
        // Regra 1: profissional disponível nesse dia/horário
        var disponivel = await _context.Disponibilidades.AnyAsync(d =>
            d.IdPessoa == agendamento.IdProfissional &&
            d.DiaSemana == agendamento.DiaSemana &&
            d.StatusDisponibilidade == "D" &&
            d.HorarioInicio <= agendamento.HorarioInicio &&
            d.HorarioFim >= agendamento.HorarioFim);

        if (!disponivel)
            throw new InvalidOperationException("Profissional indisponível neste dia/horário.");

        // Regra 2: conflito com outro agendamento
        var conflito = await _context.Agendamentos.AnyAsync(a =>
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