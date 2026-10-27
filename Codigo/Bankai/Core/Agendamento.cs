using System;
using System.Collections.Generic;

namespace Core;

public partial class Agendamento
{
    public int Id { get; set; }

    public string DescricaoServico { get; set; } = null!;

    public string TipoCobranca { get; set; } = null!;

    public string StatusAgendamento { get; set; } = null!;

    public int IdPagamento { get; set; }

    public string Nota { get; set; } = null!;

    public string? Comentario { get; set; }

    public int IdProfissional { get; set; }

    public int IdCliente { get; set; }

    public virtual ICollection<Agendamentoservico> Agendamentoservicos { get; set; } = new List<Agendamentoservico>();

    public virtual ICollection<Chamadogarantium> Chamadogarantia { get; set; } = new List<Chamadogarantium>();

    public virtual Pessoa IdClienteNavigation { get; set; } = null!;

    public virtual Pagamento IdPagamentoNavigation { get; set; } = null!;

    public virtual Pessoa IdProfissionalNavigation { get; set; } = null!;
}
