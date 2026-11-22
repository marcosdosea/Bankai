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

        [DataType(DataType.Date)]
        [Display(Name = "Data Específica")]
        public DateTime? DataIndisponibilizade { get; set; }

        [StringLength(255, ErrorMessage = "O motivo da indisponibilidade deve ter no máximo 255 caracteres.")]
        [Display(Name = "Motivo da Indisponibilidade")]
        public string? DescricaoIndisponibilidade { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HorarioInicio.HasValue && HorarioFim.HasValue && HorarioFim <= HorarioInicio)
            {
                yield return new ValidationResult(
                    "O horário de término deve ser posterior ao horário de início.",
                    new[] { nameof(HorarioFim) });
            }

            if (DataIndisponibilizade.HasValue)
            {
                if (string.IsNullOrWhiteSpace(DescricaoIndisponibilidade))
                {
                    yield return new ValidationResult(
                        "Informe o motivo da indisponibilidade quando uma data específica for preenchida.",
                        new[] { nameof(DescricaoIndisponibilidade) });
                }

                var diaEsperado = ObterDiaSemanaSigla(DataIndisponibilizade.Value.DayOfWeek);
                if (!string.IsNullOrWhiteSpace(DiaSemana) && DiaSemana != diaEsperado)
                {
                    yield return new ValidationResult(
                        "O dia da semana deve corresponder à data específica informada.",
                        new[] { nameof(DiaSemana) });
                }
            }
            else if (!string.IsNullOrWhiteSpace(DescricaoIndisponibilidade))
            {
                yield return new ValidationResult(
                    "Informe a data específica quando um motivo de indisponibilidade for preenchido.",
                    new[] { nameof(DataIndisponibilizade) });
            }
        }

        private static string ObterDiaSemanaSigla(DayOfWeek dia) => dia switch
        {
            DayOfWeek.Sunday => "D",
            DayOfWeek.Monday => "SF",
            DayOfWeek.Tuesday => "TF",
            DayOfWeek.Wednesday => "QF",
            DayOfWeek.Thursday => "QIF",
            DayOfWeek.Friday => "SXF",
            DayOfWeek.Saturday => "S",
            _ => "D"
        };
    }
}