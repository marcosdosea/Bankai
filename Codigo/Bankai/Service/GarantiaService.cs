using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class GarantiaService : IGarantiaService
    {
        private readonly BankaiContext context;

        public GarantiaService(BankaiContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Insere um novo chamado de garantia na base de dados.
        /// </summary>
        /// <param name="chamadoGarantia"></param>
        /// <returns>Id do chamado de garantia criado</returns>
        public uint Create(Chamadogarantium chamadoGarantia)
        {
            Validar(chamadoGarantia);
            context.Add(chamadoGarantia);
            context.SaveChanges();
            return (uint)chamadoGarantia.Id;
        }

        /// <summary>
        /// Atualiza um chamado de garantia existente.
        /// </summary>
        /// <param name="chamadoGarantia"></param>
        public void Edit(Chamadogarantium chamadoGarantia)
        {
            Validar(chamadoGarantia);
            context.Update(chamadoGarantia);
            context.SaveChanges();
        }

        /// <summary>
        /// Remove um chamado de garantia pelo ID.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(uint id)
        {
            var chamado = context.Chamadogarantia.FirstOrDefault(g => g.Id == id);
            if (chamado != null)
            {
                context.Remove(chamado);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtém um chamado de garantia pelo ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Chamado de garantia encontrado</returns>
        public Chamadogarantium? Get(uint id)
        {
            return context.Chamadogarantia
                .AsNoTracking()
                .FirstOrDefault(g => g.Id == id);
        }

        /// <summary>
        /// Recupera todos os chamados de garantia.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Chamadogarantium> GetAll()
        {
            return context.Chamadogarantia
                .AsNoTracking();
        }

        /// <summary>
        /// Busca chamados associados a um agendamento.
        /// </summary>
        /// <param name="idAgendamento"></param>
        /// <returns>Chamados associados ao agendamento</returns>
        public IEnumerable<Chamadogarantium> GetByIdAgendamento(uint idAgendamento)
        {
            return context.Chamadogarantia
                .Where(g => g.IdAgendamento == idAgendamento)
                .AsNoTracking();
        }

        /// <summary>
        /// Valida as regras de negócio para um chamado de garantia.
        /// </summary>
        /// <param name="chamadoGarantia"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void Validar(Chamadogarantium chamadoGarantia)
        {
            if (string.IsNullOrWhiteSpace(chamadoGarantia.Descricao))
            {
                throw new ArgumentException("A descrição do chamado de garantia não pode ser nula ou vazia.");
            }

            if (chamadoGarantia.IdAgendamento == 0)
            {
                throw new ArgumentException("O chamado de garantia deve estar associado a um agendamento válido.");
            }
        }
    }
}
