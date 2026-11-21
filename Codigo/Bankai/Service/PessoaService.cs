using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class PessoaService : IPessoaService
    {

        private readonly BankaiContext _context;
        public PessoaService(BankaiContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Cria uma nova pessoa no banco de dados e retorna o id da pessoa criada.
        /// </summary>
        public int Create(Pessoa pessoa)
        {
            _context.Add(pessoa);
            _context.SaveChanges();
            return pessoa.Id;   
        }

        /// <summary>
        /// Atualiza as informações de uma pessoa existente no banco de dados. Se a pessoa não for encontrada, nenhuma ação será realizada.    
        /// </summary>
        public void Edit(Pessoa pessoa)
        {
            _context.Update(pessoa);
            _context.SaveChanges();
        }
        /// <summary>
        /// Exclui uma pessoa do banco de dados com base no id fornecido. Se a pessoa não for encontrada, nenhuma ação será realizada.
        /// </summary>
        public void Delete(int id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if(pessoa != null)
            {
                _context.Remove(pessoa);
                _context.SaveChanges();
            }
        }
        /// <summary>
        /// Obtém uma pessoa pelo id.
        /// </summary>
        public Pessoa get(int id)
        {
            return _context.Pessoas.Find(id);
        }
        /// <summary>
        /// Recupera todas as pessoas do banco de dados.
        /// </summary>
        public IEnumerable<Pessoa> GetAll()
        {
            return _context.Pessoas.ToList();
        }
        /// <summary>
        /// Recupera todas as pessoas cujo nome contém o texto informado.
        /// </summary>
        public IEnumerable<Pessoa> GetAllByName(string name)
        {   
            return _context.Pessoas.Where(p => p.Nome.Contains(name)).ToList();
        }
    }
}
