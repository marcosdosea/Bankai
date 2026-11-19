using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class ServicoResultadoViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int IdEspecialidade { get; set; }
        public string NomeEspecialidade { get; set; } = string.Empty;
        public int QuantidadeProfissionais { get; set; }
        public decimal? MenorPreco { get; set; }
    }

    public class BuscarServicoViewModel
    {
        [Display(Name = "Especialidade ou Serviço")]
        [StringLength(50, ErrorMessage = "O termo de busca deve ter no máximo {1} caracteres.")]
        public string? Termo { get; set; }

        [Display(Name = "Especialidade")]
        public int? IdEspecialidade { get; set; }

        [Display(Name = "Data desejada")]
        [DataType(DataType.Date)]
        public DateTime? DataDesejada { get; set; }

        public IEnumerable<EspecialidadeViewModel> Especialidades { get; set; } = new List<EspecialidadeViewModel>();
        public IEnumerable<ServicoResultadoViewModel> Resultados { get; set; } = new List<ServicoResultadoViewModel>();
    }
}
