using Core;
using Core.Service;
using Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class DisponibilidadeService : IDisponibilidadeService
    {
        private readonly BankaiContext context;

        public DisponibilidadeService(BankaiContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria uma nova disponibilidade no banco de dados e retorna o id da disponibilidade criada.
        /// </summary>
        /// <param name="disponibilidade">A disponibilidade a ser criada.</param>
        /// <returns>Id da disponibilidade criada</returns>
        public uint Create(Disponibilidade disponibilidade)
        {
            this.context.Add(disponibilidade);
            this.context.SaveChanges();
            return (uint)disponibilidade.Id;
        }

        /// <summary>
        /// Atualiza as informações de uma disponibilidade existente no banco de dados.
        /// </summary>
        /// <param name="disponibilidade">A disponibilidade com as informações atualizadas.</param>
        public void Edit(Disponibilidade disponibilidade)
        {
            var disponibilidadeRastreada = this.context.Disponibilidades.Local
                .FirstOrDefault(item => item.Id == disponibilidade.Id);

            if (disponibilidadeRastreada == null)
            {
                this.context.Update(disponibilidade);
            }
            else
            {
                this.context.Entry(disponibilidadeRastreada).CurrentValues.SetValues(disponibilidade);
            }

            this.context.SaveChanges();
        }

        /// <summary>
        /// Exclui uma disponibilidade do banco de dados com base no id fornecido. 
        /// </summary>
        /// <param name="id">O id da disponibilidade a ser excluída.</param>
        public void Delete(uint id)
        {
            var disponibilidade = this.context.Disponibilidades.FirstOrDefault(d => d.Id == id);
            if (disponibilidade != null)
            {
                this.context.Remove(disponibilidade);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtém uma disponibilidade do banco de dados com base no id fornecido. 
        /// </summary>
        /// <param name="id">O id da disponibilidade.</param>
        /// <returns>A disponibilidade encontrada ou nulo.</returns>
        public Disponibilidade? Get(uint id)
        {
            return this.context.Disponibilidades.FirstOrDefault(d => d.Id == id);
        }

        /// <summary>
        /// Recupera todas as disponibilidades cadastradas no banco de dados.
        /// </summary>
        /// <returns>Uma coleção contendo todas as disponibilidades</returns>
        public IEnumerable<Disponibilidade> GetAll()
        {
            return this.context.Disponibilidades.AsNoTracking();
        }

        public uint? GetFirstProfissionalId()
        {
            return this.context.Pessoas
                .AsNoTracking()
                .Where(pessoa => pessoa.EhProfissional)
                .Select(pessoa => (uint?)pessoa.Id)
                .FirstOrDefault();
        }

        public bool IsProfissional(uint idPessoa)
        {
            return idPessoa <= int.MaxValue
                && this.context.Pessoas.Any(pessoa => pessoa.Id == (int)idPessoa && pessoa.EhProfissional);
        }

        /// <summary>
        /// Recupera as disponibilidades de um profissional específico com base no idPessoa fornecido.
        /// </summary>
        /// <param name="idPessoa">O id da pessoa (profissional) para busca.</param>
        /// <returns>Uma coleção de disponibilidades vinculadas ao profissional.</returns>
        public IEnumerable<Disponibilidade> GetByIdPessoa(uint idPessoa)
        {
            var consulta = from disponibilidade in this.context.Disponibilidades
                           where disponibilidade.IdPessoa == idPessoa
                           orderby disponibilidade.DiaSemana
                           select disponibilidade;

            return consulta.AsNoTracking();
        }
    }
}