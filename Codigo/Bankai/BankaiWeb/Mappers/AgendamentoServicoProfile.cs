using BankaiWeb.Models;
using Core;
using Mapster;
namespace BankaiWeb.Mappers
{
    public class AgendamentoServicoProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // ViewModel -> Entidade (create/edit): não aceita nota nem comentário do formulário
            config.NewConfig<AgendamentoViewModel, Agendamento>()
                .Ignore(dest => dest.Nota!, dest => dest.Comentario!);

            // Entidade -> ViewModel (exibir): mapeia tudo por nome
            config.NewConfig<Agendamento, AgendamentoViewModel>();
        }
    }
}
