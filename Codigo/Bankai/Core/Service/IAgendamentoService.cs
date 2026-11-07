using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IAgendamentoService
    {
        Task<int> Create(Agendamento agendamento);
        void Edit(Agendamento agendamento);
        void Delete(int idAgendamento);
        Agendamento GetById(int idAgendamento);
    }
}
