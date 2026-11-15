using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public class AgendamentoDTO
    {
        [Display(Name = "Código")]
        public int Id { get; set; }

        public int IdCliente { get; set; }
        public string NomeCliente { get; set; } = null!;
        public int IdProfissional { get; set; }
        public string NomeProfissional { get; set; } = null!;

        [Display(Name = "Situação")]
        public string StatusAgendamento { get; set; } = null!;

        [Display(Name = "Data")]
        public DateTime Data { get; set; }

        [Display(Name = "Horário de Início")]
        public DateTime HorarioInicio { get; set; }

        [Display(Name = "Horário de Fim")]
        public DateTime HorarioFim { get; set; }

        [Display(Name = "Tipo de Cobrança")]
        public string TipoCobranca { get; set; } = null!;

        public string? Comentario { get; set; }

    }
}