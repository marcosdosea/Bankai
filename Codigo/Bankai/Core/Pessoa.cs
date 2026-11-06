using System;
using System.Collections.Generic;

namespace Core;

public partial class Pessoa
{
    public int Id { get; set; }

    public string CpfCnpj { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefone { get; set; }

    public bool EhProfissional { get; set; }

    public string Cep { get; set; } = null!;

    public string Rua { get; set; } = null!;

    public string Bairro { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string NumeroCasa { get; set; } = null!;

    public virtual ICollection<Agendamento> AgendamentoIdClienteNavigations { get; set; } = new List<Agendamento>();

    public virtual ICollection<Agendamento> AgendamentoIdProfissionalNavigations { get; set; } = new List<Agendamento>();

    public virtual ICollection<Disponibilidade> Disponibilidades { get; set; } = new List<Disponibilidade>();

    public virtual ICollection<Profissionalservico> Profissionalservicos { get; set; } = new List<Profissionalservico>();
}
