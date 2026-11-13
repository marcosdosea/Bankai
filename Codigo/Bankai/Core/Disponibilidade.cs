using System;
using System.Collections.Generic;

namespace Core;

public partial class Disponibilidade
{
    public int Id { get; set; }

    /// <summary>
    /// D - Disponível
    /// ND - Não Disponível
    /// </summary>
    public string StatusDisponibilidade { get; set; } = null!;

    /// <summary>
    /// D - Domingo
    /// SF - Segunda-Feira
    /// TF - Terça-Feira
    /// QF - Quarta-Feira
    /// QIF - Quinta-Feira
    /// SXF - Sexta-Feira
    /// S - Sábado
    /// </summary>
    public string DiaSemana { get; set; } = null!;

    public TimeSpan HorarioInicio { get; set; }

    public TimeSpan HorarioFim { get; set; }

    public int IdPessoa { get; set; }

    public DateTime? DataIndisponibilizade { get; set; }

    public string? DescricaoIndisponibilidade { get; set; }

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
}
