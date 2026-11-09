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
        IEnumerable<Disponibilidade> GetByIdPessoa(uint idPessoa);
    }
}