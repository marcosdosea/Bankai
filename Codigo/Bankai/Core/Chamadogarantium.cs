using System;
using System.Collections.Generic;

namespace Core;

public partial class Chamadogarantium
{
    public int Id { get; set; }

    public DateTime DataChamado { get; set; }

    /// <summary>
    /// S - Solicitada
    /// A - Agendado
    /// R - Rota
    /// C - Concluído
    /// E - Execucao
    /// </summary>
    public string StatusGarantia { get; set; } = null!;

    public int IdAgendamento { get; set; }

    public string Descricao { get; set; } = null!;

    public byte[]? Foto { get; set; }

    public virtual Agendamento IdAgendamentoNavigation { get; set; } = null!;
}
