using Core.DTO;
using System;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IServicoService
    {
        int Create(Servico servico);
        void Edit(Servico servico);
        void Delete(int id);
        Servico? Get(int id);
        IEnumerable<Servico> GetAll();
        IEnumerable<Servico> GetByNome(string nome);

        /// <summary>
        /// Lista as especialidades disponíveis no catálogo ordenadas por nome.
        /// </summary>
        IEnumerable<Especialidade> ObterEspecialidades();

        /// <summary>
        /// Busca serviços do catálogo aplicando filtros opcionais de termo, especialidade e data desejada.
        /// </summary>
        IEnumerable<ServicoBuscaDTO> BuscarServicos(string? termo, int? idEspecialidade, DateTime? dataDesejada);

        /// <summary>
        /// Busca profissionais que oferecem um serviço específico, com filtros de verificação, disponibilidade na data, preço máximo e ordenação.
        /// </summary>
        IEnumerable<ProfissionalDisponivelDTO> BuscarProfissionaisPorServico(
            int idServico,
            DateTime? dataDesejada,
            string? ordenarPor = "melhor_avaliado",
            bool apenasVerificados = false,
            bool apenasDisponiveisNaData = false,
            decimal? precoMaximo = null);
    }
}
