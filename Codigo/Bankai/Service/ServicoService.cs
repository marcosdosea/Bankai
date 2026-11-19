using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class ServicoService : IServicoService
    {
        private readonly BankaiContext context;

        public ServicoService(BankaiContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria um novo serviço no banco de dados e retorna o id gerado.
        /// </summary>
        /// <param name="servico">O serviço a ser cadastrado.</param>
        /// <returns>O id do serviço criado.</returns>
        public int Create(Servico servico)
        {
            context.Add(servico);
            context.SaveChanges();
            return servico.Id;
        }

        /// <summary>
        /// Atualiza as informações de um serviço existente no banco de dados.
        /// </summary>
        /// <param name="servico">O serviço a ser atualizado.</param>
        public void Edit(Servico servico)
        {
            context.Update(servico);
            context.SaveChanges();
        }

        /// <summary>
        /// Exclui um serviço do banco de dados com base no identificador.
        /// Se o serviço não for localizado, nenhuma ação será executada.
        /// </summary>
        /// <param name="id">O identificador do serviço a ser excluído.</param>
        public void Delete(int id)
        {
            var servico = context.Servicos.Find(id);
            if (servico != null)
            {
                context.Remove(servico);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtém um serviço a partir de seu identificador, carregando sua especialidade quando disponível.
        /// </summary>
        /// <param name="id">O id do serviço.</param>
        /// <returns>A entidade de serviço encontrada ou nula.</returns>
        public Servico? Get(int id)
        {
            var servico = context.Servicos.Find(id);
            if (servico != null)
            {
                context.Entry(servico).Reference(s => s.IdEspecialidadeNavigation).Load();
            }
            return servico;
        }

        /// <summary>
        /// Recupera todos os serviços cadastrados no banco de dados.
        /// </summary>
        /// <returns>Coleção de serviços sem rastreamento.</returns>
        public IEnumerable<Servico> GetAll()
        {
            return context.Servicos.AsNoTracking();
        }

        /// <summary>
        /// Recupera os serviços cujo nome começa com o termo informado.
        /// </summary>
        /// <param name="nome">Prefixo do nome do serviço.</param>
        /// <returns>Coleção de serviços correspondentes.</returns>
        public IEnumerable<Servico> GetByNome(string nome)
        {
            return context.Servicos
                .Where(s => s.Nome.StartsWith(nome))
                .AsNoTracking();
        }

        /// <summary>
        /// Lista as especialidades disponíveis no catálogo ordenadas alfabeticamente.
        /// </summary>
        /// <returns>Coleção de especialidades.</returns>
        public IEnumerable<Especialidade> ObterEspecialidades()
        {
            return context.Especialidades
                .AsNoTracking()
                .OrderBy(e => e.Nome)
                .ToList();
        }
    }
}
