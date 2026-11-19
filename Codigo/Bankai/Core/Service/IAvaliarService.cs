using System.Collections.Generic;

namespace Core.Service
{
    public interface IAvaliarService
    {
        int Create(Avaliacao avaliacao);
        void Edit(Avaliacao avaliacao);
        void Delete(int id);
        Avaliacao? Get(int id);
        IEnumerable<Avaliacao> GetAll();
        Avaliacao Avaliar(int idAgendamento, int pontualidade, int limpezaLocal, int qualidadeReparo, string? depoimento = null);
        Avaliacao? GetByAgendamento(int idAgendamento);
        IEnumerable<Avaliacao> GetByProfissional(int idProfissional);
        IEnumerable<Avaliacao> GetByCliente(int idCliente);
        IEnumerable<Agendamento> GetPendentesPorCliente(int idCliente);
        double CalcularMediaNota(int idProfissional);
        double CalcularBankaiRating(int idProfissional);
    }
}

