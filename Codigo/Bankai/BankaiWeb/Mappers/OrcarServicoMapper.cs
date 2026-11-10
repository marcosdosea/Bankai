using Mapster;
using BankaiWeb.Models;
using Core;

namespace BankaiWeb.Mapper
{
    public static class OrcarServicoMapper
    {
        public static OrcarServicoViewModel ToViewModel(Agendamentoservico orcarServico)
        {
            return orcarServico.Adapt<OrcarServicoViewModel>();
        }

        public static Agendamentoservico ToEntity(OrcarServicoViewModel orcarServicoViewModel)
        {
            return orcarServicoViewModel.Adapt<Agendamentoservico>();
        }

        public static IEnumerable<OrcarServicoViewModel> ToViewModel(IEnumerable<Agendamentoservico> orcarServicos)
        {
            return orcarServicos.Adapt<IEnumerable<OrcarServicoViewModel>>();
        }

        public static IEnumerable<Agendamentoservico> ToEntity(IEnumerable<OrcarServicoViewModel> orcarServicosViewModel)
        {
            return orcarServicosViewModel.Adapt<IEnumerable<Agendamentoservico>>();
        }
    }
}