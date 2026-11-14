using BankaiWeb.Models;
using Core;
using Mapster;
namespace BankaiWeb.Mappers
{
    public class AgendamentoServicoProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidade -> ViewModel (exibir): mapeia tudo por nome
            config.NewConfig<Agendamento, AgendamentoViewModel>();
        }
    }
}
