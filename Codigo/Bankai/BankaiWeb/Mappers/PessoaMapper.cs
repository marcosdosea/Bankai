using BankaiWeb.Models;
using Core;
using Mapster;


namespace BankaiWeb.Mappers
{
    public class PessoaMapper
    {
        public static PessoaViewModel ToViewModel(Pessoa pessoa)
        {
            var pessoaViewModel = pessoa.Adapt<PessoaViewModel>();
            pessoaViewModel.id = checked((uint)pessoa.Id);
            return pessoaViewModel;
        }

        public static Pessoa ToEntity(PessoaViewModel pessoaViewModel)
        {
            var pessoa = pessoaViewModel.Adapt<Pessoa>();
            pessoa.Id = checked((int)pessoaViewModel.id);
            return pessoa;
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
