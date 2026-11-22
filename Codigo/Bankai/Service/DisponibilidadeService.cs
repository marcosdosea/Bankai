using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class DisponibilidadeService : IDisponibilidadeService
    {
        private static readonly HashSet<string> StatusValidos = new(StringComparer.Ordinal) { "D", "ND" };
        private static readonly HashSet<string> DiasValidos = new(StringComparer.Ordinal) { "D", "SF", "TF", "QF", "QIF", "SXF", "S" };
        private const int TamanhoMaximoDescricao = 255;

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
            Validar(disponibilidade, isEdicao: false);
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
            Validar(disponibilidade, isEdicao: true);

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

        private void Validar(Disponibilidade disponibilidade, bool isEdicao)
        {
            if (disponibilidade == null)
            {
                throw new ArgumentNullException(nameof(disponibilidade), "A disponibilidade não pode ser nula.");
            }

            if (disponibilidade.IdPessoa <= 0)
            {
                throw new ArgumentException("O profissional informado é inválido.");
            }

            if (string.IsNullOrWhiteSpace(disponibilidade.StatusDisponibilidade) || !StatusValidos.Contains(disponibilidade.StatusDisponibilidade))
            {
                throw new ArgumentException("O status da disponibilidade deve ser 'D' ou 'ND'.");
            }

            if (string.IsNullOrWhiteSpace(disponibilidade.DiaSemana) || !DiasValidos.Contains(disponibilidade.DiaSemana))
            {
                throw new ArgumentException("Selecione um dia da semana válido.");
            }

            if (disponibilidade.HorarioFim <= disponibilidade.HorarioInicio)
            {
                throw new ArgumentException("O horário de término deve ser posterior ao horário de início.");
            }

            if (disponibilidade.DataIndisponibilizade.HasValue)
            {
                var dataBloqueio = disponibilidade.DataIndisponibilizade.Value.Date;
                disponibilidade.DataIndisponibilizade = dataBloqueio;

                if (string.IsNullOrWhiteSpace(disponibilidade.DescricaoIndisponibilidade))
                {
                    throw new ArgumentException("Informe o motivo da indisponibilidade.");
                }

                disponibilidade.DescricaoIndisponibilidade = disponibilidade.DescricaoIndisponibilidade.Trim();

                if (disponibilidade.DescricaoIndisponibilidade.Length > TamanhoMaximoDescricao)
                {
                    throw new ArgumentException("O motivo da indisponibilidade deve ter no máximo 255 caracteres.");
                }

                var diaEsperado = ObterDiaSemanaSigla(dataBloqueio.DayOfWeek);
                if (disponibilidade.DiaSemana != diaEsperado)
                {
                    throw new ArgumentException("O dia da semana deve corresponder à data específica informada.");
                }

                var dataAnterior = isEdicao
                    ? this.context.Disponibilidades
                        .AsNoTracking()
                        .Where(d => d.Id == disponibilidade.Id)
                        .Select(d => d.DataIndisponibilizade)
                        .FirstOrDefault()
                    : null;

                if ((!isEdicao || dataAnterior?.Date != dataBloqueio) && dataBloqueio < DateTime.Today)
                {
                    throw new InvalidOperationException("Não é permitido bloquear uma data passada.");
                }

                var bloqueioDuplicado = this.context.Disponibilidades
                    .AsNoTracking()
                    .Any(d => d.IdPessoa == disponibilidade.IdPessoa
                        && d.DataIndisponibilizade.HasValue
                        && d.DataIndisponibilizade.Value.Date == dataBloqueio
                        && (!isEdicao || d.Id != disponibilidade.Id));

                if (bloqueioDuplicado)
                {
                    throw new InvalidOperationException("Já existe um bloqueio cadastrado para esta data.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(disponibilidade.DescricaoIndisponibilidade))
            {
                throw new ArgumentException("Informe a data específica quando um motivo de indisponibilidade for preenchido.");
            }
            else
            {
                disponibilidade.DescricaoIndisponibilidade = null;
            }
        }

        private static string ObterDiaSemanaSigla(DayOfWeek dia) => dia switch
        {
            DayOfWeek.Sunday => "D",
            DayOfWeek.Monday => "SF",
            DayOfWeek.Tuesday => "TF",
            DayOfWeek.Wednesday => "QF",
            DayOfWeek.Thursday => "QIF",
            DayOfWeek.Friday => "SXF",
            DayOfWeek.Saturday => "S",
            _ => "D"
        };
    }
}