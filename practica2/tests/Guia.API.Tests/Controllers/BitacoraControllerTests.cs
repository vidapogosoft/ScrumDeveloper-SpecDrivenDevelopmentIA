using System.Text.Json;
using Guia.API.Models;
using Guia.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Guia.API.Tests.Controllers;

public class BitacoraControllerTests
{
    [Fact]
    public async Task RegistrarEntrada_GuardaYRetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new BitacoraController(context);
        var entrada = new Bitacora
        {
            PersonaId = 1,
            Fecha = DateTime.UtcNow,
            Tipo = "Sentimiento",
            Contenido = "Me siento bien"
        };

        var result = await controller.RegistrarEntrada(entrada);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.Equal("Entrada guardada en tu bitácora sagrada", json.RootElement.GetProperty("mensaje").GetString());
        Assert.Single(context.Bitacoras);
    }

    [Fact]
    public async Task GetMisRegistros_SinDatos_RetornaListaVacia()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new BitacoraController(context);

        var result = await controller.GetMisRegistros(999);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task FinalizarReto_NoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new BitacoraController(context);

        var result = await controller.FinalizarReto(123);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerRetoActivo_CuandoExiste_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();

        context.RetosSemanales.Add(new RetoSemanal
        {
            Id = 50,
            TemaId = 1,
            Activo = true,
            Titulo = "Reto Paz",
            Descripcion = "Respira",
            Instrucciones = "Hazlo"
        });
        context.Bitacoras.Add(new Bitacora
        {
            PersonaId = 10,
            Fecha = DateTime.UtcNow,
            Tipo = "Reto",
            EstadoReto = "En Curso",
            RetoId = 50,
            Contenido = "Inicio"
        });
        await context.SaveChangesAsync();

        var controller = new BitacoraController(context);
        var result = await controller.ObtenerRetoActivo(10);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.Equal(50, json.RootElement.GetProperty("RetoId").GetInt32());
    }

    [Fact]
    public async Task FinalizarReto_CuandoExiste_ActualizaEstado()
    {
        await using var context = TestDbContextFactory.Create();
        context.Bitacoras.Add(new Bitacora
        {
            Id = 70,
            PersonaId = 1,
            Fecha = DateTime.UtcNow,
            Tipo = "Reto",
            EstadoReto = "En Curso",
            Contenido = "Inicio"
        });
        await context.SaveChangesAsync();
        var controller = new BitacoraController(context);

        var result = await controller.FinalizarReto(70);

        Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Completado", context.Bitacoras.Single(b => b.Id == 70).EstadoReto);
    }

    [Fact]
    public async Task ObtenerHistorial_RetornaElementosOrdenados()
    {
        await using var context = TestDbContextFactory.Create();
        context.Bitacoras.AddRange(
            new Bitacora { PersonaId = 2, Fecha = DateTime.Today.AddDays(-1), Tipo = "Sentimiento", Contenido = "A" },
            new Bitacora { PersonaId = 2, Fecha = DateTime.Today, Tipo = "Sentimiento", Contenido = "B" });
        await context.SaveChangesAsync();
        var controller = new BitacoraController(context);

        var result = await controller.ObtenerHistorial(2);

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
        Assert.Equal(2, list.Count());
    }
}
