using BankaiWeb.Models;
using Core;
using Mapster;

namespace BankaiWeb.Mappers
{
    public class AgendamentoServicoProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidade -> ViewModel (exibir)
            config.NewConfig<Agendamento, AgendamentoViewModel>();

            // ViewModel -> Entidade (criar/editar)
            config.NewConfig<AgendamentoViewModel, Agendamento>()
                .Ignore(dest => dest.Agendamentoservicos)
                .Ignore(dest => dest.Avaliacaos)
                .Ignore(dest => dest.Chamadogarantia)
                .Ignore(dest => dest.Pagamentos)
                .Ignore(dest => dest.IdClienteNavigation)
                .Ignore(dest => dest.IdProfissionalNavigation)
                .Ignore(dest => dest.IdOrcamentoNavigation);
        }
    }
}
