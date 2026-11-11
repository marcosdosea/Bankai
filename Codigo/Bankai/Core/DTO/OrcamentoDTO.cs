namespace Core.DTO
{
    public class OrcamentoDTO
    {
        public int IdAgendamento { get; set; }
        public int IdServico { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public string DescricaoServico { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public int IdProfissional { get; set; }
        public string NomeProfissional { get; set; } = string.Empty;
        public string TipoCobranca { get; set; } = string.Empty;
        public string StatusAgendamento { get; set; } = string.Empty;
        public string StatusServico { get; set; } = string.Empty;
        public decimal ValorProposto { get; set; }
        public decimal TaxaPlataforma { get; set; }
        public decimal ValorLiquido { get; set; }
    }
}

