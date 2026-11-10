using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IOrcarServicoService
    {
        uint Create(Agendamentoservico orcamento);
        void Edit(Agendamentoservico orcamento);
        void Delete(uint Id);
        Agendamentoservico? Get(uint Id);

        IEnumerable<Agendamentoservico> GetAll();


    }
}
