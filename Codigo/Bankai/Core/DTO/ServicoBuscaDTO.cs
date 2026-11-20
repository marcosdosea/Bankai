namespace Core.DTO
{
    public class ServicoBuscaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int IdEspecialidade { get; set; }
        public string NomeEspecialidade { get; set; } = string.Empty;
        public int QuantidadeProfissionais { get; set; }
        public decimal? MenorPreco { get; set; }
    }
}
