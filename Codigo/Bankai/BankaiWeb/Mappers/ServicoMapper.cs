using BankaiWeb.Models;
using Core;
using Core.DTO;
using Mapster;
using System.Collections.Generic;
using System.Linq;

namespace BankaiWeb.Mappers
{
    public static class ServicoMapper
    {
        public static ServicoViewModel ToViewModel(Servico servico)
        {
            var viewModel = servico.Adapt<ServicoViewModel>();
            viewModel.NomeEspecialidade = servico.IdEspecialidadeNavigation?.Nome;
            return viewModel;
        }

        public static Servico ToEntity(ServicoViewModel servicoViewModel)
        {
            return servicoViewModel.Adapt<Servico>();
        }

        public static IEnumerable<ServicoViewModel> ToViewModel(IEnumerable<Servico> servicos)
        {
            return servicos.Select(ToViewModel);
        }

        public static IEnumerable<Servico> ToEntity(IEnumerable<ServicoViewModel> servicosViewModel)
        {
            return servicosViewModel.Adapt<IEnumerable<Servico>>();
        }

        public static IEnumerable<ServicoResultadoViewModel> ToResultadoViewModel(IEnumerable<ServicoBuscaDTO> servicos)
        {
            return servicos.Adapt<IEnumerable<ServicoResultadoViewModel>>();
        }

        public static IEnumerable<ProfissionalResultadoViewModel> ToProfissionalResultadoViewModel(IEnumerable<ProfissionalDisponivelDTO> profissionais)
        {
            return profissionais.Adapt<IEnumerable<ProfissionalResultadoViewModel>>();
        }
    }
}
