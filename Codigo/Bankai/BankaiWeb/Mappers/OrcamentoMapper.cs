using BankaiWeb.Models;
using Core;
using Mapster;

namespace BankaiWeb.Mappers
{
    public class OrcamentoMapper
    {
        public static OrcamentoViewModel ToViewModel(Orcamento orcamento)
        {
            return orcamento.Adapt<OrcamentoViewModel>();
        }

        public static Orcamento ToEntity(OrcamentoViewModel orcamentoViewModel)
        {
            return orcamentoViewModel.Adapt<Orcamento>();
        }

        public static IEnumerable<OrcamentoViewModel> ToViewModel(IEnumerable<Orcamento> orcamentos)
        {
            return orcamentos.Adapt<IEnumerable<OrcamentoViewModel>>();
        }

        public static IEnumerable<Orcamento> ToEntity(IEnumerable<OrcamentoViewModel> orcamentosViewModel)
        {
            return orcamentosViewModel.Adapt<IEnumerable<Orcamento>>();
        }
    }
}