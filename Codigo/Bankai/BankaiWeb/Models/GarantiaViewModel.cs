using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class GarantiaViewModel
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O agendamento é obrigatório.")]
        [Display(Name = "Agendamento")]
        public int AgendamentoId { get; set; }

        [Required(ErrorMessage = "A descrição do chamado de garantia é obrigatória.")]
        [StringLength(300, ErrorMessage = "A descrição do chamado de garantia não pode exceder 300 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Display(Name = "Data do Chamado")]
        [DataType(DataType.DateTime)]
        public DateTime DataChamado { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public string Status { get; set; } = "S";

        [Display(Name = "Descrição do Status")]
        public string StatusGarantiaDescricao => Status switch
        {
            "S" => "Solicitada",
            "A" => "Agendada",
            "R" => "Em Rota",
            "E" => "Em Execução",
            "C" => "Concluída",
        };
    }
}
