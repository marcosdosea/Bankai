using BankaiWeb.Models;
using Core;
using Mapster;
using System.Collections.Generic;

namespace BankaiWeb.Mappers
{
    public class EspecialidadeMapper
    {
        public static EspecialidadeViewModel ToViewModel(Especialidade especialidade)
        {
            return especialidade.Adapt<EspecialidadeViewModel>();
        }

        public static Especialidade ToEntity(EspecialidadeViewModel especialidadeViewModel)
        {
            return especialidadeViewModel.Adapt<Especialidade>();
        }

        public static IEnumerable<EspecialidadeViewModel> ToViewModel(IEnumerable<Especialidade> especialidades)
        {
            return especialidades.Adapt<IEnumerable<EspecialidadeViewModel>>();
        }

        public static IEnumerable<Especialidade> ToEntity(IEnumerable<EspecialidadeViewModel> especialidadesViewModel)
        {
            return especialidadesViewModel.Adapt<IEnumerable<Especialidade>>();
        }
    }
}
