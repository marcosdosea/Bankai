using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class OrcamentoViewModel
    {
        [Display(Name = "Código da Ordem / Agendamento")]
        public int IdAgendamento { get; set; }

        [Display(Name = "Código do Serviço")]
        public int IdServico { get; set; }

        [Display(Name = "Serviço")]
        public string NomeServico { get; set; } = string.Empty;

        [Display(Name = "Solicitação do Cliente")]
        public string DescricaoServico { get; set; } = string.Empty;

        [Display(Name = "Cliente")]
        public string NomeCliente { get; set; } = string.Empty;

        [Display(Name = "Profissional")]
        public string NomeProfissional { get; set; } = string.Empty;

        [Display(Name = "Valor da Mão de Obra Técnica (R$)")]
        [Required(ErrorMessage = "O valor do orçamento é obrigatório.")]
        [Range(0.01, 999999.99, ErrorMessage = "O valor deve ser maior que zero.")]
        [DataType(DataType.Currency)]
        public decimal ValorProposto { get; set; }

        [Display(Name = "Intermediação Bankai (15%)")]
        [DataType(DataType.Currency)]
        public decimal TaxaPlataforma { get; set; }

        [Display(Name = "Seu Repasse Líquido (R$)")]
        [DataType(DataType.Currency)]
        public decimal ValorLiquido { get; set; }

        [Display(Name = "Status do Atendimento")]
        public string StatusServico { get; set; } = string.Empty;

        // --- ATRIBUTOS PRESENTES NA TELA 010 MAS INEXISTENTES NO BANCO ATUAL ---
        // (Deixados documentados na ViewModel para viabilizar apresentação e evolução)

        [Display(Name = "Validade do Orçamento (Dias)")]
        public int? ValidadeOrcamentoDias { get; set; } = 15;

        [Display(Name = "Laudo / Parecer Descritivo")]
        [StringLength(500, ErrorMessage = "O laudo deve ter no máximo 500 caracteres.")]
        public string? LaudoParecerTecnico { get; set; }
    }
}

