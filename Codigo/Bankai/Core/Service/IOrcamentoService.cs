using Core.DTO;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IOrcamentoService
    {
        uint Create(Orcamento orcamento, uint idAgendamento);

        void Edit(Orcamento orcamento);

        void Delete(uint Id);

        Orcamento? Get(uint Id);

        IEnumerable<Orcamento> GetAll();

     
        decimal CalcularTaxaPlataforma(decimal valorBruto);

        decimal CalcularValorLiquido(decimal valorBruto);
    }
}

