using BankaiWeb.Controllers;
using BankaiWeb.Models;
using BankaiWeb.Controllers;
using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankaiWEBTests.Controllers
{
    [TestClass]
    public class PagamentoControllerTest
    {
        private Mock<IPagamentoService> _mockService = null!;
        private PagamentoController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IPagamentoService>();
            _controller = new PagamentoController(_mockService.Object);
        }

        [TestMethod]
        public void Index_RetornaViewComListaDePagamentos()
        {
            // Arrange
            var pagamentos = new List<Pagamento>
            {
                new Pagamento { Id = 1, IdAgendamento = 10, FormaPagamento = "Pix", StatusPagamento = "Aprovado", ValorTransacao = 150.00m },
                new Pagamento { Id = 2, IdAgendamento = 11, FormaPagamento = "Cartao de credito", StatusPagamento = "Pendente", ValorTransacao = 250.00m }
            };
            _mockService.Setup(s => s.getAll()).Returns(pagamentos);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as IEnumerable<PagamentoViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        public void RealizarPagamento_Get_ExibeTela008ComValoresIniciais()
        {
            // Act
            var result = _controller.RealizarPagamento(idAgendamento: 5, valor: 180.00m) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as PagamentoViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(5, model.CodigoAgendamento);
            Assert.AreEqual(180.00m, model.ValorTransacao);
            Assert.AreEqual("Pix", model.FormaPagamento);
            Assert.AreEqual("Aguardando Processamento", model.StatusPagamento);
        }

        [TestMethod]
        public void RealizarPagamento_Post_PixValido_RedirecionaParaDetails()
        {
            // Arrange
            var viewModel = new PagamentoViewModel
            {
                CodigoAgendamento = 5,
                FormaPagamento = "Pix",
                ValorTransacao = 180.00m
            };

            var pagamentoAprovado = new Pagamento
            {
                Id = 12,
                IdAgendamento = 5,
                FormaPagamento = "Pix",
                StatusPagamento = "Aprovado",
                ValorTransacao = 180.00m
            };

            _mockService
                .Setup(s => s.RealizarPagamento(It.IsAny<Pagamento>(), true))
                .Returns(pagamentoAprovado);

            // Act
            var result = _controller.RealizarPagamento(viewModel) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual(12, result.RouteValues?["id"]);
        }

        [TestMethod]
        public void RealizarPagamento_Post_DadosEmBranco_RetornaViewComErroCSU06()
        {
            // Arrange
            var viewModelInvalido = new PagamentoViewModel
            {
                CodigoAgendamento = 0,
                FormaPagamento = "Pix",
                ValorTransacao = 0m
            };

            _controller.ModelState.AddModelError("ValorTransacao", "Por favor, preencha todos os campos obrigatórios corretamente.");

            // Act
            var result = _controller.RealizarPagamento(viewModelInvalido) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
            _mockService.Verify(s => s.RealizarPagamento(It.IsAny<Pagamento>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void RealizarPagamento_Post_TransacaoRecusada_RetornaViewComMensagemDeRecusa()
        {
            // Arrange
            var viewModel = new PagamentoViewModel
            {
                CodigoAgendamento = 5,
                FormaPagamento = "Cartao de credito",
                ValorTransacao = 500.00m,
                NumeroCartao = "4111111111111111",
                NomeTitularCartao = "Cliente Teste",
                ValidadeCartao = "12/29",
                CvvCartao = "123"
            };

            _mockService
                .Setup(s => s.RealizarPagamento(It.IsAny<Pagamento>(), true))
                .Throws(new Exception("Pagamento não autorizado pela instituição financeira."));

            // Act
            var result = _controller.RealizarPagamento(viewModel) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
            var erro = _controller.ModelState[string.Empty]?.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.AreEqual("Pagamento não autorizado pela instituição financeira.", erro);
        }
    }
}