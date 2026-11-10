using System;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IServicoService
    {
        int Create(Servico servico);
        void Edit(Servico servico);
        void Delete(int id);
        Servico? Get(int id);
        IEnumerable<Servico> GetAll();
        IEnumerable<Servico> GetByNome(string nome);
    }
}

