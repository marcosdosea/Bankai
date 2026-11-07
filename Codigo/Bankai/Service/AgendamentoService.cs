using Core;
using Core.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class AgendamentoService : IAgendamentoService
    {
        private readonly BankaiContext _context;
        public AgendamentoService(BankaiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// cria um agendamento no banco de dados
        /// </summary>
        /// <param name="agendamento">Dados do agendamento</param>
        /// <returns>Id do agendamento</returns>
        public async Task<int> Create(Agendamento agendamento)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            return agendamento.Id;
        }

        public void Delete(int idAgendamento)
        {
            throw new NotImplementedException();
        }

        public void Edit(Agendamento agendamento)
        {
            throw new NotImplementedException();
        }

        public Agendamento GetById(int idAgendamento)
        {
            throw new NotImplementedException();
        }
    }
}
