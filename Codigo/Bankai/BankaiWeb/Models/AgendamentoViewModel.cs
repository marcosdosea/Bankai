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
        [RegularExpression("^(D|SF|TF|QF|QIF|SXF|S)$", ErrorMessage = "Dia da semana inválido")]
        public string DiaSemana { get; set; } = null!;

        // Calculada a partir de DiaSemana (não vem do formulário)
        [Display(Name = "Data do agendamento")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [Display(Name = "Horário de início")]
        [Required(ErrorMessage = "O horário de início é obrigatório")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan HorarioInicio { get; set; }

        [Display(Name = "Horário de término")]
        [Required(ErrorMessage = "O horário de término é obrigatório")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan HorarioFim { get; set; }

        [Display(Name = "Tipo de cobrança")]
        [Required(ErrorMessage = "O tipo de cobrança é obrigatório")]
        [RegularExpression("^(F|O)$", ErrorMessage = "Informe um tipo de cobrança válido")]
        public string TipoCobranca { get; set; } = null!;

        [Display(Name = "Status do Agendamento")]
        public string StatusAgendamento { get; set; } = "Novo";

        [Display(Name = "Comentário")]
        [StringLength(255, ErrorMessage = "O comentário deve ter no máximo 255 caracteres")]
        public string? Comentario { get; set; }

        // Dados salvos no banco
        public byte[]? Foto { get; set; }

        // Arquivo enviado pelo formulário
        [Display(Name = "Foto")]
        public IFormFile? FotoUpload { get; set; }
    }
}