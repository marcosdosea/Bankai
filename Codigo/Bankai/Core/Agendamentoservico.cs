using System;
using System.Collections.Generic;

namespace Core;

public partial class Agendamentoservico
{
    public int IdAgendamento { get; set; }

    public int IdServico { get; set; }

    public decimal ValorProposto { get; set; }


    public string StatusServico { get; set; } = null!;

    public virtual Agendamento IdAgendamentoNavigation { get; set; } = null!;

    public virtual Servico IdServicoNavigation { get; set; } = null!;

    private const decimal PERCENTUAL_BANKAI = 0.15m; // 15% de Intermediação

    // Entradas informadas
    public decimal ValorMaoDeObra { get; set; }
    public decimal ValorMateriais { get; set; }

    // Propriedades calculadas
    public decimal OrcamentoBruto { get; private set; }
    public decimal ValorIntermediacao { get; private set; }
    public decimal RepasseLiquido { get; private set; }

    /// <summary>
    /// Realiza o cálculo do orçamento bruto, desconto da taxa Bankai e o repasse líquido.
    /// </summary>
    public void CalcularOrcamento()
    {
        if (ValorMaoDeObra < 0 || ValorMateriais <= 0)
            throw new InvalidOperationException("Os valores não podem ser negativos.");

        // 1. Orçamento Bruto = Mão de Obra + Materiais
        OrcamentoBruto = ValorMaoDeObra + ValorMateriais;

        // 2. Intermediação Bankai (15%) sobre o Bruto
        ValorIntermediacao = OrcamentoBruto * PERCENTUAL_BANKAI;

        // 3. Repasse Líquido final
        RepasseLiquido = OrcamentoBruto - ValorIntermediacao;

        ValorProposto = OrcamentoBruto; // Atualiza o valor proposto com o orçamento bruto
    }
}
