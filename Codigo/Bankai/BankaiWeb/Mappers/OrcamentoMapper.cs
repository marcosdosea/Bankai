using BankaiWeb.Models;
using Core.DTO;
using Mapster;
using System.Collections.Generic;

namespace BankaiWeb.Mappers
{
    public static class OrcamentoMapper
    {
        public static OrcamentoViewModel ToViewModel(OrcamentoDTO dto)
        {
            return dto.Adapt<OrcamentoViewModel>();
        }

        public static OrcamentoDTO ToDTO(OrcamentoViewModel viewModel)
        {
            return viewModel.Adapt<OrcamentoDTO>();
        }

        public static IEnumerable<OrcamentoViewModel> ToViewModel(IEnumerable<OrcamentoDTO> dtos)
        {
            return dtos.Adapt<IEnumerable<OrcamentoViewModel>>();
        }
    }
}

