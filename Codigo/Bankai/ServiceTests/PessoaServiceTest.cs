using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Service;
using Service;
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
                EhProfissional = true,
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

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdPessoaCriada()
    {
        // Arrange
        var novaPessoa = new Pessoa
        {
            Id = 3,
            CpfCnpj = "11122233344",
            Nome = "Carlos Oliveira",
            Email = "carlosoliveira@gmail.com",
            Telefone = "79999888777",
            EhProfissional = false,
            Cep = "54321098",
            Rua = "Rua C",
            Bairro = "Bairro C",
            Cidade = "Cidade C",
            Estado = "Estado C",
            NumeroCasa = "789",
        };

        // Act
        var idRetornado = servicoPessoa.Create(novaPessoa);

        // Assert
        Assert.AreEqual(3, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarPessoa()
    {
        // Arrange
        var pessoaEsperada = new Pessoa
        {
            Id = 4,
            CpfCnpj = "55566677788",
            Nome = "Ana Lima",
            Email = "analima@gmail.com",
            Telefone = "79876543210",
            EhProfissional = true,
            Cep = "98765432",
            Rua = "Rua D",
            Bairro = "Bairro D",
            Cidade = "Cidade D",
            Estado = "Estado D",
            NumeroCasa = "321",
        };

        // Act
        servicoPessoa.Create(pessoaEsperada);

        // Assert
        var pessoaCriada = servicoPessoa.get(4);

        Assert.IsNotNull(pessoaCriada);
        Assert.AreEqual(pessoaEsperada.Id, pessoaCriada.Id);
        Assert.AreEqual(pessoaEsperada.CpfCnpj, pessoaCriada.CpfCnpj);
        Assert.AreEqual(pessoaEsperada.Nome, pessoaCriada.Nome);
        Assert.AreEqual(pessoaEsperada.Email, pessoaCriada.Email);
        Assert.AreEqual(pessoaEsperada.Telefone, pessoaCriada.Telefone);
        Assert.AreEqual(pessoaEsperada.EhProfissional, pessoaCriada.EhProfissional);
        Assert.AreEqual(pessoaEsperada.Cep, pessoaCriada.Cep);
        Assert.AreEqual(pessoaEsperada.Rua, pessoaCriada.Rua);
        Assert.AreEqual(pessoaEsperada.Bairro, pessoaCriada.Bairro);
        Assert.AreEqual(pessoaEsperada.Cidade, pessoaCriada.Cidade);
        Assert.AreEqual(pessoaEsperada.Estado, pessoaCriada.Estado);
        Assert.AreEqual(pessoaEsperada.NumeroCasa, pessoaCriada.NumeroCasa);
    }

    [TestMethod]
    public void Delete_QuandoPessoaExiste_DeveRemoverPessoa()
    {
        // Act
        servicoPessoa.Delete(2);

        // Assert
        var pessoaDeletada = servicoPessoa.get(2);
        var pessoasRestantes = servicoPessoa.GetAll().ToList();

        Assert.IsNull(pessoaDeletada);
        Assert.HasCount(2, pessoasRestantes);
        Assert.IsFalse(pessoasRestantes.Any(pessoa => pessoa.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoPessoaNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var pessoasIniciais = servicoPessoa.GetAll().ToList();

        // Act
        servicoPessoa.Delete(999); // ID inexistente

        // Assert
        var pessoasFinais = servicoPessoa.GetAll().ToList();

        Assert.HasCount(pessoasIniciais.Count, pessoasFinais);
        CollectionAssert.AreEquivalent(
            pessoasIniciais.Select(pessoa => pessoa.Id).ToList(),
            pessoasFinais.Select(pessoa => pessoa.Id).ToList());
    }

    [TestMethod]
    public void Edit_QuandoPessoaExiste_DeveAtualizarDados()
    {
        // Arrange
        var pessoaExistente = servicoPessoa.get(3);

        Assert.IsNotNull(pessoaExistente);

        pessoaExistente!.Nome = "Carlos Oliveira Atualizado";
        pessoaExistente.Email = "carlos.oliveira.atualizado@gmail.com";

        // Act
        servicoPessoa.Edit(pessoaExistente);

        // Assert
        var pessoaAtualizada = servicoPessoa.get(3);

        Assert.IsNotNull(pessoaAtualizada);
        Assert.AreEqual("Carlos Oliveira Atualizado", pessoaAtualizada.Nome);
        Assert.AreEqual("carlos.oliveira.atualizado@gmail.com", pessoaAtualizada.Email);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarPessoa()
    {
        // Act
        var pessoa = servicoPessoa.get(1);

        // Assert
        Assert.IsNotNull(pessoa);
        Assert.AreEqual(1, pessoa.Id);
        Assert.AreEqual("João Silva", pessoa.Nome);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var pessoa = servicoPessoa.get(999); // ID inexistente

        // Assert
        Assert.IsNull(pessoa);
    }

    [TestMethod]
    public void GetAll_QuandoExistemPessoas_DeveRetornarTodas()
    {
        // Act
        var pessoas = servicoPessoa.GetAll().ToList();

        // Assert
        Assert.IsNotNull(pessoas);
        Assert.HasCount(3, pessoas);

        CollectionAssert.AreEquivalent(
            new List<uint> { 1, 2, 3 },
            pessoas.Select(pessoa => pessoa.Id).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoExistemPessoas_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Pessoas.RemoveRange(contexto.Pessoas);
        contexto.SaveChanges();

        // Act
        var pessoas = servicoPessoa.GetAll().ToList();

        // Assert
        Assert.IsNotNull(pessoas);
        Assert.IsEmpty(pessoas);
    }

}