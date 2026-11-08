using BankaiWeb.Controllers;
using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace BankaiWebTests;

[TestClass]
public class EspecialidadeControllerTest
{
    private Mock<IEspecialidadeService> serviceMock = null!;
    private EspecialidadeController controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        serviceMock = new Mock<IEspecialidadeService>();
        controller = new EspecialidadeController(serviceMock.Object);
    }

    [TestMethod]
    public void Index_AoInvocar_RetornaViewResultComListaViewModels()
    {
        // Arrange
        var lista = new List<Especialidade>
        {
            new Especialidade { Id = 1, Nome = "Carpinteiro" },
            new Especialidade {Id = 2, Nome = "Eletricista"}
        };
        serviceMock.Setup(s => s.GetAll()).Returns(lista);

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = (ViewResult)result;
        Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<EspecialidadeViewModel>));

        var model = (IEnumerable<EspecialidadeViewModel>)viewResult.Model!;
        Assert.AreEqual(2, model.Count());
    }

    [TestMethod]
    public void Create_Post_ComModelStateValido_RedirecionaParaIndex()
    {
        // Arrange
        var viewModel = new EspecialidadeViewModel { Nome = "Encanador" };
        serviceMock.Setup(s => s.Create(It.IsAny<Especialidade>())).Returns(1u);

        // Act
        var result = controller.Create(viewModel);

        // Assert
        serviceMock.Verify(s => s.Create(It.IsAny<Especialidade>()), Times.Once);
        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

        var redirectResult = (RedirectToActionResult)result;
        Assert.AreEqual(nameof(EspecialidadeController.Index), redirectResult.ActionName);
    }

    [TestMethod]
    public void Edit_Get_IdExistente_RetornaViewComViewModel()
    {
        // Arrage 
        var especialidade = new Especialidade { Id = 1, Nome = "Pintor" };
        serviceMock.Setup(s => s.Get(1u)).Returns(especialidade);

        // Act
        var result = controller.Edit(1u);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = (ViewResult)result;
        var model = (EspecialidadeViewModel)viewResult.Model!;
        Assert.AreEqual("Pintor", model.Nome);
    }
}
