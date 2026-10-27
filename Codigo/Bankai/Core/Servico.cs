using System;
using System.Collections.Generic;

namespace Core;

public partial class Servico
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public virtual ICollection<Agendamentoservico> Agendamentoservicos { get; set; } = new List<Agendamentoservico>();

    public virtual ICollection<Profissionalservico> Profissionalservicos { get; set; } = new List<Profissionalservico>();
}
