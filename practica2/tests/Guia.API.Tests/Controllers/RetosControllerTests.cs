using System.Text.Json;
using Guia.API.Models;
using Guia.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Guia.API.Tests.Controllers;

public class RetosControllerTests
{
    [Fact]
    public async Task VerificarRetoActivo_CuandoNoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new RetosController(context);

        var result = await controller.VerificarRetoActivo(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerEstadoReto_ConRetoActivo_RetornaTieneRetosTrue()
    {
        await using var context = TestDbContextFactory.Create();
        context.RetosSemanales.Add(new RetoSemanal
        {
            Id = 7,
            TemaId = 1,
            Activo = true,
            Titulo = "Reto",
            Descripcion = "Desc",
            Instrucciones = "Inst"
        });
        context.Bitacoras.Add(new Bitacora
        {
            PersonaId = 44,
            Fecha = DateTime.Today,
            Tipo = "Reto",
            EstadoReto = "En Curso",
            RetoId = 7
        });
        await context.SaveChangesAsync();

        var controller = new RetosController(context);
        var result = await controller.ObtenerEstadoReto(44);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.True(json.RootElement.GetProperty("tieneRetos").GetBoolean());
        Assert.Equal("En Curso", json.RootElement.GetProperty("estado").GetString());
    }

    [Fact]
    public async Task ObtenerEstadoReto_SinSentimiento_RetornaRequiereEscritura()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new RetosController(context);

        var result = await controller.ObtenerEstadoReto(90);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.False(json.RootElement.GetProperty("tieneRetos").GetBoolean());
        Assert.True(json.RootElement.GetProperty("requiereEscritura").GetBoolean());
    }

    [Fact]
    public async Task ObtenerEstadoReto_ConSentimientoYSugerencia_RetornaSugerido()
    {
        await using var context = TestDbContextFactory.Create();
        context.Bitacoras.Add(new Bitacora
        {
            PersonaId = 5,
            Fecha = DateTime.Now,
            Tipo = "Sentimiento",
            Contenido = "me siento triste hoy"
        });
        context.RetosSemanales.Add(new RetoSemanal
        {
            TemaId = 1,
            Activo = true,
            Titulo = "Reto Gratitud",
            Descripcion = "Haz una lista de gratitud",
            Instrucciones = "Escribe 3 cosas"
        });
        await context.SaveChangesAsync();

        var controller = new RetosController(context);
        var result = await controller.ObtenerEstadoReto(5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.True(json.RootElement.GetProperty("tieneRetos").GetBoolean());
        Assert.Equal("Sugerido", json.RootElement.GetProperty("estado").GetString());
    }

    [Fact]
    public async Task VerificarRetoActivo_CuandoExiste_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.RetosSemanales.Add(new RetoSemanal
        {
            Id = 2,
            TemaId = 1,
            Activo = true,
            Titulo = "Reto Escudo",
            Descripcion = "Respira",
            Instrucciones = "Anota"
        });
        context.Bitacoras.Add(new Bitacora
        {
            PersonaId = 88,
            Fecha = DateTime.Now,
            Tipo = "Reto",
            EstadoReto = "En Curso",
            RetoId = 2,
            Reto = context.RetosSemanales.Local.First()
        });
        await context.SaveChangesAsync();
        var controller = new RetosController(context);

        var result = await controller.VerificarRetoActivo(88);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerEstadoReto_CuandoLlegoLimite_RetornaSinRetos()
    {
        await using var context = TestDbContextFactory.Create();
        context.Bitacoras.Add(new Bitacora
        {
            PersonaId = 20,
            Fecha = DateTime.Today,
            Tipo = "Reto",
            EstadoReto = "Completado",
            Contenido = "Completo"
        });
        await context.SaveChangesAsync();
        var controller = new RetosController(context);

        var result = await controller.ObtenerEstadoReto(20);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.False(json.RootElement.GetProperty("tieneRetos").GetBoolean());
    }
}
