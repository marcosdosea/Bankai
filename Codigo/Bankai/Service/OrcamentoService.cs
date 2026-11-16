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


        public uint Create(Orcamento orcamento)
        {
            context.Orcamentos.Add(orcamento);
            context.SaveChanges();
            return (uint)orcamento.Id;
        }


        /// <summary>
        /// Registra uma proposta de orçamento, desde que o agendamento esteja configurado para cobrança por orçamento ('O').
        /// </summary>
        public bool EnviarOrcamento(uint Id, uint idAgendamento, decimal valorProposto, uint validade, string descricao, byte[] MateriaisAdicionais)
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

            if (validade <= 0)
            {
                throw new ArgumentException("O prazo de validade do orçamento deve ser estritamente maior que zero.", nameof(validade));
            }

            var orcamentoServico = context.Orcamentos.Find(Id);
            
            if (orcamentoServico == null)
            {
                return false;
            }

            

            orcamentoServico.ValorProposto = valorProposto;
            orcamentoServico.Validade = (int)validade;
            orcamentoServico.Descricao = descricao;
            orcamentoServico.MateriaisAdicionais = MateriaisAdicionais;


            context.SaveChanges();

            return true;
        }

        public void Edit(Orcamento orcamento)
        {
            context.Update(orcamento);
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

