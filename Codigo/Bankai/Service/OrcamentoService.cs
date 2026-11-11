using Core;
using Core.DTO;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class OrcamentoService : IOrcamentoService
    {
        private const decimal TAXA_INTERMEDIACAO = 0.15m; // 15% retido pela plataforma conforme RN01
        private readonly BankaiContext context;

        public OrcamentoService(BankaiContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Calcula a taxa de intermediação retida pela Bankai (15%) conforme RN01.
        /// </summary>
        public decimal CalcularTaxaPlataforma(decimal valorBruto)
        {
            if (valorBruto < 0)
                return 0;

            return Math.Round(valorBruto * TAXA_INTERMEDIACAO, 2);
        }

        /// <summary>
        /// Calcula o repasse líquido devido ao profissional (85%) conforme RN01.
        /// </summary>
        public decimal CalcularValorLiquido(decimal valorBruto)
        {
            if (valorBruto < 0)
                return 0;

            return Math.Round(valorBruto * (1.0m - TAXA_INTERMEDIACAO), 2);
        }

        /// <summary>
        /// Registra a proposta financeira de orçamento para a ordem de serviço.
        /// </summary>
        public bool EnviarOrcamento(int idAgendamento, int idServico, decimal valorProposto)
        {
            if (valorProposto <= 0)
            {
                throw new ArgumentException("O valor do orçamento deve ser estritamente maior que zero.", nameof(valorProposto));
            }

            var agendamento = context.Agendamentos.Find(idAgendamento);
            if (agendamento == null)
            {
                return false;
            }

            if (agendamento.TipoCobranca != "O")
            {
                throw new InvalidOperationException("Apenas agendamentos definidos com cobrança por Orçamento ('O') permitem inserção de proposta orçamentária.");
            }

            var agendamentoServico = context.Agendamentoservicos.Find(idAgendamento, idServico);
            if (agendamentoServico == null)
            {
                return false;
            }

            agendamentoServico.ValorProposto = valorProposto;
            agendamentoServico.StatusServico = "E"; // Em Execução / Orçado
            context.SaveChanges();

            return true;
        }

        /// <summary>
        /// Busca o orçamento de um serviço específico dentro de um agendamento.
        /// </summary>
        public OrcamentoDTO? Get(int idAgendamento, int idServico)
        {
            var item = context.Agendamentoservicos
                .Include(asv => asv.IdAgendamentoNavigation)
                    .ThenInclude(ag => ag.IdClienteNavigation)
                .Include(asv => asv.IdAgendamentoNavigation)
                    .ThenInclude(ag => ag.IdProfissionalNavigation)
                .Include(asv => asv.IdServicoNavigation)
                .AsNoTracking()
                .FirstOrDefault(asv => asv.IdAgendamento == idAgendamento && asv.IdServico == idServico);

            if (item == null)
            {
                return null;
            }

            return new OrcamentoDTO
            {
                IdAgendamento = item.IdAgendamento,
                IdServico = item.IdServico,
                NomeServico = item.IdServicoNavigation?.Nome ?? string.Empty,
                DescricaoServico = item.IdAgendamentoNavigation?.DescricaoServico ?? string.Empty,
                IdCliente = item.IdAgendamentoNavigation?.IdCliente ?? 0,
                NomeCliente = item.IdAgendamentoNavigation?.IdClienteNavigation?.Nome ?? string.Empty,
                IdProfissional = item.IdAgendamentoNavigation?.IdProfissional ?? 0,
                NomeProfissional = item.IdAgendamentoNavigation?.IdProfissionalNavigation?.Nome ?? string.Empty,
                TipoCobranca = item.IdAgendamentoNavigation?.TipoCobranca ?? string.Empty,
                StatusAgendamento = item.IdAgendamentoNavigation?.StatusAgendamento ?? string.Empty,
                StatusServico = item.StatusServico,
                ValorProposto = item.ValorProposto,
                TaxaPlataforma = CalcularTaxaPlataforma(item.ValorProposto),
                ValorLiquido = CalcularValorLiquido(item.ValorProposto)
            };
        }

        /// <summary>
        /// Retorna todos os agendamentos registrados como Orçamento ('O').
        /// </summary>
        public IEnumerable<OrcamentoDTO> GetAll()
        {
            return context.Agendamentoservicos
                .Include(asv => asv.IdAgendamentoNavigation)
                    .ThenInclude(ag => ag.IdClienteNavigation)
                .Include(asv => asv.IdAgendamentoNavigation)
                    .ThenInclude(ag => ag.IdProfissionalNavigation)
                .Include(asv => asv.IdServicoNavigation)
                .Where(asv => asv.IdAgendamentoNavigation.TipoCobranca == "O")
                .AsNoTracking()
                .Select(item => new OrcamentoDTO
                {
                    IdAgendamento = item.IdAgendamento,
                    IdServico = item.IdServico,
                    NomeServico = item.IdServicoNavigation.Nome,
                    DescricaoServico = item.IdAgendamentoNavigation.DescricaoServico,
                    IdCliente = item.IdAgendamentoNavigation.IdCliente,
                    NomeCliente = item.IdAgendamentoNavigation.IdClienteNavigation.Nome,
                    IdProfissional = item.IdAgendamentoNavigation.IdProfissional,
                    NomeProfissional = item.IdAgendamentoNavigation.IdProfissionalNavigation.Nome,
                    TipoCobranca = item.IdAgendamentoNavigation.TipoCobranca,
                    StatusAgendamento = item.IdAgendamentoNavigation.StatusAgendamento,
                    StatusServico = item.StatusServico,
                    ValorProposto = item.ValorProposto,
                    TaxaPlataforma = Math.Round(item.ValorProposto * TAXA_INTERMEDIACAO, 2),
                    ValorLiquido = Math.Round(item.ValorProposto * (1.0m - TAXA_INTERMEDIACAO), 2)
                })
                .ToList();
        }

        /// <summary>
        /// Retorna todos os orçamentos vinculados a um determinado profissional.
        /// </summary>
        public IEnumerable<OrcamentoDTO> GetByProfissional(int idProfissional)
        {
            return context.Agendamentoservicos
                .Include(asv => asv.IdAgendamentoNavigation)
                    .ThenInclude(ag => ag.IdClienteNavigation)
                .Include(asv => asv.IdAgendamentoNavigation)
                    .ThenInclude(ag => ag.IdProfissionalNavigation)
                .Include(asv => asv.IdServicoNavigation)
                .Where(asv => asv.IdAgendamentoNavigation.TipoCobranca == "O" && asv.IdAgendamentoNavigation.IdProfissional == idProfissional)
                .AsNoTracking()
                .Select(item => new OrcamentoDTO
                {
                    IdAgendamento = item.IdAgendamento,
                    IdServico = item.IdServico,
                    NomeServico = item.IdServicoNavigation.Nome,
                    DescricaoServico = item.IdAgendamentoNavigation.DescricaoServico,
                    IdCliente = item.IdAgendamentoNavigation.IdCliente,
                    NomeCliente = item.IdAgendamentoNavigation.IdClienteNavigation.Nome,
                    IdProfissional = item.IdAgendamentoNavigation.IdProfissional,
                    NomeProfissional = item.IdAgendamentoNavigation.IdProfissionalNavigation.Nome,
                    TipoCobranca = item.IdAgendamentoNavigation.TipoCobranca,
                    StatusAgendamento = item.IdAgendamentoNavigation.StatusAgendamento,
                    StatusServico = item.StatusServico,
                    ValorProposto = item.ValorProposto,
                    TaxaPlataforma = Math.Round(item.ValorProposto * TAXA_INTERMEDIACAO, 2),
                    ValorLiquido = Math.Round(item.ValorProposto * (1.0m - TAXA_INTERMEDIACAO), 2)
                })
                .ToList();
        }
    }
}

