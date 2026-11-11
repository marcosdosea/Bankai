using BankaiWeb.Models;
using Core;
using Mapster;

namespace BankaiWeb.Mappers
{
    public static class ServicoMapper
    {
        public static ServicoViewModel ToViewModel(Servico servico)
        {
            return servico.Adapt<ServicoViewModel>();
        }

        public static Servico ToEntity(ServicoViewModel servicoViewModel)
        {
            return servicoViewModel.Adapt<Servico>();
        }

        public static IEnumerable<ServicoViewModel> ToViewModel(IEnumerable<Servico> servicos)
        {
            return servicos.Adapt<IEnumerable<ServicoViewModel>>();
        }

        public static IEnumerable<Servico> ToEntity(IEnumerable<ServicoViewModel> servicosViewModel)
        {
            return servicosViewModel.Adapt<IEnumerable<Servico>>();
        }
    }
}

