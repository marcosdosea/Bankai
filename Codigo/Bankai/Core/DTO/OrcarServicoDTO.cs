using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTO
{
    public class OrcarServicoDTO
    {
        public decimal OrcamentoBruto { get; set; }
        public decimal ValorIntermediacao { get; set; }
        public decimal RepasseLiquido { get; set; }

        public decimal ValorMaoDeObra { get; set; }

        public decimal ValorMateriais { get; set; }

    }
}
