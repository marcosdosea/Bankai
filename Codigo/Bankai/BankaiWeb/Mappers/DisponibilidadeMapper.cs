using BankaiWeb.Models;
using Core;
using Mapster;
using System.Collections.Generic;

namespace BankaiWeb.Mappers
{
    public class DisponibilidadeMapper
    {
        public static DisponibilidadeViewModel ToViewModel(Disponibilidade disponibilidade)
        {
            return disponibilidade.Adapt<DisponibilidadeViewModel>();
        }

        public static Disponibilidade ToEntity(DisponibilidadeViewModel disponibilidadeViewModel)
        {
            return disponibilidadeViewModel.Adapt<Disponibilidade>();
        }

        public static IEnumerable<DisponibilidadeViewModel> ToViewModel(IEnumerable<Disponibilidade> disponibilidades)
        {
            return disponibilidades.Adapt<IEnumerable<DisponibilidadeViewModel>>();
        }
    }
}