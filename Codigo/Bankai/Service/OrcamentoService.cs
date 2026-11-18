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
        public decimal CalcularTaxaPlataforma(decimal valorProposto)
        {
            if (valorProposto < 0)
                return 0;

            return Math.Round(valorProposto * TAXA_INTERMEDIACAO, 2);
        }

        /// <summary>
        /// Calcula o repasse líquido devido ao profissional (85%) conforme RN01.
        /// </summary>
        public decimal CalcularValorLiquido(decimal valorProposto)
        {
            if (valorProposto < 0)
                return 0;

            return Math.Round(valorProposto * (1.0m - TAXA_INTERMEDIACAO), 2);
        }


        public uint Create(Orcamento orcamento, uint idAgendamento)
        {
            if (orcamento.ValorProposto <= 0)
                throw new ArgumentException("O valor do orçamento deve ser maior que zero.");

            if (orcamento.Validade <= 0)
                throw new ArgumentException("A validade deve ser maior que zero.");

            var agendamento = context.Agendamentos.Find((int)idAgendamento);
            if (agendamento == null)
                throw new ArgumentException("Agendamento não encontrado.");

            if (agendamento.TipoCobranca != "O")
                throw new InvalidOperationException("O agendamento não é do tipo orçamento.");

            context.Orcamentos.Add(orcamento);
            context.SaveChanges(); // Gera o Id do orçamento

            // Vincula o orçamento recém-criado ao agendamento
            agendamento.IdOrcamento = orcamento.Id;
            context.SaveChanges();

            return (uint)orcamento.Id;
        }


        /// <summary>
        /// Registra uma proposta de orçamento, desde que o agendamento esteja configurado para cobrança por orçamento ('O').
        /// </summary>


        public void Edit(Orcamento orcamento)
        {
            if (orcamento.ValorProposto <= 0)
                throw new ArgumentException("O valor do orçamento deve ser maior que zero.");

            if (orcamento.Validade <= 0)
                throw new ArgumentException("A validade deve ser maior que zero.");

            var orcamentoExistente = context.Orcamentos.Find(orcamento.Id);
            if (orcamentoExistente == null)
                throw new ArgumentException("Orçamento não encontrado.");

            var agendamento = context.Agendamentos.FirstOrDefault(a => a.IdOrcamento == orcamento.Id);
            if (agendamento == null || agendamento.TipoCobranca != "O")
                throw new InvalidOperationException("O orçamento não está vinculado a um agendamento válido do tipo orçamento ('O').");

            // Atualiza apenas os campos permitidos na entidade já rastreada pelo EF
            orcamentoExistente.ValorProposto = orcamento.ValorProposto;
            orcamentoExistente.Validade = orcamento.Validade;
            orcamentoExistente.Descricao = orcamento.Descricao;
            orcamentoExistente.MateriaisAdicionais = orcamento.MateriaisAdicionais;

            context.SaveChanges();
        }

        /// <summary>
        /// Obtem um orçamento específico pelo seu ID. Retorna null se não encontrado.           
        /// </summary>
        public Orcamento? Get(uint Id)
        {
            return context.Orcamentos.FirstOrDefault(o => o.Id == Id);
        }

        public void Delete(uint Id)
        {
            var orcamento = context.Orcamentos.Find((int)Id);
            if (orcamento != null)
            {
                context.Remove(orcamento);
                context.SaveChanges();
            }
        }


        /// <summary>
        /// Retorna todos os orçamentos cadastrados no banco de dados.
        /// </summary>
        public IEnumerable<Orcamento> GetAll()
        {
            return context.Orcamentos.AsNoTracking();
        }



    }
}

