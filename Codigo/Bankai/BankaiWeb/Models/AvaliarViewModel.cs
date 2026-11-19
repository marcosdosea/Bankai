using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class AvaliarViewModel
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Display(Name = "Pontualidade e Assiduidade")]
        [Required(ErrorMessage = "Por favor, selecione uma nota para avaliar o serviço.")]
        [Range(1, 5, ErrorMessage = "Por favor, selecione uma nota para avaliar o serviço.")]
        public int Pontualidade { get; set; }

        [Display(Name = "Limpeza e Organização do Local")]
        [Required(ErrorMessage = "Por favor, selecione uma nota para avaliar o serviço.")]
        [Range(1, 5, ErrorMessage = "Por favor, selecione uma nota para avaliar o serviço.")]
        public int LimpezaLocal { get; set; }

        [Display(Name = "Qualidade Técnica do Reparo")]
        [Required(ErrorMessage = "Por favor, selecione uma nota para avaliar o serviço.")]
        [Range(1, 5, ErrorMessage = "Por favor, selecione uma nota para avaliar o serviço.")]
        public int QualidadeReparo { get; set; }

        [Display(Name = "Depoimento Adicional (Opcional)")]
        [StringLength(255, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? Depoimento { get; set; }

        [Display(Name = "Agendamento")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O agendamento é obrigatório.")]
        public int IdAgendamento { get; set; }
    }
}

