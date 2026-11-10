using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IDisponibilidadeService
    {
        uint Create(Disponibilidade disponibilidade);
        void Edit(Disponibilidade disponibilidade);
        void Delete(uint id);
        Disponibilidade? Get(uint id);
        IEnumerable<Disponibilidade> GetAll();
        uint? GetFirstProfissionalId();
        bool IsProfissional(uint idPessoa);
        IEnumerable<Disponibilidade> GetByIdPessoa(uint idPessoa);
    }
}