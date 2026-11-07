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
    /// Cria um agendamento no banco de dados, validando disponibilidade do profissional e conflito de horários.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<int> Create(CriarAgendamentoDto dto)
    {
        var precos = await Validar(dto);

        var agendamento = new Agendamento
        {
            IdCliente = dto.IdCliente,
            IdProfissional = dto.IdProfissional,
            DiaSemana = dto.Data.ToString(),
            HorarioInicio = dto.HorarioInicio,
            HorarioFim = dto.HorarioFim,
            DescricaoServico = dto.DescricaoServico,
            TipoCobranca = dto.TipoCobranca,
            StatusAgendamento = EmExecucao
        };

        foreach (var s in dto.Servicos)
            agendamento.Agendamentoservicos.Add(new Agendamentoservico
            {
                IdServico = s.IdServico,
                ValorProposto = s.ValorProposto ?? precos[s.IdServico],
                StatusServico = "E" // use o valor do seu ENUM
            });

        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync(); // agendamento + serviços na mesma transação
        return agendamento.Id;
    }
    /// <summary>
    /// edita um agendamento no banco de dados
    /// </summary>
    /// <param name="agendamento"></param>
    public void Edit(Agendamento agendamento)
    {
        ExigirStatus(agendamento, EmExecucao, "Só é possível concluir agendamentos em execução.");
        agendamento.StatusAgendamento = Concluido;
        _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// deleta um agendamento no banco de dados
    /// </summary>
    /// <param name="idAgendamento"></param>
    public void Delete(int idAgendamento)
    {   
        var _agendamento = _context.Agendamentos.Find(idAgendamento);
        _context.Remove(_agendamento);
        _context.SaveChangesAsync();
    }

    public async void Avaliar(AvaliarAgendamentoDto dto)
    {
        var ag = await ObterOuFalharAsync(dto.Id);
        ExigirStatus(ag, Concluido, "Só é possível avaliar agendamentos concluídos.");
        ag.Nota = dto.Nota.ToString();
        ag.Comentario = dto.Comentario;
        await _context.SaveChangesAsync();
    }

    public Agendamento Get(int idAgendamento)
    {
        return _context.Agendamentos.Find(idAgendamento) ?? throw new InvalidOperationException("Agendamento não encontrado.");
    }
        

    public async Task<IEnumerable<Agendamento>> GetByPessoa(int idPessoa)
    {

        return await _context.Agendamentos
            .AsNoTracking()
            .Where(a => a.IdCliente == idPessoa || a.IdProfissional == idPessoa)
            .OrderByDescending(a => a.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Agendamento>> GetAllByCpfCnpj(string cpfCnpj)
    {
        return await _context.Agendamentos
            .Include(a => a.IdClienteNavigation)
            .Include(a => a.IdProfissionalNavigation)
            .AsNoTracking()
            .Where(a => a.IdClienteNavigation.CpfCnpj == cpfCnpj || a.IdProfissionalNavigation.CpfCnpj == cpfCnpj)
            .OrderByDescending(a => a.Id)
            .ToListAsync();
    }

    // ---------- auxiliares ----------

    private async Task<Dictionary<int, decimal>> Validar(CriarAgendamentoDto dto)
    {
        var dia = ToDiaSemana(dto.Data);

        // Regra 1: profissional dispõe desse horário? (depende do banco)
        var disponivel = await _context.Disponibilidades.AnyAsync(d =>
            d.IdPessoa == dto.IdProfissional &&
            d.DiaSemana == dia &&
            d.StatusDisponibilidade == "D" &&
            d.HorarioInicio <= dto.HorarioInicio &&
            d.HorarioFim >= dto.HorarioFim);

        if (!disponivel)
            throw new InvalidOperationException("Profissional indisponível neste dia/horário.");

        // Regra 2: conflito com outro agendamento (depende do banco)
        var conflito = await _context.Agendamentos.AnyAsync(a =>
            a.DiaSemana == dia &&
            a.StatusAgendamento != Cancelado &&
            (a.IdProfissional == dto.IdProfissional || a.IdCliente == dto.IdCliente) &&
            a.HorarioInicio < dto.HorarioFim &&
            a.HorarioFim > dto.HorarioInicio);

        if (conflito)
            throw new InvalidOperationException("Já existe um agendamento neste horário.");

        // Preços padrão do profissional (também valida se ele oferece o serviço)
        var ids = dto.Servicos.Select(s => s.IdServico).ToList();
        var precos = await _context.Profissionalservicos
            .Where(ps => ps.IdPessoa == dto.IdProfissional && ids.Contains(ps.IdServico))
            .ToDictionaryAsync(ps => ps.IdServico, ps => ps.Preco);

        if (ids.Any(id => !precos.ContainsKey(id)))
            throw new InvalidOperationException("O profissional não oferece todos os serviços informados.");

        return precos;
    }

    private async Task<Agendamento> ObterOuFalharAsync(int id) =>
        await _context.Agendamentos.FirstOrDefaultAsync(a => a.Id == id)
        ?? throw new KeyNotFoundException("Agendamento não encontrado.");

    private static void ExigirStatus(Agendamento ag, string esperado, string msg)
    {
        if (ag.StatusAgendamento != esperado)
            throw new InvalidOperationException(msg);
    }

    // Use o mesmo valor que o ENUM do banco espera
    private static string ToDiaSemana(DateOnly d) => d.DayOfWeek switch
    {
        DayOfWeek.Monday => "SF",
        DayOfWeek.Tuesday => "TF",
        DayOfWeek.Wednesday => "QF",
        DayOfWeek.Thursday => "QIF",
        DayOfWeek.Friday => "SXF",
        DayOfWeek.Saturday => "S",
        _ => "D"
    };
}