using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class ServicoService : IServicoService
    {
        private const string StatusDisponivel = "D";
        private const string StatusIndisponivel = "ND";

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
        /// Obtém um serviço a partir de seu identificador.
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

        /// <summary>
        /// Busca serviços aplicando filtros opcionais de termo (nome, descrição ou especialidade),
        /// especialidade específica e disponibilidade na data desejada.
        /// </summary>
        public IEnumerable<ServicoBuscaDTO> BuscarServicos(string? termo, int? idEspecialidade, DateTime? dataDesejada)
        {
            var consulta = context.Servicos.AsNoTracking().AsQueryable();

            if (idEspecialidade.HasValue && idEspecialidade.Value > 0)
            {
                consulta = consulta.Where(s => s.IdEspecialidade == idEspecialidade.Value);
            }

            if (!string.IsNullOrWhiteSpace(termo))
            {
                string termoLower = termo.Trim().ToLower();
                consulta = consulta.Where(s =>
                    s.Nome.ToLower().Contains(termoLower) ||
                    s.Descricao.ToLower().Contains(termoLower) ||
                    (s.IdEspecialidadeNavigation != null && s.IdEspecialidadeNavigation.Nome.ToLower().Contains(termoLower)));
            }

            var servicos = consulta
                .Select(s => new
                {
                    s.Id,
                    s.Nome,
                    s.Descricao,
                    s.IdEspecialidade,
                    NomeEspecialidade = s.IdEspecialidadeNavigation != null ? s.IdEspecialidadeNavigation.Nome : string.Empty
                })
                .ToList();

            if (servicos.Count == 0)
            {
                return Enumerable.Empty<ServicoBuscaDTO>();
            }

            var servicoIds = servicos.Select(s => s.Id).ToList();

            var ofertas = context.Profissionalservicos
                .AsNoTracking()
                .Where(ps => servicoIds.Contains(ps.IdServico) && ps.IdPessoaNavigation.EhProfissional)
                .Select(ps => new { ps.IdServico, ps.IdPessoa, ps.Preco })
                .ToList();

            HashSet<int>? profissionaisDisponiveis = null;
            if (dataDesejada.HasValue && ofertas.Count > 0)
            {
                var idsProfissionais = ofertas.Select(o => o.IdPessoa).Distinct().ToList();
                var disponibilidades = context.Disponibilidades
                    .AsNoTracking()
                    .Where(d => idsProfissionais.Contains(d.IdPessoa))
                    .ToList();

                profissionaisDisponiveis = idsProfissionais
                    .Where(id => ProfissionalEstaDisponivelNaData(
                        disponibilidades.Where(d => d.IdPessoa == id),
                        dataDesejada.Value))
                    .ToHashSet();
            }

            return servicos
                .Select(s =>
                {
                    var ofertasServico = ofertas.Where(o => o.IdServico == s.Id);
                    var ofertasValidas = profissionaisDisponiveis == null
                        ? ofertasServico
                        : ofertasServico.Where(o => profissionaisDisponiveis.Contains(o.IdPessoa));

                    var listaOfertas = ofertasValidas.ToList();

                    return new ServicoBuscaDTO
                    {
                        Id = s.Id,
                        Nome = s.Nome,
                        Descricao = s.Descricao,
                        IdEspecialidade = s.IdEspecialidade,
                        NomeEspecialidade = s.NomeEspecialidade,
                        QuantidadeProfissionais = listaOfertas.Select(o => o.IdPessoa).Distinct().Count(),
                        MenorPreco = listaOfertas.Count > 0 ? listaOfertas.Min(o => o.Preco) : null
                    };
                })
                .OrderByDescending(s => s.QuantidadeProfissionais > 0)
                .ThenBy(s => s.Nome)
                .ToList();
        }

        /// <summary>
        /// Busca profissionais que executam um determinado serviço, aplicando filtros de verificação,
        /// disponibilidade para a data informada, teto de preço e ordenação.
        /// </summary>
        public IEnumerable<ProfissionalDisponivelDTO> BuscarProfissionaisPorServico(
            int idServico,
            DateTime? dataDesejada,
            string? ordenarPor = "melhor_avaliado",
            bool apenasVerificados = false,
            bool apenasDisponiveisNaData = false,
            decimal? precoMaximo = null)
        {
            var consulta = context.Profissionalservicos
                .AsNoTracking()
                .Where(ps => ps.IdServico == idServico && ps.IdPessoaNavigation.EhProfissional);

            if (apenasVerificados)
            {
                consulta = consulta.Where(ps => ps.IdPessoaNavigation.RegistroTecnico != null);
            }

            if (precoMaximo.HasValue && precoMaximo.Value > 0)
            {
                consulta = consulta.Where(ps => ps.Preco <= precoMaximo.Value);
            }

            var profissionais = consulta
                .Select(ps => new
                {
                    ps.IdPessoa,
                    ps.IdServico,
                    NomeProfissional = ps.IdPessoaNavigation.Nome,
                    Bairro = ps.IdPessoaNavigation.Bairro,
                    Cidade = ps.IdPessoaNavigation.Cidade,
                    Estado = ps.IdPessoaNavigation.Estado,
                    Verificado = ps.IdPessoaNavigation.RegistroTecnico != null,
                    ps.Preco
                })
                .ToList();

            if (profissionais.Count == 0)
            {
                return Enumerable.Empty<ProfissionalDisponivelDTO>();
            }

            var idsProfissionais = profissionais.Select(p => p.IdPessoa).Distinct().ToList();

            var avaliacoesPorProfissional = context.Avaliacaos
                .AsNoTracking()
                .Where(a => idsProfissionais.Contains(a.IdAgendamentoNavigation.IdProfissional))
                .Select(a => new
                {
                    a.IdAgendamentoNavigation.IdProfissional,
                    Media = (a.Pontualidade + a.LimpezaLocal + a.QualidadeReparo) / 3.0m
                })
                .ToList()
                .GroupBy(a => a.IdProfissional)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        NotaMedia = Math.Round(g.Average(x => x.Media), 2),
                        Total = g.Count()
                    });

            DateTime dataReferencia = (dataDesejada ?? DateTime.Today).Date;
            var disponibilidades = context.Disponibilidades
                .AsNoTracking()
                .Where(d => idsProfissionais.Contains(d.IdPessoa))
                .ToList();

            var resultados = profissionais.Select(p =>
            {
                avaliacoesPorProfissional.TryGetValue(p.IdPessoa, out var resumoAvaliacao);
                bool disponivel = ProfissionalEstaDisponivelNaData(
                    disponibilidades.Where(d => d.IdPessoa == p.IdPessoa),
                    dataReferencia);

                return new ProfissionalDisponivelDTO
                {
                    IdProfissional = p.IdPessoa,
                    IdServico = p.IdServico,
                    NomeProfissional = p.NomeProfissional,
                    Bairro = p.Bairro,
                    Cidade = p.Cidade,
                    Estado = p.Estado,
                    Verificado = p.Verificado,
                    Preco = p.Preco,
                    NotaMedia = resumoAvaliacao?.NotaMedia ?? 0m,
                    TotalAvaliacoes = resumoAvaliacao?.Total ?? 0,
                    DisponivelNaData = disponivel
                };
            });

            if (apenasDisponiveisNaData)
            {
                resultados = resultados.Where(r => r.DisponivelNaData);
            }

            return ordenarPor switch
            {
                "menor_preco" => resultados
                    .OrderBy(r => r.Preco)
                    .ThenByDescending(r => r.NotaMedia)
                    .ToList(),
                "nome" => resultados
                    .OrderBy(r => r.NomeProfissional)
                    .ToList(),
                _ => resultados
                    .OrderByDescending(r => r.NotaMedia)
                    .ThenByDescending(r => r.TotalAvaliacoes)
                    .ThenBy(r => r.Preco)
                    .ToList()
            };
        }

        private static bool ProfissionalEstaDisponivelNaData(IEnumerable<Disponibilidade> disponibilidades, DateTime data)
        {
            string diaSemanaSigla = ConverterDiaSemanaParaSigla(data.DayOfWeek);
            var lista = disponibilidades.ToList();

            bool bloqueadoNaData = lista.Any(d =>
                d.StatusDisponibilidade == StatusIndisponivel &&
                d.DataIndisponibilizade.HasValue &&
                d.DataIndisponibilizade.Value.Date == data.Date);

            if (bloqueadoNaData)
            {
                return false;
            }

            return lista.Any(d =>
                d.StatusDisponibilidade == StatusDisponivel &&
                d.DiaSemana == diaSemanaSigla);
        }

        private static string ConverterDiaSemanaParaSigla(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "D",
                DayOfWeek.Monday => "SF",
                DayOfWeek.Tuesday => "TF",
                DayOfWeek.Wednesday => "QF",
                DayOfWeek.Thursday => "QIF",
                DayOfWeek.Friday => "SXF",
                DayOfWeek.Saturday => "S",
                _ => string.Empty
            };
        }
    }
}
