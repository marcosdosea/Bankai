using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service;

namespace Service.Tests
{
    [TestClass]
    public class AvaliarServiceTeste
    {
        private BankaiContext contexto = null!;
        private IAvaliarService avaliarService = null!;

        [TestInitialize]
        public void Inicializar()
        {
            var nomeBanco = $"Bankai_Avaliar_{Guid.NewGuid()}";

            var opcoes = new DbContextOptionsBuilder<BankaiContext>()
                .UseInMemoryDatabase(databaseName: nomeBanco)
                .Options;

            contexto = new BankaiContext(opcoes);

            contexto.Pessoas.AddRange(
                CriarPessoa(1, "Carlos Albuquerque", false),
                CriarPessoa(2, "Mariana Costa", false),
                CriarPessoa(3, "Marcos Silva", true),
                CriarPessoa(4, "Ricardo Lima", true)
            );

            contexto.Agendamentos.AddRange(
                new Agendamento
                {
                    Id = 1,
                    IdCliente = 1,
                    IdProfissional = 3,
                    TipoCobranca = "F",
                    StatusAgendamento = "Concluido",
                    Data = DateTime.Today.AddDays(-5),
                    HorarioInicio = DateTime.Today.AddDays(-5).AddHours(9),
                    HorarioFim = DateTime.Today.AddDays(-5).AddHours(11)
                },
                new Agendamento
                {
                    Id = 2,
                    IdCliente = 1,
                    IdProfissional = 3,
                    TipoCobranca = "O",
                    StatusAgendamento = "Concluido",
                    Data = DateTime.Today.AddDays(-2),
                    HorarioInicio = DateTime.Today.AddDays(-2).AddHours(14),
                    HorarioFim = DateTime.Today.AddDays(-2).AddHours(16)
                },
                new Agendamento
                {
                    Id = 3,
                    IdCliente = 1,
                    IdProfissional = 3,
                    TipoCobranca = "F",
                    StatusAgendamento = "Execucao",
                    Data = DateTime.Today,
                    HorarioInicio = DateTime.Today.AddHours(10),
                    HorarioFim = DateTime.Today.AddHours(12)
                },
                new Agendamento
                {
                    Id = 4,
                    IdCliente = 2,
                    IdProfissional = 4,
                    TipoCobranca = "F",
                    StatusAgendamento = "Concluido",
                    Data = DateTime.Today.AddDays(-3),
                    HorarioInicio = DateTime.Today.AddDays(-3).AddHours(8),
                    HorarioFim = DateTime.Today.AddDays(-3).AddHours(10)
                }
            );

            contexto.Avaliacaos.AddRange(
                new Avaliacao
                {
                    Id = 1,
                    IdAgendamento = 1,
                    Pontualidade = 4,
                    LimpezaLocal = 5,
                    QualidadeReparo = 3,
                    Depoimento = "Bom atendimento, serviço bem feito."
                },
                new Avaliacao
                {
                    Id = 2,
                    IdAgendamento = 4,
                    Pontualidade = 5,
                    LimpezaLocal = 5,
                    QualidadeReparo = 5,
                    Depoimento = "Excelente profissional!"
                }
            );

            contexto.SaveChanges();
            contexto.ChangeTracker.Clear();

            avaliarService = new AvaliarService(contexto);
        }

        [TestCleanup]
        public void Finalizar()
        {
            contexto.Database.EnsureDeleted();
            contexto.Dispose();
        }

        [TestMethod]
        public void Create_QuandoDadosSaoValidos_DeveRetornarIdEArmazenarAvaliacao()
        {
            var novaAvaliacao = new Avaliacao
            {
                IdAgendamento = 2,
                Pontualidade = 5,
                LimpezaLocal = 4,
                QualidadeReparo = 5,
                Depoimento = "Muito pontual e cuidadoso."
            };

            var id = avaliarService.Create(novaAvaliacao);

            Assert.IsTrue(id > 0);
            var criada = avaliarService.Get(id);
            Assert.IsNotNull(criada);
            Assert.AreEqual(2, criada.IdAgendamento);
            Assert.AreEqual(5, criada.Pontualidade);
            Assert.AreEqual(4, criada.LimpezaLocal);
            Assert.AreEqual(5, criada.QualidadeReparo);
            Assert.AreEqual("Muito pontual e cuidadoso.", criada.Depoimento);
        }

        [TestMethod]
        public void Create_QuandoNotaForaDoIntervalo_DeveLancarArgumentExceptionComMensagemCSU07()
        {
            var avaliacaoNotaZero = new Avaliacao
            {
                IdAgendamento = 2,
                Pontualidade = 0,
                LimpezaLocal = 5,
                QualidadeReparo = 5
            };

            var avaliacaoNotaAcimaDeCinco = new Avaliacao
            {
                IdAgendamento = 2,
                Pontualidade = 5,
                LimpezaLocal = 6,
                QualidadeReparo = 5
            };

            var exZero = Assert.ThrowsExactly<ArgumentException>(() => avaliarService.Create(avaliacaoNotaZero));
            Assert.AreEqual("Por favor, selecione uma nota para avaliar o serviço.", exZero.Message);

            var exSeis = Assert.ThrowsExactly<ArgumentException>(() => avaliarService.Create(avaliacaoNotaAcimaDeCinco));
            Assert.AreEqual("Por favor, selecione uma nota para avaliar o serviço.", exSeis.Message);
        }

        [TestMethod]
        public void Create_QuandoDepoimentoExcede255Caracteres_DeveLancarArgumentException()
        {
            var avaliacao = new Avaliacao
            {
                IdAgendamento = 2,
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 5,
                Depoimento = new string('a', 256)
            };

            Assert.ThrowsExactly<ArgumentException>(() => avaliarService.Create(avaliacao));
        }

        [TestMethod]
        public void Create_QuandoAgendamentoNaoExiste_DeveLancarKeyNotFoundException()
        {
            var avaliacao = new Avaliacao
            {
                IdAgendamento = 999,
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 5
            };

            Assert.ThrowsExactly<KeyNotFoundException>(() => avaliarService.Create(avaliacao));
        }

        [TestMethod]
        public void Create_QuandoAgendamentoNaoEstaConcluido_DeveLancarInvalidOperationException()
        {
            var avaliacao = new Avaliacao
            {
                IdAgendamento = 3,
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 5
            };

            var ex = Assert.ThrowsExactly<InvalidOperationException>(() => avaliarService.Create(avaliacao));
            Assert.AreEqual("Apenas serviços concluídos podem ser avaliados.", ex.Message);
        }

        [TestMethod]
        public void Create_QuandoAgendamentoJaFoiAvaliado_DeveLancarInvalidOperationException()
        {
            var avaliacaoDuplicada = new Avaliacao
            {
                IdAgendamento = 1,
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 5
            };

            var ex = Assert.ThrowsExactly<InvalidOperationException>(() => avaliarService.Create(avaliacaoDuplicada));
            Assert.AreEqual("Este agendamento já foi avaliado.", ex.Message);
        }

        [TestMethod]
        public void Avaliar_QuandoDadosSaoValidosComDepoimento_DeveRegistrarAvaliacao()
        {
            var resultado = avaliarService.Avaliar(2, 5, 5, 4, "  Serviço excelente!  ");

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Id > 0);
            Assert.AreEqual(2, resultado.IdAgendamento);
            Assert.AreEqual(5, resultado.Pontualidade);
            Assert.AreEqual(5, resultado.LimpezaLocal);
            Assert.AreEqual(4, resultado.QualidadeReparo);
            Assert.AreEqual("Serviço excelente!", resultado.Depoimento);
        }

        [TestMethod]
        public void Avaliar_QuandoDepoimentoOpcionalEstaEmBranco_DeveSalvarComDepoimentoNulo()
        {
            var resultado = avaliarService.Avaliar(2, 4, 4, 4, "   ");

            Assert.IsNotNull(resultado);
            Assert.IsNull(resultado.Depoimento);
        }

        [TestMethod]
        public void Avaliar_QuandoNotaObrigatoriaEmBranco_DeveLancarArgumentExceptionCSU07()
        {
            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                avaliarService.Avaliar(2, 5, 5, 0, "Faltou nota de qualidade"));

            Assert.AreEqual("Por favor, selecione uma nota para avaliar o serviço.", ex.Message);
        }

        [TestMethod]
        public void Edit_QuandoAvaliacaoExiste_DeveAtualizarDados()
        {
            var avaliacao = avaliarService.Get(1);
            Assert.IsNotNull(avaliacao);
            avaliacao!.Pontualidade = 5;
            avaliacao.QualidadeReparo = 5;
            avaliacao.Depoimento = "Depoimento atualizado.";

            avaliarService.Edit(avaliacao);

            var atualizada = avaliarService.Get(1);
            Assert.IsNotNull(atualizada);
            Assert.AreEqual(5, atualizada.Pontualidade);
            Assert.AreEqual(5, atualizada.QualidadeReparo);
            Assert.AreEqual("Depoimento atualizado.", atualizada.Depoimento);
        }

        [TestMethod]
        public void Edit_QuandoOutraInstanciaComMesmoId_DeveAtualizarAvaliacaoRastreada()
        {
            var rastreada = contexto.Avaliacaos.First(a => a.Id == 1);
            var edicao = new Avaliacao
            {
                Id = 1,
                IdAgendamento = 1,
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 4,
                Depoimento = "Editado com nova instância"
            };

            avaliarService.Edit(edicao);

            Assert.AreEqual(5, rastreada.Pontualidade);
            Assert.AreEqual("Editado com nova instância", avaliarService.Get(1)!.Depoimento);
        }

        [TestMethod]
        public void Edit_QuandoAvaliacaoNaoExiste_DeveLancarKeyNotFoundException()
        {
            var inexistente = new Avaliacao
            {
                Id = 999,
                IdAgendamento = 2,
                Pontualidade = 5,
                LimpezaLocal = 5,
                QualidadeReparo = 5
            };

            Assert.ThrowsExactly<KeyNotFoundException>(() => avaliarService.Edit(inexistente));
        }

        [TestMethod]
        public void Delete_QuandoAvaliacaoExiste_DeveRemoverAvaliacao()
        {
            avaliarService.Delete(1);

            Assert.IsNull(avaliarService.Get(1));
            Assert.AreEqual(1, avaliarService.GetAll().Count());
        }

        [TestMethod]
        public void Delete_QuandoAvaliacaoNaoExiste_NaoDeveAlterarOsDados()
        {
            var totalInicial = avaliarService.GetAll().Count();

            avaliarService.Delete(999);

            Assert.AreEqual(totalInicial, avaliarService.GetAll().Count());
        }

        [TestMethod]
        public void Get_QuandoIdExiste_DeveRetornarAvaliacaoCorrespondente()
        {
            var avaliacao = avaliarService.Get(1);

            Assert.IsNotNull(avaliacao);
            Assert.AreEqual(1, avaliacao.Id);
            Assert.AreEqual(1, avaliacao.IdAgendamento);
        }

        [TestMethod]
        public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
        {
            var avaliacao = avaliarService.Get(999);

            Assert.IsNull(avaliacao);
        }

        [TestMethod]
        public void GetAll_QuandoExistemAvaliacoes_DeveRetornarTodas()
        {
            var avaliacoes = avaliarService.GetAll().ToList();

            Assert.HasCount(2, avaliacoes);
        }

        [TestMethod]
        public void GetByAgendamento_QuandoExiste_DeveRetornarAvaliacao()
        {
            var avaliacao = avaliarService.GetByAgendamento(1);

            Assert.IsNotNull(avaliacao);
            Assert.AreEqual(1, avaliacao.Id);
        }

        [TestMethod]
        public void GetByAgendamento_QuandoNaoExiste_DeveRetornarNulo()
        {
            var avaliacao = avaliarService.GetByAgendamento(2);

            Assert.IsNull(avaliacao);
        }

        [TestMethod]
        public void GetByProfissional_QuandoExistemAvaliacoes_DeveRetornarOrdenadasPorMaisRecentes()
        {
            avaliarService.Avaliar(2, 5, 5, 5, "Segunda avaliação");

            var avaliacoes = avaliarService.GetByProfissional(3).ToList();

            Assert.HasCount(2, avaliacoes);
            Assert.AreEqual(2, avaliacoes[0].IdAgendamento);
            Assert.AreEqual(1, avaliacoes[1].IdAgendamento);
        }

        [TestMethod]
        public void GetByCliente_QuandoExistemAvaliacoes_DeveRetornarApenasDoCliente()
        {
            var avaliacoes = avaliarService.GetByCliente(1).ToList();

            Assert.HasCount(1, avaliacoes);
            Assert.AreEqual(1, avaliacoes[0].IdAgendamento);
        }

        [TestMethod]
        public void GetPendentesPorCliente_DeveRetornarApenasConcluidosAindaNaoAvaliados()
        {
            var pendentes = avaliarService.GetPendentesPorCliente(1).ToList();

            Assert.HasCount(1, pendentes);
            Assert.AreEqual(2, pendentes[0].Id);
        }

        [TestMethod]
        public void CalcularMediaNota_QuandoProfissionalPossuiAvaliacoes_DeveCalcularMediaCorretamente()
        {
            avaliarService.Avaliar(2, 2, 4, 3, "Atrasou um pouco.");

            var media = avaliarService.CalcularMediaNota(3);

            Assert.AreEqual(3.5, media);
        }

        [TestMethod]
        public void CalcularMediaNota_QuandoProfissionalNaoPossuiAvaliacoes_DeveRetornarZero()
        {
            var media = avaliarService.CalcularMediaNota(999);

            Assert.AreEqual(0, media);
        }

        [TestMethod]
        public void CalcularBankaiRating_QuandoNotaMaximaESemGarantia_DeveRetornarCinco()
        {
            var rating = avaliarService.CalcularBankaiRating(4);

            Assert.AreEqual(5.0, rating);
        }

        [TestMethod]
        public void CalcularBankaiRating_QuandoHaAcionamentoDeGarantia_DeveAplicarPesosRN05()
        {
            avaliarService.Avaliar(2, 2, 4, 3);
            contexto.Chamadogarantia.Add(new Chamadogarantium
            {
                Id = 1,
                IdAgendamento = 1,
                Descricao = "Problema recorrente após reparo",
                DataChamado = DateTime.Today,
                StatusGarantia = "S"
            });
            contexto.SaveChanges();

            var rating = avaliarService.CalcularBankaiRating(3);

            Assert.AreEqual(3.1, rating);
        }

        [TestMethod]
        public void CalcularBankaiRating_QuandoProfissionalNaoPossuiAvaliacoes_DeveRetornarZero()
        {
            var rating = avaliarService.CalcularBankaiRating(999);

            Assert.AreEqual(0, rating);
        }

        private static Pessoa CriarPessoa(int id, string nome, bool ehProfissional)
        {
            return new Pessoa
            {
                Id = id,
                Nome = nome,
                CpfCnpj = $"0000000000{id}",
                Email = $"pessoa{id}@bankai.com",
                Telefone = $"7999999000{id}",
                EhProfissional = ehProfissional,
                Cep = "49000000",
                Rua = "Rua Teste",
                Bairro = "Centro",
                Cidade = "Aracaju",
                Estado = "SE",
                NumeroCasa = $"{id}00"
            };
        }
    }
}

