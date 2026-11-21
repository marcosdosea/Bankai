using BankaiWeb.Models;
using Core;
using Mapster;

namespace BankaiWeb.Mappers
{
    public class PessoaMapper
    {
        public static PessoaViewModel ToViewModel(Pessoa pessoa)
        {
            return pessoa.Adapt<PessoaViewModel>();
        }

        public static Pessoa ToEntity(PessoaViewModel pessoaViewModel)
        {
            return pessoaViewModel.Adapt<Pessoa>();
        }

        public static IEnumerable<PessoaViewModel> ToViewModel(IEnumerable<Pessoa>? pessoas)
        {
            return pessoas?.Select(ToViewModel) ?? Enumerable.Empty<PessoaViewModel>();
        }

        public static IEnumerable<Pessoa> ToEntity(IEnumerable<PessoaViewModel>? pessoasViewModel)
        {
            return pessoasViewModel?.Select(ToEntity) ?? Enumerable.Empty<Pessoa>();
        }
    }
}