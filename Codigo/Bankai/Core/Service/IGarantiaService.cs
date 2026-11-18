using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IGarantiaService
    {
        uint Create(Chamadogarantium chamadoGarantia);
        void Edit(Chamadogarantium chamadoGarantia);
        void Delete(uint id);
        Chamadogarantium? Get(uint id);
        IEnumerable<Chamadogarantium> GetAll();
        IEnumerable<Chamadogarantium> GetByIdAgendamento(uint idAgendamento);
    }
}
