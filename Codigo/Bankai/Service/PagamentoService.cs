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
        /// Insere um novo registro de pagamento na base de dados (retorno int conforme IPagamentoService)
        /// </summary>
        public int Create(Pagamento pagamento)
        {
            ValidarDadosPagamento(pagamento);

            if (string.IsNullOrWhiteSpace(pagamento.StatusPagamento))
            {
                pagamento.StatusPagamento = "Pendente";
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
        public void Delete(uint codigo)
        {
            var pagamento = this.context.Pagamentos.Find((int)codigo);
            if (pagamento != null)
            {
                this.context.Pagamentos.Remove(pagamento);
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Consulta um pagamento específico pelo código (assinatura get com 'g' minúsculo)
        /// </summary>
        public Pagamento? get(uint codigo)
        {
            return this.context.Pagamentos
                .FirstOrDefault(p => p.Id == (int)codigo);
        }

        /// <summary>
        /// Retorna todos os pagamentos registrados (assinatura getAll com 'g' minúsculo)
        /// </summary>
        public IEnumerable<Pagamento> getAll()
        {
            return this.context.Pagamentos
                .AsNoTracking()
                .ToList();
        }

        /// <summary>
        /// Retorna os pagamentos vinculados a um agendamento (assinatura getByAgendamento com parâmetro int)
        /// </summary>
        public IEnumerable<Pagamento> getByAgendamento(int codigoAgendamento)
        {
            return this.context.Pagamentos
                .AsNoTracking()
                .Where(p => p.IdAgendamento == codigoAgendamento)
                .ToList();
        }

        /// <summary>
        /// Processa e registra o pagamento referente a um agendamento/serviço (CSU06)
        /// </summary>
        public Pagamento RealizarPagamento(Pagamento pagamento, bool transacaoAutorizada = true)
        {
            ValidarDadosPagamento(pagamento);

            var agendamentoExiste = this.context.Agendamentos
                .Any(a => a.Id == pagamento.IdAgendamento);

            if (!agendamentoExiste)
            {
                throw new Exception("Por favor, preencha todos os campos obrigatórios corretamente.");
            }

            if (!transacaoAutorizada)
            {
                pagamento.StatusPagamento = "Recusado";
                this.context.Pagamentos.Add(pagamento);
                this.context.SaveChanges();

                throw new Exception("Pagamento não autorizado pela instituição financeira.");
            }

            pagamento.StatusPagamento = "Aprovado";
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
                (pagamento.FormaPagamento != "Pix" && pagamento.FormaPagamento != "Cartao de credito"))
            {
                throw new Exception("Por favor, preencha todos os campos obrigatórios corretamente.");
            }
        }
    }
}