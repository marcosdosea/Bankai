using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IDisponibilidadeService
    {
        uint Create(DisponibilidadeProfissional disponibilidade);
        void Edit(DisponibilidadeProfissional disponibilidade);
        void Delete(uint id);
        DisponibilidadeProfissional? Get(uint id);
        IEnumerable<DisponibilidadeProfissional> GetAll();
        bool Validar(DisponibilidadeProfissional disponibilidade);
    }
}