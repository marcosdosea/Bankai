using System;
using System.Collections.Generic;

namespace Core;

public partial class Pagamento
{
    public int Id { get; set; }

    /// <summary>
    /// P - Pix
    /// CC - Cartão de Crédito
    /// </summary>
    public string FormaPagamento { get; set; } = null!;

    /// <summary>
    /// AP -  Aguardando Processamento
    /// P -  Processando
    /// R - Recusado
    /// A - Aprovado
    /// C - Cancelado
    /// </summary>
    public string StatusPagamento { get; set; } = null!;

    public decimal ValorTransacao { get; set; }

    public int IdAgendamento { get; set; }

    public virtual Agendamento IdAgendamentoNavigation { get; set; } = null!;
}
