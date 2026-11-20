using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankaiWeb.Models
{
    public class ProfissionalResultadoViewModel
    {
        public int IdProfissional { get; set; }
        public int IdServico { get; set; }
        public string NomeProfissional { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public decimal Preco { get; set; }
        public decimal NotaMedia { get; set; }
        public int TotalAvaliacoes { get; set; }
        public bool DisponivelNaData { get; set; }
    }

    public class ProfissionaisDisponiveisViewModel
    {
        public int IdServico { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public string DescricaoServico { get; set; } = string.Empty;

        [Display(Name = "Data desejada")]
        [DataType(DataType.Date)]
        public DateTime? DataDesejada { get; set; }

        [Display(Name = "Ordenar por")]
        public string OrdenarPor { get; set; } = "melhor_avaliado";

        [Display(Name = "Apenas Verificados")]
        public bool ApenasVerificados { get; set; }

        [Display(Name = "Atende na Data")]
        public bool ApenasDisponiveisNaData { get; set; }

        [Display(Name = "Preço Máximo (R$)")]
        [Range(0, 999999.99, ErrorMessage = "Informe um valor máximo válido.")]
        public decimal? PrecoMaximo { get; set; }

        public IEnumerable<ProfissionalResultadoViewModel> Profissionais { get; set; } = new List<ProfissionalResultadoViewModel>();
    }
}
