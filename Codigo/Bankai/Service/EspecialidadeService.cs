using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class EspecialidadeService : IEspecialidadeService
    {
        private readonly BankaiContext context;

        public EspecialidadeService(BankaiContext context)
        {
            this.context = context;
        }

        public uint Create(Especialidade especialidade)
        {
            this.context.Add(especialidade);
            this.context.SaveChanges();
            return (uint) especialidade.Id;
        }

        public void Edit(Especialidade especialidade)
        {
            this.context.Update(especialidade);
            this.context.SaveChanges();
        }

        public void Delete(uint id)
        {
            var especialidade = this.context.Especialidades.Find(id);
            if (especialidade != null)
            {
                this.context.Remove(especialidade);
                this.context.SaveChanges();
            }
        }

        public Especialidade? Get(uint id)
        {
            return this.context.Especialidades.Find(id);
        }

        public IEnumerable<Especialidade> GetAll()
        {
            return this.context.Especialidades.AsNoTracking();
        }

        public IEnumerable<Especialidade> GetByNome(string nome)
        {
            var consulta = from especialidade in this.context.Especialidades
                           where especialidade.Nome.StartsWith(nome)
                           orderby especialidade.Nome
                           select especialidade;
            return consulta.AsNoTracking();
        }
    }
}
