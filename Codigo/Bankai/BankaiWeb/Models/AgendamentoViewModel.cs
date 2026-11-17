using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class AgendamentoViewModel
    {
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        [Range(1, int.MaxValue, ErrorMessage = "O cliente é obrigatório")]
        public int IdCliente { get; set; }

        [Display(Name = "Profissional")]
        [Range(1, int.MaxValue, ErrorMessage = "O profissional é obrigatório")]
        public int IdProfissional { get; set; }

        [Display(Name = "Dia da semana")]
        [Required(ErrorMessage = "O dia da semana é obrigatório")]
        public DateTime Data { get; set; }

        [Display(Name = "Horário de início")]
        [Required(ErrorMessage = "O horário de início é obrigatório")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public DateTime HorarioInicio { get; set; }

        [Display(Name = "Horário de término")]
        [Required(ErrorMessage = "O horário de término é obrigatório")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public DateTime HorarioFim { get; set; }

        [Display(Name = "Tipo de cobrança")]
        [Required(ErrorMessage = "O tipo de cobrança é obrigatório")]
        [RegularExpression("^(F|O)$", ErrorMessage = "Informe um tipo de cobrança válido")]
        public string TipoCobranca { get; set; } = null!;

        [Display(Name = "Status do Agendamento")]
        public string StatusAgendamento { get; set; } = "Novo";

        [Display(Name = "Comentário")]
        [StringLength(255, ErrorMessage = "O comentário deve ter no máximo 255 caracteres")]
        public string? Comentario { get; set; }

        [Display(Name = "Foto")]
        public byte[]? Foto { get; set; }

    }
}
