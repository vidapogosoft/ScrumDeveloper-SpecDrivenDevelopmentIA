using System.Text.Json;
using Guia.API.Controllers;
using Guia.API.Models;
using Guia.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Guia.API.Tests.Controllers;

public class NumerologiaControllerTests
{
    [Fact]
    public async Task GetSignificado_SiNoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new NumerologiaController(context);

        var result = await controller.GetSignificado(1, 1);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetTemas_SoloRetornaActivos()
    {
        await using var context = TestDbContextFactory.Create();
        context.Temas.AddRange(
            new Tema { Titulo = "A", EstaActivo = true },
            new Tema { Titulo = "B", EstaActivo = false });
        await context.SaveChangesAsync();
        var controller = new NumerologiaController(context);

        var result = await controller.GetTemas();
        var temas = Assert.IsAssignableFrom<IEnumerable<Tema>>(result.Value);

        Assert.Single(temas);
    }

    [Fact]
    public async Task CalcularArbolDeLaVida_RequestNulo_RetornaBadRequest()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new NumerologiaController(context);

        var result = await controller.CalcularArbolDeLaVida(null!);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CalcularArbolDeLaVida_RequestValido_RetornaOkYPersiste()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new NumerologiaController(context);

        var request = new ArbolVidaRequest
        {
            Persona = new Persona { Id = 1, FechaNacimiento = new DateTime(1990, 5, 10) },
            Datos = new DatosNumerologiaDto
            {
                MisionVida = 6,
                NumeroAlma = 3,
                NumeroPersonalidad = 4,
                NumeroDestino = 7,
                RegaloDivino = 8
            }
        };

        var result = await controller.CalcularArbolDeLaVida(request);

        Assert.IsType<OkObjectResult>(result);
        Assert.Single(context.ArbolesVida);
    }

    [Fact]
    public async Task ObtenerDetalle_SenderoExistente_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.Arcanos.Add(new Arcano
        {
            Numero = 5,
            Nombre = "Hierofante",
            LetraHebrea = "Vav"
        });
        await context.SaveChangesAsync();

        var controller = new NumerologiaController(context);
        var result = await controller.ObtenerDetalle("sendero", 5);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.True(json.RootElement.GetProperty("exito").GetBoolean());
    }

    [Fact]
    public async Task ObtenerRegaloPorPersona_CuandoNoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new NumerologiaController(context);

        var result = await controller.ObtenerRegaloPorPersona(456);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetSignificado_Existente_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.Significados.Add(new Significado
        {
            TemaId = 1,
            ValorNumero = 3,
            Apodo = "Comunicador"
        });
        await context.SaveChangesAsync();
        var controller = new NumerologiaController(context);

        var result = await controller.GetSignificado(1, 3);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task ObtenerArbol_Existente_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.ArbolesVida.Add(new ArbolVida { PersonaId = 33, Kether_Valor = 5 });
        await context.SaveChangesAsync();
        var controller = new NumerologiaController(context);

        var result = await controller.ObtenerArbol(33);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerDetalle_SefiraExistente_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.Arquetipos.Add(new Arquetipo
        {
            Numero = 11,
            Nombre = "Fuerza",
            LetraHebrea = "Kaf"
        });
        await context.SaveChangesAsync();
        var controller = new NumerologiaController(context);

        var result = await controller.ObtenerDetalle("sefira", 11, "Kether");

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerRegaloPorPersona_CuandoExiste_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.ArbolesVida.Add(new ArbolVida { PersonaId = 99, Kether_Valor = 14 });
        await context.SaveChangesAsync();
        var controller = new NumerologiaController(context);

        var result = await controller.ObtenerRegaloPorPersona(99);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.Equal(14, json.RootElement.GetProperty("valor").GetInt32());
    }
}
