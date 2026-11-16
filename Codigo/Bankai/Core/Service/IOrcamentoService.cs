using Core.DTO;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IOrcamentoService
    {
        uint Create(Orcamento orcamento);

        void Edit(Orcamento orcamento);

        void Delete(uint Id);

        Orcamento? Get(uint Id);

        IEnumerable<Orcamento> GetAll();



        bool EnviarOrcamento(uint Id, uint idAgendamento, decimal valorProposto, uint validade, string descricao, byte[] MateriaisAdicionais);

     
        decimal CalcularTaxaPlataforma(decimal valorBruto);

        decimal CalcularValorLiquido(decimal valorBruto);
    }
}

