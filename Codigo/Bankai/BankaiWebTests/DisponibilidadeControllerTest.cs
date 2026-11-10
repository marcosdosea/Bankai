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
    public void Index_SemIdPessoa_UsaProfissionalPadraoERetornaViewComListaDeViewModels()
    {
        var disponibilidades = new List<Disponibilidade> { CriarDisponibilidade(1) };
        this.serviceMock.Setup(service => service.GetByIdPessoa(1)).Returns(disponibilidades);

        var result = this.controller.Index(null);

        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = (ViewResult)result;
        Assert.IsInstanceOfType(viewResult.Model, typeof(IEnumerable<DisponibilidadeViewModel>));
        var model = (IEnumerable<DisponibilidadeViewModel>)viewResult.Model!;
        Assert.AreEqual(1, model.Count());
        Assert.AreEqual((uint)1, this.controller.ViewBag.IdPessoa);
        this.serviceMock.Verify(service => service.GetByIdPessoa(1), Times.Once);
        this.serviceMock.Verify(service => service.GetAll(), Times.Never);
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
    public void SalvarLote_SemanaCompleta_CriaHorariosERedirecionaParaProfissional()
    {
        this.serviceMock.Setup(service => service.Create(It.IsAny<Disponibilidade>())).Returns(1);

        var result = this.controller.SalvarLote(CriarSemana(1), 1);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        var redirect = (RedirectToActionResult)result;
        Assert.AreEqual(nameof(DisponibilidadeController.Index), redirect.ActionName);
        Assert.AreEqual((uint)1, redirect.RouteValues!["idPessoa"]);
        this.serviceMock.Verify(service => service.Create(It.IsAny<Disponibilidade>()), Times.Exactly(7));
        this.serviceMock.Verify(service => service.Edit(It.IsAny<Disponibilidade>()), Times.Never);
    }

    [TestMethod]
    public void SalvarLote_SemanaExistente_AtualizaHorariosSemCriarDuplicatas()
    {
        var semana = CriarSemana(1);
        for (var i = 0; i < semana.Count; i++)
        {
            var id = (uint)(i + 1);
            semana[i].Id = id;
            this.serviceMock.Setup(service => service.Get(id)).Returns(new Disponibilidade
            {
                Id = (int)id,
                IdPessoa = 1,
                DiaSemana = semana[i].DiaSemana,
                HorarioInicio = new TimeSpan(8, 0, 0),
                HorarioFim = new TimeSpan(17, 0, 0),
                StatusDisponibilidade = "ND"
            });
        }

        var result = this.controller.SalvarLote(semana, 1);

        Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        this.serviceMock.Verify(service => service.Edit(It.IsAny<Disponibilidade>()), Times.Exactly(7));
        this.serviceMock.Verify(service => service.Create(It.IsAny<Disponibilidade>()), Times.Never);
    }

    [TestMethod]
    public void SalvarLote_DiasRepetidos_RetornaBadRequestSemGravar()
    {
        var semana = CriarSemana(1);
        semana[6].DiaSemana = semana[5].DiaSemana;

        var result = this.controller.SalvarLote(semana, 1);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        this.serviceMock.Verify(service => service.Create(It.IsAny<Disponibilidade>()), Times.Never);
        this.serviceMock.Verify(service => service.Edit(It.IsAny<Disponibilidade>()), Times.Never);
    }

    [TestMethod]
    public void SalvarLote_ProfissionalDiferenteDoSelecionado_RetornaBadRequestSemGravar()
    {
        var semana = CriarSemana(1);
        semana[0].IdPessoa = 2;

        var result = this.controller.SalvarLote(semana, 1);

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        this.serviceMock.Verify(service => service.Create(It.IsAny<Disponibilidade>()), Times.Never);
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
        var redirect = (RedirectToActionResult)result;
        Assert.AreEqual(nameof(DisponibilidadeController.Index), redirect.ActionName);
        Assert.AreEqual((uint)5, redirect.RouteValues!["idPessoa"]);
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

    private static List<DisponibilidadeViewModel> CriarSemana(uint profissionalId)
    {
        var dias = new[] { "D", "SF", "TF", "QF", "QIF", "SXF", "S" };
        return dias.Select(dia => new DisponibilidadeViewModel
        {
            IdPessoa = profissionalId,
            DiaSemana = dia,
            HorarioInicio = new TimeSpan(8, 0, 0),
            HorarioFim = new TimeSpan(17, 0, 0),
            StatusDisponibilidade = "ND"
        }).ToList();
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
