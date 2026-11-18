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
        void Delete(uint idPagamento);
        Pagamento? get(uint idPagamento);
        IEnumerable<Pagamento> getAll();
        IEnumerable<Pagamento> getByAgendamento(int idAgendamento);
        Pagamento RealizarPagamento(Pagamento pagamento, bool transacaoCompleta = true);
    }
}
