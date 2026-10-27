using System;
using System.Collections.Generic;

namespace Core;

public partial class Profissionalservico
{
    public decimal Preco { get; set; }

    public int IdPessoa { get; set; }

    public int IdServico { get; set; }

    public virtual Pessoa IdPessoaNavigation { get; set; } = null!;

    public virtual Servico IdServicoNavigation { get; set; } = null!;
}
