using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service.Tests;

[TestClass]
public class ServicoServiceTests
{
    private BankaiContext contexto = null!;
    private IServicoService servicoService = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"Bankai_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<BankaiContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new BankaiContext(opcoes);

        contexto.Servicos.AddRange(
            new Servico
            {
                Id = 1,
                Nome = "Instalação Elétrica Residencial",
                Descricao = "Instalação de tomadas, interruptores e fiação"
            },
            new Servico
            {
                Id = 2,
                Nome = "Reparo Hidráulico",
                Descricao = "Troca de torneiras, reparo de vazamentos e canos"
            },
            new Servico
            {
                Id = 3,
                Nome = "Pintura de Parede",
                Descricao = "Pintura interna e externa com preparação de superfície"
            });

        contexto.SaveChanges();

        servicoService = new ServicoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDoServicoCriado()
    {
        // Arrange
        var servico = new Servico
        {
            Id = 4,
            Nome = "Manutenção de Ar Condicionado",
            Descricao = "Limpeza de filtros e verificação de gás refrigerante"
        };

        // Act
        var idRetornado = servicoService.Create(servico);

        // Assert
        Assert.AreEqual(4, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarServico()
    {
        // Arrange
        var servicoEsperado = new Servico
        {
            Id = 4,
            Nome = "Manutenção de Ar Condicionado",
            Descricao = "Limpeza de filtros e verificação de gás refrigerante"
        };

        // Act
        servicoService.Create(servicoEsperado);

        // Assert
        var servicoCriado = servicoService.Get(4);

        Assert.IsNotNull(servicoCriado);
        Assert.AreEqual(servicoEsperado.Id, servicoCriado.Id);
        Assert.AreEqual(servicoEsperado.Nome, servicoCriado.Nome);
        Assert.AreEqual(servicoEsperado.Descricao, servicoCriado.Descricao);
    }

    [TestMethod]
    public void Delete_QuandoServicoExiste_DeveRemoverServico()
    {
        // Act
        servicoService.Delete(2);

        // Assert
        var servicoExcluido = servicoService.Get(2);
        var servicosRestantes = servicoService.GetAll().ToList();

        Assert.IsNull(servicoExcluido);
        Assert.AreEqual(2, servicosRestantes.Count);
        Assert.IsFalse(servicosRestantes.Any(s => s.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoServicoNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var servicosIniciais = servicoService.GetAll().ToList();

        // Act
        servicoService.Delete(999);

        // Assert
        var servicosFinais = servicoService.GetAll().ToList();

        Assert.AreEqual(servicosIniciais.Count, servicosFinais.Count);
        CollectionAssert.AreEquivalent(
            servicosIniciais.Select(s => s.Id).ToList(),
            servicosFinais.Select(s => s.Id).ToList());
    }

    [TestMethod]
    public void Edit_QuandoServicoExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var servico = servicoService.Get(3);

        Assert.IsNotNull(servico);

        servico!.Nome = "Pintura Decorativa e Textura";
        servico.Descricao = "Aplicação de textura grafiato e pintura acrílica";

        // Act
        servicoService.Edit(servico);

        // Assert
        var servicoAtualizado = servicoService.Get(3);

        Assert.IsNotNull(servicoAtualizado);
        Assert.AreEqual("Pintura Decorativa e Textura", servicoAtualizado.Nome);
        Assert.AreEqual("Aplicação de textura grafiato e pintura acrílica", servicoAtualizado.Descricao);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarServicoCorrespondente()
    {
        // Act
        var servico = servicoService.Get(1);

        // Assert
        Assert.IsNotNull(servico);
        Assert.AreEqual(1, servico.Id);
        Assert.AreEqual("Instalação Elétrica Residencial", servico.Nome);
        Assert.AreEqual("Instalação de tomadas, interruptores e fiação", servico.Descricao);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var servico = servicoService.Get(999);

        // Assert
        Assert.IsNull(servico);
    }

    [TestMethod]
    public void GetAll_QuandoExistemServicos_DeveRetornarTodosOsServicos()
    {
        // Act
        var servicos = servicoService.GetAll().ToList();

        // Assert
        Assert.IsNotNull(servicos);
        Assert.AreEqual(3, servicos.Count);

        CollectionAssert.AreEquivalent(
            new int[] { 1, 2, 3 },
            servicos.Select(s => s.Id).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaServicos_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Servicos.RemoveRange(contexto.Servicos);
        contexto.SaveChanges();

        // Act
        var servicos = servicoService.GetAll().ToList();

        // Assert
        Assert.IsNotNull(servicos);
        Assert.AreEqual(0, servicos.Count);
    }

    [TestMethod]
    public void GetByNome_QuandoExisteCorrespondencia_DeveRetornarServicosCorrespondentes()
    {
        // Act
        var servicos = servicoService.GetByNome("Instalação").ToList();

        // Assert
        Assert.AreEqual(1, servicos.Count);
        Assert.AreEqual("Instalação Elétrica Residencial", servicos[0].Nome);
    }

    [TestMethod]
    public void GetByNome_QuandoNomeCorrespondeAoInicio_DeveRetornarTodosOsServicosCorrespondentes()
    {
        // Arrange
        contexto.Servicos.Add(new Servico
        {
            Id = 4,
            Nome = "Instalação de Câmeras de Segurança",
            Descricao = "Instalação de CFTV e configuração remota"
        });

        contexto.SaveChanges();

        // Act
        var servicos = servicoService.GetByNome("Instalação").ToList();

        // Assert
        Assert.AreEqual(2, servicos.Count);
        Assert.IsTrue(servicos.All(s => s.Nome.StartsWith("Instalação")));
    }

    [TestMethod]
    public void GetByNome_QuandoNaoExisteCorrespondencia_DeveRetornarColecaoVazia()
    {
        // Act
        var servicos = servicoService.GetByNome("Marcenaria").ToList();

        // Assert
        Assert.IsNotNull(servicos);
        Assert.AreEqual(0, servicos.Count);
    }

        [TestMethod]
        public void GetByNome_QuandoNomeEstaVazio_DeveRetornarTodosOsServicos()
        {
            // Act
            var servicos = servicoService.GetByNome(string.Empty).ToList();

            // Assert
            Assert.AreEqual(3, servicos.Count);
        }

        [TestMethod]
        public void Get_QuandoServicoPossuiEspecialidade_DeveCarregarNavegacaoEspecialidade()
        {
            // Arrange
            PopularCenarioBusca();

            // Act
            var servico = servicoService.Get(10);

            // Assert
            Assert.IsNotNull(servico);
            Assert.AreEqual(20, servico.IdEspecialidade);
            Assert.IsNotNull(servico.IdEspecialidadeNavigation);
            Assert.AreEqual("Manutenção Elétrica", servico.IdEspecialidadeNavigation.Nome);
        }

        [TestMethod]
        public void ObterEspecialidades_QuandoExistemEspecialidades_DeveRetornarOrdenadasPorNome()
        {
            // Arrange
            contexto.Especialidades.AddRange(
                new Especialidade { Id = 10, Nome = "Hidráulica" },
                new Especialidade { Id = 11, Nome = "Elétrica" });
            contexto.SaveChanges();

            // Act
            var especialidades = servicoService.ObterEspecialidades().ToList();

            // Assert
            Assert.HasCount(2, especialidades);
            Assert.AreEqual("Elétrica", especialidades[0].Nome);
            Assert.AreEqual("Hidráulica", especialidades[1].Nome);
        }

        [TestMethod]
        public void BuscarServicos_QuandoFiltradoPorTermoEEspecialidade_DeveRetornarServicosCorrespondentes()
        {
            // Arrange
            PopularCenarioBusca();

            // Act
            var resultados = servicoService.BuscarServicos("chuveiro", idEspecialidade: 20, dataDesejada: null).ToList();

            // Assert
            Assert.HasCount(1, resultados);
            Assert.AreEqual("Troca de Chuveiro", resultados[0].Nome);
            Assert.AreEqual("Manutenção Elétrica", resultados[0].NomeEspecialidade);
            Assert.AreEqual(2, resultados[0].QuantidadeProfissionais);
            Assert.AreEqual(110.00m, resultados[0].MenorPreco);
        }

        [TestMethod]
        public void BuscarServicos_QuandoFiltradoPorData_DeveContabilizarApenasProfissionaisDisponiveisNaData()
        {
            // Arrange
            PopularCenarioBusca();
            // Próxima segunda-feira ("SF"): Profissional 20 está disponível; Profissional 21 atende apenas terça ("TF").
            DateTime segunda = ProximoDiaDaSemana(DayOfWeek.Monday);

            // Act
            var resultados = servicoService.BuscarServicos("chuveiro", idEspecialidade: 20, dataDesejada: segunda).ToList();

            // Assert
            Assert.HasCount(1, resultados);
            Assert.AreEqual(1, resultados[0].QuantidadeProfissionais);
            Assert.AreEqual(120.00m, resultados[0].MenorPreco);
        }

        [TestMethod]
        public void BuscarProfissionaisPorServico_QuandoFiltradoEOrdenadoPorMenorPreco_DeveRetornarOrdenadoComAvaliacao()
        {
            // Arrange
            PopularCenarioBusca();
            DateTime segunda = ProximoDiaDaSemana(DayOfWeek.Monday);

            // Act
            var profissionais = servicoService.BuscarProfissionaisPorServico(
                idServico: 10,
                dataDesejada: segunda,
                ordenarPor: "menor_preco",
                apenasVerificados: false,
                apenasDisponiveisNaData: false,
                precoMaximo: null).ToList();

            // Assert
            Assert.HasCount(2, profissionais);
            Assert.AreEqual("Carlos Eduardo", profissionais[0].NomeProfissional);
            Assert.AreEqual(110.00m, profissionais[0].Preco);
            Assert.IsFalse(profissionais[0].Verificado);
            Assert.IsFalse(profissionais[0].DisponivelNaData);

            Assert.AreEqual("Marcos Silva", profissionais[1].NomeProfissional);
            Assert.AreEqual(120.00m, profissionais[1].Preco);
            Assert.IsTrue(profissionais[1].Verificado);
            Assert.IsTrue(profissionais[1].DisponivelNaData);
            Assert.AreEqual(5.00m, profissionais[1].NotaMedia);
            Assert.AreEqual(1, profissionais[1].TotalAvaliacoes);
        }

        [TestMethod]
        public void BuscarProfissionaisPorServico_QuandoApenasVerificadosEDisponiveisNaData_DeveFiltrarCorretamente()
        {
            // Arrange
            PopularCenarioBusca();
            DateTime segunda = ProximoDiaDaSemana(DayOfWeek.Monday);

            // Act
            var profissionais = servicoService.BuscarProfissionaisPorServico(
                idServico: 10,
                dataDesejada: segunda,
                ordenarPor: "melhor_avaliado",
                apenasVerificados: true,
                apenasDisponiveisNaData: true,
                precoMaximo: 150.00m).ToList();

            // Assert
            Assert.HasCount(1, profissionais);
            Assert.AreEqual("Marcos Silva", profissionais[0].NomeProfissional);
        }

        private void PopularCenarioBusca()
        {
            var especialidade = new Especialidade { Id = 20, Nome = "Manutenção Elétrica" };
            var servicoChuveiro = new Servico
            {
                Id = 10,
                Nome = "Troca de Chuveiro",
                Descricao = "Substituição de resistência e fiação",
                IdEspecialidade = 20
            };

            var cliente = CriarPessoaTeste(1, "Cliente Teste", ehProfissional: false, verificado: false);
            var marcos = CriarPessoaTeste(20, "Marcos Silva", ehProfissional: true, verificado: true);
            var carlos = CriarPessoaTeste(21, "Carlos Eduardo", ehProfissional: true, verificado: false);

            contexto.Especialidades.Add(especialidade);
            contexto.Servicos.Add(servicoChuveiro);
            contexto.Pessoas.AddRange(cliente, marcos, carlos);
            contexto.Profissionalservicos.AddRange(
                new Profissionalservico { IdPessoa = 20, IdServico = 10, Preco = 120.00m },
                new Profissionalservico { IdPessoa = 21, IdServico = 10, Preco = 110.00m });

            contexto.Disponibilidades.AddRange(
                new Disponibilidade
                {
                    Id = 100,
                    IdPessoa = 20,
                    DiaSemana = "SF",
                    StatusDisponibilidade = "D",
                    HorarioInicio = TimeSpan.FromHours(8),
                    HorarioFim = TimeSpan.FromHours(18)
                },
                new Disponibilidade
                {
                    Id = 101,
                    IdPessoa = 21,
                    DiaSemana = "TF",
                    StatusDisponibilidade = "D",
                    HorarioInicio = TimeSpan.FromHours(8),
                    HorarioFim = TimeSpan.FromHours(18)
                });

            var agendamento = new Agendamento
            {
                Id = 500,
                IdCliente = 1,
                IdProfissional = 20,
                TipoCobranca = "F",
                StatusAgendamento = "Concluido",
                Data = DateTime.Today.AddDays(-2),
                HorarioInicio = DateTime.Today.AddDays(-2).AddHours(9),
                HorarioFim = DateTime.Today.AddDays(-2).AddHours(10)
            };
            contexto.Agendamentos.Add(agendamento);
            contexto.Avaliacaos.Add(new Avaliacao
            {
                Id = 300,
                IdAgendamento = 500,
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 5
            });

            contexto.SaveChanges();
        }

        private static Pessoa CriarPessoaTeste(int id, string nome, bool ehProfissional, bool verificado)
        {
            return new Pessoa
            {
                Id = id,
                Nome = nome,
                CpfCnpj = $"{id:00000000000}",
                Email = $"pessoa{id}@bankai.com",
                Telefone = $"7998888{id:0000}",
                EhProfissional = ehProfissional,
                RegistroTecnico = verificado ? new byte[] { 1, 2, 3 } : null,
                Cep = "49000000",
                Rua = "Rua A",
                Bairro = "Centro",
                Cidade = "Aracaju",
                Estado = "SE",
                NumeroCasa = "10"
            };
        }

        private static DateTime ProximoDiaDaSemana(DayOfWeek diaAlvo)
        {
            var data = DateTime.Today.AddDays(1);
            while (data.DayOfWeek != diaAlvo)
            {
                data = data.AddDays(1);
            }
            return data;
        }
    }

