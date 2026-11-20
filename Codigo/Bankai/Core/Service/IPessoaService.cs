using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IPessoaService
    {
        int Create(Pessoa pessoa);
        void Edit (Pessoa pessoa);
        void Delete(int id);
        Pessoa get(int id);
        IEnumerable<Pessoa> GetAll();
        IEnumerable<Pessoa> GetAllByName(string name);
    }
}
