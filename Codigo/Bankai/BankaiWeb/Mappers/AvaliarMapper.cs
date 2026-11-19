using BankaiWeb.Models;
using Core;
using Mapster;

namespace BankaiWeb.Mappers
{
    public static class AvaliarMapper
    {
        public static AvaliarViewModel ToViewModel(Avaliacao avaliacao)
        {
            return avaliacao.Adapt<AvaliarViewModel>();
        }

        public static Avaliacao ToEntity(AvaliarViewModel avaliarViewModel)
        {
            return avaliarViewModel.Adapt<Avaliacao>();
        }

        public static IEnumerable<AvaliarViewModel> ToViewModel(IEnumerable<Avaliacao> avaliacoes)
        {
            return avaliacoes.Adapt<IEnumerable<AvaliarViewModel>>();
        }

        public static IEnumerable<Avaliacao> ToEntity(IEnumerable<AvaliarViewModel> avaliacoesViewModel)
        {
            return avaliacoesViewModel.Adapt<IEnumerable<Avaliacao>>();
        }
    }
}

