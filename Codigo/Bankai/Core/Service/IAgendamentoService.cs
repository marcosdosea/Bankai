using Core.DTO;

namespace Core.Service;

public interface IAgendamentoService
{
    Task<int> Create(Agendamento agendamento);
    void Edit(Agendamento agendamento);
    void Delete(int idAgendamento);
    void Avaliar(int idAgendamento, int nota, string? comentario);
    Agendamento Get(int idAgendamento);
    IEnumerable<Agendamento> GetByPessoa(int idPessoa);
}