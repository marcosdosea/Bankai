using System;
using System.Collections.Generic;

namespace Core;

public partial class Agendamentoservico
{
    public int IdAgendamento { get; set; }

    public int IdServico { get; set; }

    public decimal ValorProposto { get; set; }

    public string StatusServico { get; set; } = null!;

    public virtual Agendamento IdAgendamentoNavigation { get; set; } = null!;

    public virtual Servico IdServicoNavigation { get; set; } = null!;
}
