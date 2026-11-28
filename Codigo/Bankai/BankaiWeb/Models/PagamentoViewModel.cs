using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class PagamentoViewModel : IValidatableObject
    {
        [Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [Display(Name = "Ordem Vincular")]
        public int CodigoAgendamento { get; set; }

        // Dados informativos do Resumo do Pedido (Tela_008) vindos da tela anterior
        [Display(Name = "Ordem Vincular")]
        public string OrdemVincular => CodigoAgendamento > 0 ? $"#BKS-{CodigoAgendamento}" : string.Empty;

        [Display(Name = "Serviço Adquirido")]
        public string ServicoAdquirido { get; set; } = string.Empty;

        [Display(Name = "Profissional")]
        public string NomeProfissional { get; set; } = string.Empty;

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [StringLength(2, ErrorMessage = "Forma de pagamento inválida.")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = "CC";

        [StringLength(2)]
        [Display(Name = "Status do Pagamento")]
        public string StatusPagamento { get; set; } = "AP";

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [Range(0.01, 99999999.99, ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [Display(Name = "Total a Reter")]
        public decimal ValorTransacao { get; set; }

        // Campos exibidos no Fluxo Alternativo da Tela_008 (Aba Cartão de Crédito - CSU06)
        [Display(Name = "Número do Cartão")]
        [StringLength(19)]
        public string? NumeroCartao { get; set; }

        [Display(Name = "Nome Impresso no Cartão")]
        [StringLength(100)]
        public string? NomeTitularCartao { get; set; }

        [Display(Name = "Validade")]
        [StringLength(5)]
        public string? ValidadeCartao { get; set; }

        [Display(Name = "CVC (Código de Segurança)")]
        [StringLength(4)]
        public string? CvvCartao { get; set; }

        // Validação condicional executada automaticamente pelo ASP.NET MVC via IValidatableObject
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FormaPagamento != "P" && FormaPagamento != "CC")
            {
                yield return new ValidationResult(
                    "Por favor, preencha todos os campos obrigatórios corretamente.",
                    new[] { nameof(FormaPagamento) });
            }

            if (FormaPagamento == "CC")
            {
                if (string.IsNullOrWhiteSpace(NumeroCartao) ||
                    string.IsNullOrWhiteSpace(NomeTitularCartao) ||
                    string.IsNullOrWhiteSpace(ValidadeCartao) ||
                    string.IsNullOrWhiteSpace(CvvCartao))
                {
                    yield return new ValidationResult(
                        "Por favor, preencha todos os campos obrigatórios corretamente.",
                        new[] { nameof(NumeroCartao), nameof(NomeTitularCartao), nameof(ValidadeCartao), nameof(CvvCartao) });
                }
            }
        }
    }
}