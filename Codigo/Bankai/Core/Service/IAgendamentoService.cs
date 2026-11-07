using Core.DTO;

namespace Core.Service;

public interface IAgendamentoService
{
    Task<int> Create(CriarAgendamentoDto dto);
    void Edit(Agendamento agendamento);
    void Delete(int idAgendamento);
    void Avaliar(AvaliarAgendamentoDto dto);
    Agendamento Get(int idAgendamento);
    Task<IEnumerable<Agendamento>> GetByPessoa(int idPessoa);
    Task<IEnumerable<Agendamento>> GetAllByCpfCnpj(string cpfCnpj);
}