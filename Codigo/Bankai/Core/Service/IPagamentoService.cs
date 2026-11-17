using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IPagamentoService
    {
        uint Create(Pagamento pagamento);
        void Edit(Pagamento pagamento);
        void Delete(uint idPagamento);
        Pagamento? get(uint idPagamento);
        IEnumerable<Pagamento> getAll();
        IEnumerable<Pagamento> getByAgendamento(uint idAgendamento);
        Pagamento RealizarPagamento(Pagamento pagamento, bool transacaoCompleta = true);
    }
}
