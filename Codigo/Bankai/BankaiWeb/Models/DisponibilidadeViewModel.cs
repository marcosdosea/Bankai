using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class DisponibilidadeViewModel : IValidatableObject
    {
        [Key]
        [Display(Name = "Código")]
        public uint Id { get; set; } 

        [Required(ErrorMessage = "O campo Status da Disponibilidade é obrigatório.")]
        [RegularExpression("^(D|ND)$", ErrorMessage = "O status deve ser 'D' ou 'ND'.")]
        [Display(Name = "Status da Disponibilidade")]
        public string StatusDisponibilidade { get; set; } = null!;

        [Required(ErrorMessage = "O campo Dia da Semana é obrigatório.")]
        [RegularExpression("^(D|SF|TF|QF|QIF|SXF|S)$", ErrorMessage = "Selecione um dia da semana válido.")]
        [Display(Name = "Dia da Semana")]
        public string DiaSemana { get; set; } = null!;

        [Required(ErrorMessage = "O horário de início é obrigatório.")]
        [DataType(DataType.Time)]
        [Display(Name = "Horário de Início")]
        public TimeSpan? HorarioInicio { get; set; }

        [Required(ErrorMessage = "O horário de término é obrigatório.")]
        [DataType(DataType.Time)]
        [Display(Name = "Horário de Término")]
        public TimeSpan? HorarioFim { get; set; }

        [Required(ErrorMessage = "O código do profissional é obrigatório.")]
        [Display(Name = "Profissional")]
        public uint? IdPessoa { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioInicio.HasValue && HorarioFim.HasValue && HorarioFim <= HorarioInicio)
            {
                yield return new ValidationResult(
                    "O horário de término deve ser posterior ao horário de início.",
                    new[] { nameof(HorarioFim) });
            }
        }
    }
}