using System.Text.Json;
using Guia.API.Models;
using Guia.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Guia.API.Tests.Controllers;

public class AstroControllerTests
{
    [Fact]
    public async Task GetDetalle_ConNombreVacio_RetornaBadRequest()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new AstroController(context);

        var result = await controller.GetDetalle("signo", "");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetDetalle_SignoExistente_RetornaPayloadEsperado()
    {
        await using var context = TestDbContextFactory.Create();
        context.SignosZodiacales.Add(new SignoZodiacal
        {
            Nombre = "Aries",
            Icono = "♈",
            Elemento = "Fuego",
            DescripcionLarga = "Descripcion Aries",
            PalabrasClave = "Accion"
        });
        await context.SaveChangesAsync();

        var controller = new AstroController(context);
        var result = await controller.GetDetalle("signo", "aries");

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.Equal("Aries", json.RootElement.GetProperty("nombre").GetString());
        Assert.Equal("Descripcion Aries", json.RootElement.GetProperty("descripcion").GetString());
    }

    [Fact]
    public async Task GetDetalle_TipoInvalido_RetornaBadRequest()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new AstroController(context);

        var result = await controller.GetDetalle("x", "algo");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetSigno_CuandoNoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new AstroController(context);

        var result = await controller.GetSigno("Acuario");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetDetalle_LunaExistente_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.FasesLunares.Add(new FaseLunar
        {
            Nombre = "Luna Llena",
            Icono = "🌕",
            SignificadoEspiritual = "Plenitud",
            Recomendacion = "Agradece"
        });
        await context.SaveChangesAsync();
        var controller = new AstroController(context);

        var result = await controller.GetDetalle("luna", "Luna Llena");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetSigno_Existente_RetornaPayloadConPalabrasClave()
    {
        await using var context = TestDbContextFactory.Create();
        context.SignosZodiacales.Add(new SignoZodiacal
        {
            Nombre = "Tauro",
            Icono = "♉",
            Elemento = "Tierra",
            DescripcionLarga = "Persistencia",
            PalabrasClave = "Paciente"
        });
        await context.SaveChangesAsync();
        var controller = new AstroController(context);

        var result = await controller.GetSigno("tauro");

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.Equal("Paciente", json.RootElement.GetProperty("palabrasclave").GetString());
    }
}
