using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class PagamentoViewModel : IValidatableObject
    {
        [Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [Display(Name = "Código do Agendamento")]
        public int CodigoAgendamento { get; set; }

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [StringLength(2, ErrorMessage = "Forma de pagamento inválida.")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = "P"; // ENUM Banco de Dados: "P" (Pix) ou "CC" (Cartão de Crédito)

        [StringLength(2)]
        [Display(Name = "Status do Pagamento")]
        public string StatusPagamento { get; set; } = "AP"; // ENUM Banco de Dados: "AP", "P", "R", "A", "C"

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [Range(0.01, 99999999.99, ErrorMessage = "O valor da transação deve ser maior que zero.")]
        [Display(Name = "Valor da Transação (R$)")]
        public decimal ValorTransacao { get; set; }

        // Campos exibidos no Fluxo Alternativo da Tela_008 (Aba Cartão de Crédito - CSU06)
        [Display(Name = "Número do Cartão")]
        [StringLength(19)]
        public string? NumeroCartao { get; set; }

        [Display(Name = "Nome Impresso no Cartão")]
        [StringLength(100)]
        public string? NomeTitularCartao { get; set; }

        [Display(Name = "Validade (MM/AA)")]
        [StringLength(5)]
        public string? ValidadeCartao { get; set; }

        [Display(Name = "CVV")]
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