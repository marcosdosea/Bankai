using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PagamentoService : IPagamentoService
    {
        private readonly BankaiContext context;

        public PagamentoService(BankaiContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Insere um novo registro de pagamento na base de dados
        /// </summary>
        public int Create(Pagamento pagamento)
        {
            ValidarDadosPagamento(pagamento);

            if (string.IsNullOrWhiteSpace(pagamento.StatusPagamento))
            {
                pagamento.StatusPagamento = "AP"; // Aguardando Processamento (ENUM do Banco de Dados)
            }

            this.context.Pagamentos.Add(pagamento);
            this.context.SaveChanges();
            return pagamento.Id;
        }

        /// <summary>
        /// Atualiza os dados ou status de um pagamento existente
        /// </summary>
        public void Edit(Pagamento pagamento)
        {
            ValidarDadosPagamento(pagamento);

            this.context.Pagamentos.Update(pagamento);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove um pagamento pelo seu código
        /// </summary>
        public void Delete(int id)
        {
            var pagamento = this.context.Pagamentos.Find(id);
            if (pagamento != null)
            {
                this.context.Pagamentos.Remove(pagamento);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Consulta um pagamento específico pelo id
        /// </summary>
        public Pagamento? Get(int id)
        {
            return this.context.Pagamentos.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Retorna todos os pagamentos registrados
        /// </summary>
        public IEnumerable<Pagamento> GetAll()
        {
            return this.context.Pagamentos.AsNoTracking().ToList();
        }

        /// <summary>
        /// Retorna os pagamentos vinculados a um agendamento
        /// </summary>
        public IEnumerable<Pagamento> GetByAgendamento(int idAgendamento)
        {
            return this.context.Pagamentos.AsNoTracking().Where(p => p.IdAgendamento == idAgendamento).ToList();
        }

        /// <summary>
        /// Processa e registra o pagamento referente a um agendamento/serviço (CSU06)
        /// </summary>
        public Pagamento RealizarPagamento(Pagamento pagamento, bool transacaoCompleta = true)
        {
            ValidarDadosPagamento(pagamento);

            var agendamentoExiste = this.context.Agendamentos.Any(a => a.Id == pagamento.IdAgendamento);

            if (!agendamentoExiste)
            {
                throw new Exception("Por favor, preencha todos os campos obrigatórios corretamente.");
            }

            if (!transacaoCompleta)
            {
                pagamento.StatusPagamento = "R"; // Recusado (ENUM do Banco de Dados)
                this.context.Pagamentos.Add(pagamento);
                this.context.SaveChanges();

                throw new Exception("Pagamento não autorizado pela instituição financeira.");
            }

            pagamento.StatusPagamento = "A"; // Aprovado (ENUM do Banco de Dados)
            this.context.Pagamentos.Add(pagamento);
            this.context.SaveChanges();

            return pagamento;
        }

        private void ValidarDadosPagamento(Pagamento pagamento)
        {
            if (pagamento == null ||
                pagamento.IdAgendamento <= 0 ||
                pagamento.ValorTransacao <= 0 ||
                string.IsNullOrWhiteSpace(pagamento.FormaPagamento) ||
                (pagamento.FormaPagamento != "P" && pagamento.FormaPagamento != "CC")) // P = PIX & CC = Cartão de Crédito
            {
                throw new Exception("Por favor, preencha todos os campos obrigatórios corretamente.");
            }
        }
    }
}