using System.ComponentModel.DataAnnotations;

namespace Core.DTO;

public class AgendamentoDTO{
   
    public int IdCliente { get; init; }
    
    public int IdProfissional { get; init; }

    [Display(Name = "Dia da Semana")]
    public string diaSemana { get; init; } = null!;
    
    [Display(Name = "Horário de Início")]
    public TimeSpan HorarioInicio { get; init; }
    
    [Display(Name = "Horário de Fim")]
    public TimeSpan HorarioFim { get; init; }

    [Display(Name = "Descrição do Serviço")]
    public string DescricaoServico { get; init; } = null!;

    [Display(Name = "Tipo de Cobrança")]
    public string TipoCobranca { get; init; } = null!;

    [Display(Name = "Serviços")]
    public List<AgendamentoDTO> Servicos { get; init; } = new();

}
