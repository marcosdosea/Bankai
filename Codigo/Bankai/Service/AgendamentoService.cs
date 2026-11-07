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
            _context.Add(agendamento);
            _context.SaveChanges();
            return agendamento.Id;

        }

        public void Delete(int idAgendamento)
        {
            var agendamento = _context.Agendamentos.Find(idAgendamento);
            if(agendamento != null)
            {
                _context.Agendamentos.Remove(agendamento);
                _context.SaveChanges();
            }
        }

        public void Edit(Agendamento agendamento)
        {
            _context.Agendamentos.Update(agendamento);
            _context.SaveChanges();
        }

        public Agendamento Get(int idAgendamento)
        {
            return _context.Agendamentos.Find(idAgendamento);
        }

        public IEnumerable<Agendamento> GetAll()
        {
            try
            {
                return _context.Agendamentos.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar agendamentos: {ex.Message}");
            }
        }

    }
}
