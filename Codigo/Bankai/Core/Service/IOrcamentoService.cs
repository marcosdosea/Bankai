using Core.DTO;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IOrcamentoService
    {
        /// <summary>
        /// Obtém os dados de orçamento para um agendamento e serviço específicos.
        /// </summary>
        OrcamentoDTO? Get(int idAgendamento, int idServico);

        /// <summary>
        /// Obtém todos os orçamentos cadastrados (agendamentos de tipo cobrança 'O' - Orçamento).
        /// </summary>
        IEnumerable<OrcamentoDTO> GetAll();

        /// <summary>
        /// Obtém todos os agendamentos que necessitam ou possuem orçamento para um profissional específico.
        /// </summary>
        IEnumerable<OrcamentoDTO> GetByProfissional(int idProfissional);

        /// <summary>
        /// Registra/atualiza a proposta de orçamento com o valor acordado, aplicando as taxas da plataforma (RN01).
        /// </summary>
        /// <param name="idAgendamento">Identificador do agendamento.</param>
        /// <param name="idServico">Identificador do serviço vinculado.</param>
        /// <param name="valorProposto">Valor bruto da mão de obra/serviço orçado.</param>
        /// <returns>True se atualizado com sucesso, False se o agendamento/serviço não existir.</returns>
        bool EnviarOrcamento(int idAgendamento, int idServico, decimal valorProposto);

        /// <summary>
        /// Calcula a taxa de intermediação da plataforma (15%) conforme RN01.
        /// </summary>
        decimal CalcularTaxaPlataforma(decimal valorBruto);

        /// <summary>
        /// Calcula o repasse líquido ao profissional (85%) conforme RN01.
        /// </summary>
        decimal CalcularValorLiquido(decimal valorBruto);
    }
}

