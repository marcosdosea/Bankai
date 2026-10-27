using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTO
{
    public class PessoaDTO
    {
        public uint Id { get; set; }
        public string CpfCnpj { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public string? Telefone { get; set; }
        public bool EhProfissional { get; set; }
        public string Endereco { get; set; } = null!;
    }
}
