using System;
using System.Collections.Generic;

namespace Core;

public partial class Profissionalespecialidade
{
    public int IdPessoa { get; set; }

    public int IdEspecialidade { get; set; }

    public virtual Especialidade IdEspecialidadeNavigation { get; set; } = null!;

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;
}
