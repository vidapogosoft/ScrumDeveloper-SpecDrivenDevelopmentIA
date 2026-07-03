using System.Text.Json;
using Guia.API.Models;
using Guia.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Guia.API.Tests.Controllers;

public class SignificadosControllerTests
{
    [Fact]
    public async Task GetDetalle_CuandoExiste_RetornaObjetoPlano()
    {
        await using var context = TestDbContextFactory.Create();
        var tema = new Tema { Id = 1, Titulo = "Lección de Vida", DescripcionGeneral = "Descripcion tema" };
        context.Temas.Add(tema);
        context.Significados.Add(new Significado
        {
            TemaId = 1,
            ValorNumero = 8,
            Apodo = "El Visionario",
            Mision = "Mision",
            Reto = "Reto",
            Mantra = "Mantra",
            Amuleto = "Amuleto",
            MensajeMagico = "Mensaje"
        });
        await context.SaveChangesAsync();

        var controller = new SignificadosController(context);
        var result = await controller.GetDetalle(1, 8);

        var ok = Assert.IsType<OkObjectResult>(result);
        var json = JsonDocument.Parse(JsonSerializer.Serialize(ok.Value));
        Assert.Equal("El Visionario", json.RootElement.GetProperty("apodo").GetString());
        Assert.Equal("Descripcion tema", json.RootElement.GetProperty("descripcionTema").GetString());
    }

    [Fact]
    public async Task GetDetalle_NoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new SignificadosController(context);

        var result = await controller.GetDetalle(3, 9);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
