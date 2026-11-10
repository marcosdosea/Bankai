using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IEspecialidadeService
    {
        uint Create(Especialidade especialidade);
        void Edit(Especialidade especialidade);
        void Delete(uint id);
        Especialidade? Get(uint id);
        IEnumerable<Especialidade> GetAll();
        IEnumerable<Especialidade> GetByNome(string nome);
    }
}
