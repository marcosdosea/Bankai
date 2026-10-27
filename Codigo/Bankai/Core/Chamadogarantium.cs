using System;
using System.Collections.Generic;

namespace Core;

public partial class Chamadogarantium
{
    public int Id { get; set; }

    public DateTime DataChamado { get; set; }

    public string StatusGarantia { get; set; } = null!;

    public int IdAgendamento { get; set; }

    public virtual Agendamento IdAgendamentoNavigation { get; set; } = null!;
}
