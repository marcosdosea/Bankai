using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class AvaliarService : IAvaliarService
    {
        private const string StatusConcluido = "Concluido";
        private const int NotaMinima = 1;
        private const int NotaMaxima = 5;
        private const int TamanhoMaximoDepoimento = 255;

        // Pesos da reputaçao multifatorial conforme RN05
        private const double PesoNotaClientes = 5.0;
        private const double PesoSemGarantia = 3.0;
        private const double PesoPontualidade = 2.0;
        private const double SomaPesos = PesoNotaClientes + PesoSemGarantia + PesoPontualidade;

        private readonly BankaiContext context;

        public AvaliarService(BankaiContext context)
        {
            this.context = context;
        } 

        /// <summary>
        /// Insere uma nova avaliação no banco de dados após validar as regras de negócio.
        /// </summary>
        /// <param name="avaliacao">A avaliação a ser cadastrada.</param>
        /// <returns>O identificador da avaliação criada.</returns>
        public int Create(Avaliacao avaliacao)
        {
            Validar(avaliacao, isEdicao: false);
            context.Avaliacaos.Add(avaliacao);
            context.SaveChanges();
            return avaliacao.Id;
        }

        /// <summary>
        /// Atualiza uma avaliação existente no banco de dados.
        /// </summary>
        /// <param name="avaliacao">A avaliação com os dados atualizados.</param>
        public void Edit(Avaliacao avaliacao)
        {
            if (avaliacao == null)
            {
                throw new ArgumentNullException(nameof(avaliacao), "A avaliação não pode ser nula.");
            }

            var existe = context.Avaliacaos.Any(a => a.Id == avaliacao.Id);
            if (!existe)
            {
                throw new KeyNotFoundException("Avaliação não encontrada.");
            }

            Validar(avaliacao, isEdicao: true);

            var avaliacaoRastreada = context.Avaliacaos.Local
                .FirstOrDefault(a => a.Id == avaliacao.Id);

            if (avaliacaoRastreada == null)
            {
                context.Avaliacaos.Update(avaliacao);
            }
            else
            {
                context.Entry(avaliacaoRastreada).CurrentValues.SetValues(avaliacao);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Exclui uma avaliação pelo identificador. Se não existir, nenhuma ação é executada.
        /// </summary>
        /// <param name="id">O identificador da avaliação.</param>
        public void Delete(int id)
        {
            var avaliacao = context.Avaliacaos.FirstOrDefault(a => a.Id == id);
            if (avaliacao != null)
            {
                context.Avaliacaos.Remove(avaliacao);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtém uma avaliação pelo identificador.
        /// </summary>
        /// <param name="id">O identificador da avaliação.</param>
        /// <returns>A avaliação encontrada ou nulo.</returns>
        public Avaliacao? Get(int id)
        {
            return context.Avaliacaos
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Recupera todas as avaliações cadastradas.
        /// </summary>
        /// <returns>Coleção de avaliações sem rastreamento.</returns>
        public IEnumerable<Avaliacao> GetAll()
        {
            return context.Avaliacaos.AsNoTracking();
        }

        /// <summary>
        /// Registra a avaliação de um serviço concluído conforme CSU07 e Tela_016.
        /// </summary>
        /// <param name="idAgendamento">Identificador do agendamento avaliado.</param>
        /// <param name="pontualidade">Nota de pontualidade e assiduidade (1 a 5).</param>
        /// <param name="limpezaLocal">Nota de limpeza e organização do local (1 a 5).</param>
        /// <param name="qualidadeReparo">Nota de qualidade técnica do reparo (1 a 5).</param>
        /// <param name="depoimento">Depoimento adicional opcional (máximo 255 caracteres).</param>
        /// <returns>A avaliação registrada.</returns>
        public Avaliacao Avaliar(
            int idAgendamento,
            int pontualidade,
            int limpezaLocal,
            int qualidadeReparo,
            string? depoimento = null)
        {
            var avaliacao = new Avaliacao
            {
                IdAgendamento = idAgendamento,
                Pontualidade = pontualidade,
                LimpezaLocal = limpezaLocal,
                QualidadeReparo = qualidadeReparo,
                Depoimento = string.IsNullOrWhiteSpace(depoimento) ? null : depoimento.Trim()
            };

            Create(avaliacao);
            return avaliacao;
        }

        /// <summary>
        /// Obtém a avaliação vinculada a um agendamento específico.
        /// </summary>
        /// <param name="idAgendamento">Identificador do agendamento.</param>
        /// <returns>A avaliação encontrada ou nulo.</returns>
        public Avaliacao? GetByAgendamento(int idAgendamento)
        {
            return context.Avaliacaos
                .AsNoTracking()
                .FirstOrDefault(a => a.IdAgendamento == idAgendamento);
        }

        /// <summary>
        /// Recupera as avaliações recebidas por um profissional, ordenadas das mais recentes para as mais antigas.
        /// </summary>
        /// <param name="idProfissional">Identificador do profissional.</param>
        /// <returns>Coleção de avaliações do profissional.</returns>
        public IEnumerable<Avaliacao> GetByProfissional(int idProfissional)
        {
            return context.Avaliacaos
                .Include(a => a.IdAgendamentoNavigation)
                .Where(a => a.IdAgendamentoNavigation.IdProfissional == idProfissional)
                .OrderByDescending(a => a.Id)
                .AsNoTracking();
        }

        /// <summary>
        /// Recupera as avaliações realizadas por um cliente, ordenadas das mais recentes para as mais antigas.
        /// </summary>
        /// <param name="idCliente">Identificador do cliente.</param>
        /// <returns>Coleção de avaliações realizadas pelo cliente.</returns>
        public IEnumerable<Avaliacao> GetByCliente(int idCliente)
        {
            return context.Avaliacaos
                .Include(a => a.IdAgendamentoNavigation)
                .Where(a => a.IdAgendamentoNavigation.IdCliente == idCliente)
                .OrderByDescending(a => a.Id)
                .AsNoTracking();
        }

        /// <summary>
        /// Recupera os agendamentos concluídos de um cliente que ainda estão pendentes de avaliação.
        /// </summary>
        /// <param name="idCliente">Identificador do cliente.</param>
        /// <returns>Coleção de agendamentos concluídos ainda não avaliados.</returns>
        public IEnumerable<Agendamento> GetPendentesPorCliente(int idCliente)
        {
            return context.Agendamentos
                .Where(a => a.IdCliente == idCliente
                    && a.StatusAgendamento == StatusConcluido
                    && !context.Avaliacaos.Any(av => av.IdAgendamento == a.Id))
                .OrderByDescending(a => a.Data)
                .ThenByDescending(a => a.Id)
                .AsNoTracking();
        }

        /// <summary>
        /// Calcula a média das notas atribuídas pelos clientes ao profissional (escala de 0 a 5).
        /// </summary>
        /// <param name="idProfissional">Identificador do profissional.</param>
        /// <returns>Média arredondada para duas casas decimais ou 0 caso não haja avaliações.</returns>
        public double CalcularMediaNota(int idProfissional)
        {
            var medias = context.Avaliacaos
                .AsNoTracking()
                .Where(a => a.IdAgendamentoNavigation.IdProfissional == idProfissional)
                .Select(a => (a.Pontualidade + a.LimpezaLocal + a.QualidadeReparo) / 3.0)
                .ToList();

            if (medias.Count == 0)
            {
                return 0;
            }

            return Math.Round(medias.Average(), 2);
        }

        /// <summary>
        /// Calcula o Bankai Rating (reputação multifatorial) do profissional conforme RN05:
        /// nota média dos clientes (peso 5), índice de serviços concluídos sem garantia (peso 3)
        /// e pontualidade no atendimento (peso 2), em escala de 0 a 5.
        /// </summary>
        /// <param name="idProfissional">Identificador do profissional.</param>
        /// <returns>Nota Bankai Rating arredondada para duas casas decimais ou 0 caso não haja avaliações.</returns>
        public double CalcularBankaiRating(int idProfissional)
        {
            var avaliacoes = context.Avaliacaos
                .AsNoTracking()
                .Where(a => a.IdAgendamentoNavigation.IdProfissional == idProfissional)
                .Select(a => new
                {
                    MediaAvaliacao = (a.Pontualidade + a.LimpezaLocal + a.QualidadeReparo) / 3.0,
                    a.Pontualidade
                })
                .ToList();

            if (avaliacoes.Count == 0)
            {
                return 0;
            }

            var mediaNotasClientes = avaliacoes.Average(a => a.MediaAvaliacao);
            var mediaPontualidade = avaliacoes.Average(a => a.Pontualidade);

            var idsConcluidos = context.Agendamentos
                .AsNoTracking()
                .Where(a => a.IdProfissional == idProfissional && a.StatusAgendamento == StatusConcluido)
                .Select(a => a.Id)
                .ToList();

            double indiceSemGarantia = 0;
            if (idsConcluidos.Count > 0)
            {
                var totalComGarantia = context.Chamadogarantia
                    .AsNoTracking()
                    .Where(g => idsConcluidos.Contains(g.IdAgendamento))
                    .Select(g => g.IdAgendamento)
                    .Distinct()
                    .Count();

                var semGarantia = idsConcluidos.Count - totalComGarantia;
                indiceSemGarantia = ((double)semGarantia / idsConcluidos.Count) * NotaMaxima;
            }

            var rating = ((mediaNotasClientes * PesoNotaClientes)
                        + (indiceSemGarantia * PesoSemGarantia)
                        + (mediaPontualidade * PesoPontualidade)) / SomaPesos;

            return Math.Round(rating, 2);
        }

        private void Validar(Avaliacao avaliacao, bool isEdicao)
        {
            if (avaliacao == null)
            {
                throw new ArgumentNullException(nameof(avaliacao), "A avaliação não pode ser nula.");
            }

            if (avaliacao.IdAgendamento <= 0)
            {
                throw new ArgumentException("O agendamento informado é inválido.");
            }

            if (!NotaValida(avaliacao.Pontualidade) ||
                !NotaValida(avaliacao.LimpezaLocal) ||
                !NotaValida(avaliacao.QualidadeReparo))
            {
                throw new ArgumentException("Por favor, selecione uma nota para avaliar o serviço.");
            }

            if (avaliacao.Depoimento != null && avaliacao.Depoimento.Length > TamanhoMaximoDepoimento)
            {
                throw new ArgumentException("O depoimento deve ter no máximo 255 caracteres.");
            }

            var agendamento = context.Agendamentos
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == avaliacao.IdAgendamento);

            if (agendamento == null)
            {
                throw new KeyNotFoundException("Agendamento não encontrado.");
            }

            if (agendamento.StatusAgendamento != StatusConcluido)
            {
                throw new InvalidOperationException("Apenas serviços concluídos podem ser avaliados.");
            }

            var jaAvaliado = context.Avaliacaos.Any(a =>
                a.IdAgendamento == avaliacao.IdAgendamento &&
                (!isEdicao || a.Id != avaliacao.Id));

            if (jaAvaliado)
            {
                throw new InvalidOperationException("Este agendamento já foi avaliado.");
            }
        }

        private static bool NotaValida(int nota) => nota >= NotaMinima && nota <= NotaMaxima;
    }
}

