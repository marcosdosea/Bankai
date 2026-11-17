using BankaiWeb.Models;
using Core;
using Mapster;
using Service;

namespace BankaiWeb.Mappers
{
    public class AgendamentoMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidade -> ViewModel
            config.NewConfig<Agendamento, AgendamentoViewModel>()
                .Map(dest => dest.HorarioInicio, src => src.HorarioInicio.TimeOfDay)
                .Map(dest => dest.HorarioFim, src => src.HorarioFim.TimeOfDay)
                .Map(dest => dest.DiaSemana, src => AgendamentoService.ObterDiaSemanaString(src.Data.DayOfWeek))
                .Ignore(dest => dest.FotoUpload);

            // ViewModel -> Entidade
            config.NewConfig<AgendamentoViewModel, Agendamento>()
                .Map(dest => dest.HorarioInicio,
                     src => src.Data.Date.Add(src.HorarioInicio))
                .Map(dest => dest.HorarioFim,
                     src => src.Data.Date.Add(src.HorarioFim))
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