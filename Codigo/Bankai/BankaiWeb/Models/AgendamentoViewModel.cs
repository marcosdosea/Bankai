using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class AgendamentoViewModel : IValidatableObject
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
        [RegularExpression("^(D|SF|TF|QF|QIF|SXF|S)$", ErrorMessage = "Informe um dia da semana válido")]
        public string DiaSemana { get; set; } = null!;

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

        [Display(Name = "Descrição do serviço")]
        [Required(ErrorMessage = "A descrição do serviço é obrigatória")]
        [StringLength(255, ErrorMessage = "A descrição do serviço deve ter no máximo 255 caracteres")]
        public string DescricaoServico { get; set; } = null!;

        [Display(Name = "Tipo de cobrança")]
        [Required(ErrorMessage = "O tipo de cobrança é obrigatório")]
        [RegularExpression("^(F|O)$", ErrorMessage = "Informe um tipo de cobrança válido")]
        public string TipoCobranca { get; set; } = null!;

        [Display(Name = "Nota")]
        [RegularExpression("^(MR|R|B|MB|E)?$", ErrorMessage = "Informe uma nota válida")]
        public string? Nota { get; set; }

        [Display(Name = "Comentário")]
        [StringLength(255, ErrorMessage = "O comentário deve ter no máximo 255 caracteres")]
        public string? Comentario { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (HorarioFim <= HorarioInicio)
                yield return new ValidationResult("O horário final deve ser maior que o inicial.",
                    new[] { nameof(HorarioFim) });

            if (IdCliente == IdProfissional)
                yield return new ValidationResult("Cliente e profissional não podem ser a mesma pessoa.");
        }
    }
}
