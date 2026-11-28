using System;
using System.Collections.Generic;
using System.Text;
using Core;

namespace Core.Service
{
    public interface IPagamentoService
    {
        int Create(Pagamento pagamento);
        void Edit(Pagamento pagamento);
        void Delete(int id);
        Pagamento? Get(int id);
        IEnumerable<Pagamento> GetAll();
        IEnumerable<Pagamento> GetByAgendamento(int idAgendamento);
        Pagamento RealizarPagamento(Pagamento pagamento, bool transacaoCompleta = true);
    }
}
