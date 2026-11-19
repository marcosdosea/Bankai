using BankaiWeb.Controllers;
using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BankaiWebTests
{
    [TestClass]
    public class GarantiaControllerTest
    {
        private Mock<IGarantiaService> serviceMock = null!;
        private GarantiaController controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            serviceMock = new Mock<IGarantiaService>();
            controller = new GarantiaController(serviceMock.Object);
        }

        [TestMethod]
        public void Index_RetornaViewResultComListaDeGarantias()
        {
            // Arrange
            var listaMock = new List<Chamadogarantium>
            {
                new Chamadogarantium { Id = 1, IdAgendamento = 1, Descricao = "Defeito A", StatusGarantia = "S" },
                new Chamadogarantium { Id = 2, IdAgendamento = 1, Descricao = "Defeito B", StatusGarantia = "A" }
            };
            serviceMock.Setup(s => s.GetAll()).Returns(listaMock);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<GarantiaViewModel>));
            var model = (IEnumerable<GarantiaViewModel>)viewResult.Model!;
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        public void Details_IdExistente_RetornaViewResultComModel()
        {
            // Arrange
            var chamadoMock = new Chamadogarantium { Id = 1, IdAgendamento = 1, Descricao = "Problema no disjuntor", StatusGarantia = "S" };
            serviceMock.Setup(s => s.Get(1u)).Returns(chamadoMock);

            // Act
            var result = controller.Details(1u);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(GarantiaViewModel));
            var model = (GarantiaViewModel)viewResult.Model!;
            Assert.AreEqual("Problema no disjuntor", model.Descricao);
        }

        [TestMethod]
        public void Create_Get_RetornaViewResultComStatusS()
        {
            // Act
            var result = controller.Create();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(GarantiaViewModel));
            var model = (GarantiaViewModel)viewResult.Model!;
            Assert.AreEqual("S", model.StatusGarantia);
        }

        [TestMethod]
        public void Create_PostValido_RedirecionaParaIndex()
        {
            // Arrange
            var viewModel = new GarantiaViewModel
            {
                IdAgendamento = 1,
                Descricao = "Falha no disjuntor principal",
                StatusGarantia = "S"
            };

            // Act
            var result = controller.Create(viewModel);

            // Assert
            serviceMock.Verify(s => s.Create(It.IsAny<Chamadogarantium>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public void Create_PostInvalido_RetornaViewComViewModel()
        {
            // Arrange
            controller.ModelState.AddModelError("Descricao", "A descrição é obrigatória.");
            var viewModel = new GarantiaViewModel { IdAgendamento = 1, Descricao = "" };

            // Act
            var result = controller.Create(viewModel);

            // Assert
            serviceMock.Verify(s => s.Create(It.IsAny<Chamadogarantium>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Edit_PostValido_RedirecionaParaIndex()
        {
            // Arrange
            var viewModel = new GarantiaViewModel
            {
                Id = 1,
                IdAgendamento = 1,
                Descricao = "Descrição atualizada da garantia",
                StatusGarantia = "A"
            };

            // Act
            var result = controller.Edit(viewModel);

            // Assert
            serviceMock.Verify(s => s.Edit(It.IsAny<Chamadogarantium>()), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public void Delete_PostValido_ExecutaDeleteERedirecionaParaIndex()
        {
            // Arrange
            var viewModel = new GarantiaViewModel { Id = 1 };

            // Act
            var result = controller.Delete(viewModel);

            // Assert
            serviceMock.Verify(s => s.Delete(1u), Times.Once);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }
    }
}