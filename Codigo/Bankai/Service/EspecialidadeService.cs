using Core;
using Core.Service;
using Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class EspecialidadeService : IEspecialidadeService
    {
        private readonly BankaiContext context;

        public EspecialidadeService(BankaiContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria uma nova especialidade no banco de dados e retorna o id da especialidade criada.
        /// </summary>
        /// <param name="especialidade">A especialidade a ser criada.</param>
        /// <returns>Id da especialidade criada</returns>
        public uint Create(Especialidade especialidade)
        {
            this.context.Add(especialidade);
            this.context.SaveChanges();
            return (uint) especialidade.Id;
        }

        /// <summary>
        /// Atualiza as informações de uma especialidade existente no banco de dados.
        /// </summary>
        /// <param name="especialidade">A especialidade com as informações atualizadas.</param>
        public void Edit(Especialidade especialidade)
        {
            this.context.Update(especialidade);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Exclui uma especialidade do banco de dados com base no id fornecido. 
        /// </summary>
        /// <param name="id">O id da especialidade a ser excluída.</param>
        public void Delete(uint id)
        {
            var especialidade = this.context.Especialidades.FirstOrDefault(e => e.Id == id);
            if (especialidade != null)
            {
                this.context.Remove(especialidade);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtém uma especialidade do banco de dados com base no id fornecido. 
        /// </summary>
        /// <param name="id">O id da especialidade.</param>
        /// <returns></returns>
        public Especialidade? Get(uint id)
        {
            return this.context.Especialidades.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Recupera todas as especialidades cadastradas no banco de dados.
        /// </summary>
        /// <returns>Uma coleção contendo todas as especialidades</returns>
        public IEnumerable<Especialidade> GetAll()
        {
            return this.context.Especialidades.AsNoTracking();
        }

        /// <summary>
        /// Recupera as especialidades cujo nome inicia com o texto fornecido.
        /// </summary>
        /// <param name="nome">O texto ou prefixo do nome para busca.</param>
        /// <returns>Uma coleção de especialidades filtradas pelo nome.</returns>
        public IEnumerable<Especialidade> GetByNome(string nome)
        {
            var consulta = from especialidade in this.context.Especialidades
                           where especialidade.Nome.StartsWith(nome)
                           orderby especialidade.Nome
                           select especialidade;
            return consulta.AsNoTracking();
        }
    }
}
