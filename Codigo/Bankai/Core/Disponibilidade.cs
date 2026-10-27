using System;
using System.Collections.Generic;

namespace Core;

public partial class Disponibilidade
{
    public int Id { get; set; }

    public string StatusAgenda { get; set; } = null!;

    public string DiaSemana { get; set; } = null!;

    public string HorarioInicio { get; set; } = null!;

    public string HorarioFim { get; set; } = null!;

    public int IdPessoa { get; set; }

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
}
