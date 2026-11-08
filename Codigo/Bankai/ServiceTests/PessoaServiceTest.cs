using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class PessoaServiceTest
{
    private BankaiContext contexto = null!;
    private IPessoaService servicoPessoa = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"BancoDeDadosTeste_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<BankaiContext>()
            .UseInMemoryDatabase(databaseName: nomeBanco)
            .Options;

        contexto = new BankaiContext(opcoes);
        
        contexto.Pessoas.AddRange(
            new Pessoa 
            {
                Id = 1,
                CpfCnpj = "12345678910",
                Nome = "João Silva",
                Email = "joaosilva@gmail.com",
                Telefone = "79123456789",
                EhProfissional = false,
                Cep = "12345678",
                Rua = "Rua A",
                Bairro = "Bairro A",
                Cidade = "Cidade A",
                Estado = "Estado A",
                NumeroCasa = "123",
            },

            new Pessoa
            {
                Id = 2,
                CpfCnpj = "98765432100",
                Nome = "Maria Souza",
                Email = "mariasouza@gmail.com",
                Telefone = "79987654321",
                EhProfissional = false,
                Cep = "87654321",
                Rua = "Rua B",
                Bairro = "Bairro B",
                Cidade = "Cidade B",
                Estado = "Estado B",
                NumeroCasa = "456",
            }
        );

        contexto.SaveChanges();
        servicoPessoa = new PessoaService(contexto);
    }
