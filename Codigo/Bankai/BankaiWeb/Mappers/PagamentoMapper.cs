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
            TypeAdapterConfig<PagamentoViewModel, Pagamento>.NewConfig()
                .Map(dest => dest.Id, src => src.Codigo)
                .Map(dest => dest.IdAgendamento, src => src.CodigoAgendamento);

            TypeAdapterConfig<Pagamento, PagamentoViewModel>.NewConfig()
                .Map(dest => dest.Codigo, src => src.Id)
                .Map(dest => dest.CodigoAgendamento, src => src.IdAgendamento);
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