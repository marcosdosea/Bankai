using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class EspecialidadeViewModel
    {
        [Key]
        [Display(Name = "Código")]
        public uint Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo Nome deve ter no máximo 50 caracteres.")]
        [Display(Name = "Nome da Especialidade")]
        public string Nome { get; set; } = null!;  
    }
}
