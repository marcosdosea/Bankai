using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class OrcarServicoService : IOrcarServicoService
    {
        private readonly BankaiContext context;

        public OrcarServicoService(BankaiContext context)
        {
            this.context = context;
        }

        public uint Create(Agendamentoservico orcamento)
        {
            orcamento.CalcularOrcamento();

            context.Agendamentoservicos.Add(orcamento);
            context.SaveChanges();

            return (uint)orcamento.IdAgendamento;
        }

        public void Edit(Agendamentoservico orcamento)
        {
            orcamento.CalcularOrcamento();

            context.Update(orcamento);
            context.SaveChanges();
        }

        public void Delete(uint IdAgendamento)
        {
            var orcamento = context.Agendamentoservicos
                .FirstOrDefault(o => o.IdAgendamento == (int)IdAgendamento);

            if (orcamento != null)
            {
                context.Agendamentoservicos.Remove(orcamento);
                context.SaveChanges();
            }
        }

        public Agendamentoservico? Get(uint IdAgendamento)
        {
            return context.Agendamentoservicos
                .FirstOrDefault(o => o.IdAgendamento == (int)IdAgendamento);
        }

        public IEnumerable<Agendamentoservico> GetAll()
        {
            return context.Agendamentoservicos.AsNoTracking();
        }
    }
}