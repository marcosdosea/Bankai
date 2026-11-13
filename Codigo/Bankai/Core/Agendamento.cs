using System;
using System.Collections.Generic;

namespace Core;

public partial class Agendamento
{
    public int Id { get; set; }

    /// <summary>
    /// F - FIXO
    /// O - ORÇAMENTO
    /// </summary>
    public string TipoCobranca { get; set; } = null!;

    public string StatusAgendamento { get; set; } = null!;

    public string? Comentario { get; set; }

    public int IdProfissional { get; set; }

    public int IdCliente { get; set; }

    public DateTime HorarioInicio { get; set; }

    public DateTime HorarioFim { get; set; }

    public DateTime Data { get; set; }

    public byte[]? Foto { get; set; }

    public int? IdOrcamento { get; set; }

    public virtual ICollection<Agendamentoservico> Agendamentoservicos { get; set; } = new List<Agendamentoservico>();

    public virtual ICollection<Avaliacao> Avaliacaos { get; set; } = new List<Avaliacao>();

    public virtual ICollection<Chamadogarantium> Chamadogarantia { get; set; } = new List<Chamadogarantium>();

    public virtual Pessoa IdClienteNavigation { get; set; } = null!;

    public virtual Orcamento? IdOrcamentoNavigation { get; set; }

    public virtual Pessoa IdProfissionalNavigation { get; set; } = null!;

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
