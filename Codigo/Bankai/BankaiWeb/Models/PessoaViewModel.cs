using System.ComponentModel.DataAnnotations;
namespace BankaiWeb.Models
{
    public class PessoaViewModel
    {
        [Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O campo CPF/CNPJ é obrigatório.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O campo CPF/CNPJ deve ter entre 11 e 14 caracteres.")]
        [Display(Name = "CPF/CNPJ")]
        public string CpfCnpj { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O campo Nome deve ter entre 3 e 50 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        [StringLength(50, ErrorMessage = "O campo Email deve ter no máximo 50 caracteres.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(11, ErrorMessage = "O campo Telefone deve ter no máximo 11 caracteres.")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        [Display(Name = "Profissional")]
        public bool EhProfissional { get; set; }

        [StringLength(8, ErrorMessage = "O campo CEP deve ter no máximo 8 caracteres.")]
        [Display(Name = "CEP")]
        public string Cep { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "O campo Rua deve ter no máximo 50 caracteres.")]
        [Display(Name = "Rua")]
        public string Rua { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "O campo Bairro deve ter no máximo 30 caracteres.")]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "O campo Cidade deve ter no máximo 30 caracteres.")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "O campo Estado deve ter no máximo 30 caracteres.")]
        [Display(Name = "Estado (UF)")]
        public string Estado { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "O campo Número da Casa deve ter no máximo 10 caracteres.")]
        [Display(Name = "Número da Casa")]
        public string NumeroDaCasa { get; set; } = string.Empty;

        public string GetEnderecoCompleto()
        {
            return $"{Rua}, {NumeroDaCasa} - {Bairro}, {Cidade} - {Estado}, CEP: {Cep}";
        }
    }
}
