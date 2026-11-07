using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public record AgendamentoServiceDto(
    [property: Required] int IdServico,
    decimal? ValorProposto);

public record CriarAgendamentoDto : IValidatableObject
{
    [Required]
    public int IdCliente { get; init; }
    
    [Required]
    public int IdProfissional { get; init; }

    [Required]
    [Display(Name = "Dia da Semana")]
    public DateOnly Data { get; init; }
    
    [Required]
    [Display(Name = "Horário de Início")]
    public TimeSpan HorarioInicio { get; init; }
    
    [Required]
    [Display(Name = "Horário de Fim")]
    public TimeSpan HorarioFim { get; init; }

    [Required, StringLength(255)]
    [Display(Name = "Descrição do Serviço")]
    public string DescricaoServico { get; init; } = null!;

    [Required, RegularExpression("^[FO]$", ErrorMessage = "Use 'F' (fixo) ou 'O' (orçamento).")]
    [Display(Name = "Tipo de Cobrança")]
    public string TipoCobranca { get; init; } = null!;

    [Required, MinLength(1, ErrorMessage = "Informe ao menos um serviço.")]
    [Display(Name = "Serviços")]
    public List<AgendamentoServiceDto> Servicos { get; init; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (HorarioFim <= HorarioInicio)
            yield return new ValidationResult("O horário final deve ser maior que o inicial.",
                new[] { nameof(HorarioFim) });

        if (IdCliente == IdProfissional)
            yield return new ValidationResult("Cliente e profissional não podem ser a mesma pessoa.");
    }
}

public record AvaliarAgendamentoDto(
    [property: Required] int Id,
    [property: Range(1, 5)] int Nota,
    [property: StringLength(255)] string? Comentario);