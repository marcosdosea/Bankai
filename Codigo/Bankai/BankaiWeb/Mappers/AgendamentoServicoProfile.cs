using BankaiWeb.Models;
using Core;
using Mapster;
namespace BankaiWeb.Mappers
{
    public class AgendamentoServicoProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AgendamentoViewModel, Agendamento>().TwoWays()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IdCliente, src => src.IdCliente)
                .Map(dest => dest.IdProfissional, src => src.IdProfissional)
                .Map(dest => dest.DiaSemana, src => src.DiaSemana)
                .Map(dest => dest.HorarioInicio, src => src.HorarioInicio)
                .Map(dest => dest.HorarioFim, src => src.HorarioFim)
                .Map(dest => dest.DescricaoServico, src => src.DescricaoServico)
                .Map(dest => dest.TipoCobranca, src => src.TipoCobranca)
                .Map(dest => dest.Nota, src => src.Nota)
                .Map(dest => dest.Comentario, src => src.Comentario);
        }
    }
}
