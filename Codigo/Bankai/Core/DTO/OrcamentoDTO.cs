namespace Core.DTO
{
    public class OrcamentoDTO
    {
        public int Id { get; set; }

        public int Validade { get; set; }

        public decimal ValorProposto { get; set; }

        public byte[]? MateriaisAdicionais { get; set; }

        public string Descricao { get; set; }
        public decimal TaxaPlataforma { get; set; }
        public decimal ValorLiquido { get; set; }


    }
}

