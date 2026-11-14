using System;
using System.Collections.Generic;

namespace Core;

public partial class Orcamento
{
    public int Id { get; set; }

    public decimal ValorProposto { get; set; }

    public int Validade { get; set; }

    public byte[]? MateriaisAdicionais { get; set; }

    public string Descricao { get; set; } = null!;

    public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
}
