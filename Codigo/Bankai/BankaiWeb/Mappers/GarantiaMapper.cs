using BankaiWeb.Models;
using Core;
using Mapster;

namespace BankaiWeb.Mappers
{
    public class GarantiaMapper
    {
        public static GarantiaViewModel ToViewModel(Chamadogarantium garantia)
        {
            return garantia.Adapt<GarantiaViewModel>();
        }

        public static Chamadogarantium ToEntity(GarantiaViewModel garantiaViewModel)
        {
            return garantiaViewModel.Adapt<Chamadogarantium>();
        }

        public static IEnumerable<GarantiaViewModel> ToViewModel(IEnumerable<Chamadogarantium> chamados)
        {
            return chamados.Adapt<IEnumerable<GarantiaViewModel>>();
        }

        public static IEnumerable<Chamadogarantium> ToEntity(IEnumerable<GarantiaViewModel> garantiasViewModel)
        {
            return garantiasViewModel.Adapt<IEnumerable<Chamadogarantium>>();
        }
    }
}
