using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IPessoaService
    {
        int Create(Pessoa pessoa);
        void Edit(Pessoa pessoa);
        void Delete(uint id);
        Pessoa get(uint id);
        IEnumerable<Pessoa> GetAll();
        IEnumerable<Pessoa> GetAllByName(string name);
    }
}
