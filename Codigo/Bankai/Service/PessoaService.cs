using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    internal class PessoaService : IPessoaService
    {

        private readonly BankaiContext _context;
        public PessoaService(BankaiContext context)
        {
            _context = context;
        }
        public int Create(Pessoa pessoa)
        {
            _context.Add(pessoa);
            _context.SaveChanges();
            return pessoa.Id;   
        }
        public void Edit(Pessoa pessoa)
        {
            _context.Update(pessoa);
            _context.SaveChanges();
        }
        public void Delete(uint id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if(pessoa != null)
            {
                _context.Remove(pessoa);
                _context.SaveChanges();
            }
        }
        public Pessoa get(uint id)
        {
            return _context.Pessoas.Find(id);
        }
        public IEnumerable<Pessoa> GetAll()
        {
            return _context.Pessoas.ToList();
        }
        public IEnumerable<Pessoa> GetAllByName(string name)
        {   
            return _context.Pessoas.Where(p => p.Nome.Contains(name)).ToList();
        }
    }
}
