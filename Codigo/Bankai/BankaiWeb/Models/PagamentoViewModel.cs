using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class PagamentoViewModel
    {
        [Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [Display(Name = "Código do Agendamento")]
        public int CodigoAgendamento { get; set; }

        [Required(ErrorMessage = "Por favor, preencha todos os campos obrigatórios corretamente.")]
        [StringLength(20, ErrorMessage = "A forma de pagamento deve ter no máximo 20 caracteres.")]
        [Display(Name = "Forma de Pagamento")]
        public string FormaPagamento { get; set; } = "Pix"; // Valores válidos DD: "Pix" ou "Cartao de credito"

        [StringLength(20)]
        [Display(Name = "Status do Pagamento")]
        public string StatusPagamento { get; set; } = "Aguardando Processamento"; // Valores válidos: "Aguardando Processamento", "Processando", "Recusado", "Aprovado", "Cancelado"

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

        // Validação condicional conforme Fluxo Alternativo e Exceção do CSU06
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FormaPagamento != "Pix" && FormaPagamento != "Cartao de credito")
            {
                yield return new ValidationResult(
                    "Por favor, preencha todos os campos obrigatórios corretamente.",
                    new[] { nameof(FormaPagamento) });
            }

            if (FormaPagamento == "Cartao de credito")
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