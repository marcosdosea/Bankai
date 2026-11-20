namespace Core.DTO
{
    public class ProfissionalDisponivelDTO
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
}
