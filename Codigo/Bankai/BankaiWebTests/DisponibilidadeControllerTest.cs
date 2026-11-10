using BankaiWeb.Controllers;
using BankaiWeb.Models;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace BankaiWebTests;

[TestClass]
public class DisponibilidadeControllerTest
{
    private Mock<IDisponibilidadeService> serviceMock = null!;
    private DisponibilidadeController controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        this.serviceMock = new Mock<IDisponibilidadeService>();
        this.controller = new DisponibilidadeController(this.serviceMock.Object);
    }

    [TestMethod]
    public void Index_SemIdPessoa_RetornaViewComListaDeViewModels()
    {
        var disponibilidades = new List<Disponibilidade> { CriarDisponibilidade(1) };
        this.serviceMock.Setup(service => service.GetAll()).Returns(disponibilidades);

        var result = this.controller.Index(null);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = (ViewResult)result;
        Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<DisponibilidadeViewModel>));
        var model = (IEnumerable<DisponibilidadeViewModel>)viewResult.Model!;
        Assert.AreEqual(1, model.Count());
        this.serviceMock.Verify(service => service.GetAll(), Times.Once);
    }

    [TestMethod]
    public void Index_ComIdPessoa_BuscaDisponibilidadesDoProfissional()
    {
        this.serviceMock
            .Setup(service => service.GetByIdPessoa(7))
            .Returns(new List<Disponibilidade> { CriarDisponibilidade(1, 7) });

        var result = this.controller.Index(7);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        this.serviceMock.Verify(service => service.GetByIdPessoa(7), Times.Once);
        this.serviceMock.Verify(service => service.GetAll(), Times.Never);
    }

    [TestMethod]
    public void Details_IdExistente_RetornaViewComDisponibilidade()
    {
        this.serviceMock.Setup(service => service.Get(3)).Returns(CriarDisponibilidade(3));

        var result = this.controller.Details(3);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var model = (DisponibilidadeViewModel)((ViewResult)result).Model!;
        Assert.AreEqual((uint)3, model.Id);
        Assert.AreEqual("D", model.StatusDisponibilidade);
        this.serviceMock.Verify(service => service.Get(3), Times.Once);
    }

    [TestMethod]
    public void Details_IdInexistente_RetornaNotFound()
    {
        this.serviceMock.Setup(service => service.Get(99)).Returns((Disponibilidade?)null);

        var result = this.controller.Details(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        this.serviceMock.Verify(service => service.Get(99), Times.Once);
    }

    [TestMethod]
    public void Create_Get_RetornaViewComNovoViewModel()
    {
        var result = this.controller.Create();

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        Assert.IsInstanceOfType(((ViewResult)result).Model, typeof(DisponibilidadeViewModel));
    }

    [TestMethod]
    public void Create_PostComModelInvalido_RetornaViewSemCriar()
    {
        var model = CriarViewModel();
        this.controller.ModelState.AddModelError(nameof(model.StatusDisponibilidade), "Status inválido.");

        var result = this.controller.Create(model);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        Assert.AreSame(model, ((ViewResult)result).Model);
        this.serviceMock.Verify(service => service.Create(It.IsAny<Disponibilidade>()), Times.Never);
    }

    [TestMethod]
    public void Create_PostComModelValido_CriaERedirecionaParaIndex()
    {
        Disponibilidade? disponibilidadeCriada = null;
        this.serviceMock
            .Setup(service => service.Create(It.IsAny<Disponibilidade>()))
            .Callback<Disponibilidade>(disponibilidade => disponibilidadeCriada = disponibilidade)
            .Returns(1);

        var result = this.controller.Create(CriarViewModel());

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        Assert.AreEqual(nameof(DisponibilidadeController.Index), ((RedirectToActionResult)result).ActionName);
        Assert.IsNotNull(disponibilidadeCriada);
        Assert.AreEqual("D", disponibilidadeCriada.StatusDisponibilidade);
        Assert.AreEqual(5, disponibilidadeCriada.IdPessoa);
        this.serviceMock.Verify(service => service.Create(It.IsAny<Disponibilidade>()), Times.Once);
    }

    [TestMethod]
    public void Edit_Get_IdExistente_RetornaViewComDisponibilidade()
    {
        this.serviceMock.Setup(service => service.Get(4)).Returns(CriarDisponibilidade(4));

        var result = this.controller.Edit(4);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var model = (DisponibilidadeViewModel)((ViewResult)result).Model!;
        Assert.AreEqual((uint)4, model.Id);
    }

    [TestMethod]
    public void Edit_Get_IdInexistente_RetornaNotFound()
    {
        this.serviceMock.Setup(service => service.Get(99)).Returns((Disponibilidade?)null);

        var result = this.controller.Edit(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void Edit_PostComIdsDivergentes_RetornaBadRequestSemEditar()
    {
        var result = this.controller.Edit(1, CriarViewModel(2));

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        this.serviceMock.Verify(service => service.Edit(It.IsAny<Disponibilidade>()), Times.Never);
        this.serviceMock.Verify(service => service.Get(It.IsAny<uint>()), Times.Never);
    }

    [TestMethod]
    public void Edit_PostComModelInvalido_RetornaViewSemEditar()
    {
        var model = CriarViewModel(1);
        this.controller.ModelState.AddModelError(nameof(model.HorarioFim), "Horário inválido.");

        var result = this.controller.Edit(1, model);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        Assert.AreSame(model, ((ViewResult)result).Model);
        this.serviceMock.Verify(service => service.Edit(It.IsAny<Disponibilidade>()), Times.Never);
    }

    [TestMethod]
    public void Edit_Post_IdInexistente_RetornaNotFound()
    {
        this.serviceMock.Setup(service => service.Get(1)).Returns((Disponibilidade?)null);

        var result = this.controller.Edit(1, CriarViewModel(1));

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        this.serviceMock.Verify(service => service.Edit(It.IsAny<Disponibilidade>()), Times.Never);
    }

    [TestMethod]
    public void Edit_PostComModelValido_AtualizaERedirecionaParaIndex()
    {
        this.serviceMock.Setup(service => service.Get(1)).Returns(CriarDisponibilidade(1));

        var result = this.controller.Edit(1, CriarViewModel(1));

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        Assert.AreEqual(nameof(DisponibilidadeController.Index), ((RedirectToActionResult)result).ActionName);
        this.serviceMock.Verify(service => service.Edit(It.Is<Disponibilidade>(disponibilidade => disponibilidade.Id == 1)), Times.Once);
    }

    [TestMethod]
    public void Delete_Get_IdExistente_RetornaViewComDisponibilidade()
    {
        this.serviceMock.Setup(service => service.Get(2)).Returns(CriarDisponibilidade(2));

        var result = this.controller.Delete(2);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var model = (DisponibilidadeViewModel)((ViewResult)result).Model!;
        Assert.AreEqual((uint)2, model.Id);
    }

    [TestMethod]
    public void Delete_Get_IdInexistente_RetornaNotFound()
    {
        this.serviceMock.Setup(service => service.Get(99)).Returns((Disponibilidade?)null);

        var result = this.controller.Delete(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void DeleteConfirmed_IdInexistente_RetornaNotFoundSemExcluir()
    {
        this.serviceMock.Setup(service => service.Get(99)).Returns((Disponibilidade?)null);

        var result = this.controller.DeleteConfirmed(99);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        this.serviceMock.Verify(service => service.Delete(It.IsAny<uint>()), Times.Never);
    }

    [TestMethod]
    public void DeleteConfirmed_IdExistente_ExcluiERedirecionaParaIndex()
    {
        this.serviceMock.Setup(service => service.Get(2)).Returns(CriarDisponibilidade(2));

        var result = this.controller.DeleteConfirmed(2);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        Assert.AreEqual(nameof(DisponibilidadeController.Index), ((RedirectToActionResult)result).ActionName);
        this.serviceMock.Verify(service => service.Delete(2), Times.Once);
    }

    private static Disponibilidade CriarDisponibilidade(int id, int idPessoa = 5)
    {
        return new Disponibilidade
        {
            Id = id,
            IdPessoa = idPessoa,
            DiaSemana = "D",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(17, 0, 0),
            StatusDisponibilidade = "D"
        };
    }

    private static DisponibilidadeViewModel CriarViewModel(uint id = 0)
    {
        return new DisponibilidadeViewModel
        {
            Id = id,
            IdPessoa = 5,
            DiaSemana = "D",
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(17, 0, 0),
            StatusDisponibilidade = "D"
        };
    }
}
