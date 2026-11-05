using System;
using System.Collections.Generic;

namespace Core;

public partial class Disponibilidade
{
    public int Id { get; set; }

    public string StatusDisponibilidade { get; set; } = null!;

    public string DiaSemana { get; set; } = null!;

    public TimeSpan HorarioInicio { get; set; }

    public TimeSpan HorarioFim { get; set; }

    public int IdPessoa { get; set; }

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
}
