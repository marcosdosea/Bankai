using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public int Id { get; set; }

    public string FormaPagamento { get; set; } = null!;

    public string StatusPagamento { get; set; } = null!;

    public decimal ValorTransacao { get; set; }

    public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
}
