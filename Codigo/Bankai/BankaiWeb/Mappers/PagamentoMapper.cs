using BankaiWeb.Models;
using Core;
using Mapster;
using System.Collections.Generic;

namespace BankaiWeb.Mappers
{
    public static class PagamentoMapper
    {
        static PagamentoMapper()
        {
            TypeAdapterConfig<PagamentoViewModel, Pagamento>.NewConfig();
            TypeAdapterConfig<Pagamento, PagamentoViewModel>.NewConfig();
        }

        public static Pagamento ToEntity(PagamentoViewModel viewModel)
        {
            return viewModel.Adapt<Pagamento>();
        }

        public static PagamentoViewModel ToViewModel(Pagamento entity)
        {
            return entity.Adapt<PagamentoViewModel>();
        }

        public static IEnumerable<PagamentoViewModel> ToViewModel(IEnumerable<Pagamento> entities)
        {
            return entities.Adapt<IEnumerable<PagamentoViewModel>>();
        }
    }
}