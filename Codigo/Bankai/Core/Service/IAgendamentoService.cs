using Core.DTO;

namespace Core.Service;

public interface IAgendamentoService
{
    Task<int> Create(Agendamento agendamento);
    void Edit(Agendamento agendamento);
    void Delete(int idAgendamento);
    Task Validar(Agendamento agendamento);
    Agendamento Get(int idAgendamento);
    IEnumerable<AgendamentoDTO> GetAllByUser(int idPessoa);
}