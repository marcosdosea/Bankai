using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class OrcamentoViewModel
    {
        [Display(Name = "Código")]
        public uint Id { get; set; }

        [Required(ErrorMessage = "Id do agendamento é obrigatório.")]
        [Display(Name = "Id do Agendamento")]
        public uint IdAgendamento { get; set; }

        [Required(ErrorMessage = "O valor proposto é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do orçamento deve ser maior que zero.")]
        [Display(Name = "Valor Proposto (R$)")]
        [DataType(DataType.Currency)]
        public decimal ValorProposto { get; set; }

        [Required(ErrorMessage = "A validade é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A validade deve ser maior que zero.")]
        [Display(Name = "Validade (em dias)")]
        public int Validade { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        [Display(Name = "Descrição do Serviço")]
        public string Descricao { get; set; } = string.Empty;

        [Display(Name = "Materiais Adicionais (Imagem)")]
        public IFormFile? ArquivoMateriais { get; set; }

        [Display(Name = "Materiais Adicionais")]
        public byte[]? MateriaisAdicionais { get; set; }

        [Display(Name = "Taxa da Plataforma (15%)")]    
        [DataType(DataType.Currency)]
        public decimal TaxaPlataforma { get; set; }

        [Display(Name = "Valor Líquido (85%)")]
        [DataType(DataType.Currency)]
        public decimal ValorLiquido { get; set; }
    }
}

