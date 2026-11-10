using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BankaiWeb.Models
{
    public class OrcarServicoViewModel
    {
        [Display(Name = "Ordem")]
        [Key]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public uint Id { get; set; }

        [Display(Name = "Valor Mão de Obra Técnica")]
        [Precision(18, 2)]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal ValorMaoDeObra { get; set; }


        [Display(Name = "Validade Deste Orçamento")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int ValidadeDias { get; set; }




        [Display(Name = "Laudo / Parecer descritivo")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres")]
        public string LaudoParecer { get; set; } = null!;
    }
}
