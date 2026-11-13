using System;
using System.Collections.Generic;

namespace Core;

public partial class Avaliacao
{
    public int Id { get; set; }

    public int Pontualidade { get; set; }

    public int LimpezaLocal { get; set; }

    public int QualidadeReparo { get; set; }

    public string? Depoimento { get; set; }

    public int IdAgendamento { get; set; }

    public virtual Agendamento IdAgendamentoNavigation { get; set; } = null!;
}
