using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class ServicoViewModel
    {
        [Display(Name = "Código")]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Especialidade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma especialidade válida")]
        public int IdEspecialidade { get; set; }

        [Display(Name = "Especialidade")]
        public string? NomeEspecialidade { get; set; }

        [Display(Name = "Nome do Serviço")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Descrição Detalhada")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(255, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
        public string Descricao { get; set; } = null!;
    }
}
