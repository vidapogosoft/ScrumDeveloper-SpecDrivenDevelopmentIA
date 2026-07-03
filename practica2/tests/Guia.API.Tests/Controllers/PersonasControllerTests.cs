using System.Text.Json;
using Guia.API.Controllers;
using Guia.API.Models;
using Guia.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Guia.API.Tests.Controllers;

public class PersonasControllerTests
{
    [Fact]
    public async Task RefreshData_CuandoNoExisteUsuario_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);

        var result = await controller.RefreshData(321);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Login_CredencialesInvalidas_RetornaUnauthorized()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);

        var result = await controller.Login(new PersonasController.LoginRequest
        {
            Username = "x",
            Password = "y"
        });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ModelStateInvalido_RetornaBadRequest()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);
        controller.ModelState.AddModelError("Username", "invalido");

        var result = await controller.Login(new PersonasController.LoginRequest
        {
            Username = "bad-user",
            Password = "123"
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetPersona_CuandoNoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);

        var result = await controller.GetPersona(77);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task RegistrarPersona_ConCamposFaltantes_RetornaBadRequest()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);
        var body = JsonDocument.Parse("""
            {"nombres":"Ana"}
            """).RootElement.Clone();

        var result = await controller.RegistrarPersona(body);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RegistrarPersona_Valido_RetornaOkYPersiste()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);
        var body = JsonDocument.Parse("""
            {
                "nombres":"Ana",
                "apellidos":"Perez",
                "email":"ana@test.com",
                "username":"ana123",
                "password":"clave123",
                "fechaNacimiento":"1991-03-10"
            }
            """).RootElement.Clone();

        var result = await controller.RegistrarPersona(body);

        Assert.IsType<OkObjectResult>(result);
        Assert.Single(context.Personas);
        Assert.Single(context.ArbolesVida);
    }

    [Fact]
    public async Task GetSignificado_TemaNoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);

        var result = await controller.GetSignificado("inexistente", 1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task RecalcularTodo_CuandoHayRegistros_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.Personas.Add(new Persona
        {
            Nombres = "Luz",
            Apellidos = "Yanez",
            Email = "luz@test.com",
            Username = "luz9",
            Password = "abc123",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Numerologia = new PersonaNumerologia
            {
                NumeroAlma = 1,
                NumeroPersonalidad = 1,
                NumeroDestino = 1
            }
        });
        await context.SaveChangesAsync();
        var controller = new PersonasController(context);

        var result = await controller.RecalcularTodo();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ActualizarPersonaPut_IdNoCoincide_RetornaBadRequest()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);

        var result = await controller.ActualizarPersona(1, new Persona { Id = 2 });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_Valido_RetornaPerfil()
    {
        await using var context = TestDbContextFactory.Create();
        context.FrasesGratitud.Add(new FraseGratitud { Texto = "Gracias", Categoria = "AMOR" });
        context.Personas.Add(new Persona
        {
            Nombres = "Ana",
            Apellidos = "Perez",
            Username = "ana",
            Password = "1234",
            Email = "ana@ok.com",
            FechaNacimiento = new DateTime(1990, 5, 1),
            Detalle = new PersonaDetalle { SignoZodiaco = "Tauro", FaseLunar = "Llena", Elemento = "Tierra" },
            Numerologia = new PersonaNumerologia { MisionVida = 1, NumeroAlma = 2, NumeroDestino = 3, NumeroPersonalidad = 4 }
        });
        await context.SaveChangesAsync();
        var controller = new PersonasController(context);

        var result = await controller.Login(new PersonasController.LoginRequest
        {
            Username = "ana",
            Password = "1234"
        });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task RefreshData_UsuarioExistente_RetornaPerfil()
    {
        await using var context = TestDbContextFactory.Create();
        var persona = new Persona
        {
            Nombres = "Luis",
            Apellidos = "Reyes",
            Username = "luis",
            Password = "abc",
            Email = "luis@ok.com",
            FechaNacimiento = new DateTime(1992, 2, 2),
            Detalle = new PersonaDetalle { SignoZodiaco = "Acuario", FaseLunar = "Nueva", Elemento = "Aire" },
            Numerologia = new PersonaNumerologia { MisionVida = 2, NumeroAlma = 3, NumeroDestino = 4, NumeroPersonalidad = 5 }
        };
        context.FrasesGratitud.Add(new FraseGratitud { Texto = "Agradezco", Categoria = "PROPOSITO" });
        context.Personas.Add(persona);
        await context.SaveChangesAsync();
        var controller = new PersonasController(context);

        var result = await controller.RefreshData(persona.Id);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LoginInicial_Valido_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.FrasesGratitud.Add(new FraseGratitud { Texto = "Gracias vida", Categoria = "PAZ" });
        context.Personas.Add(new Persona
        {
            Nombres = "Mia",
            Apellidos = "Rios",
            Username = "mia",
            Password = "clave",
            Email = "mia@ok.com",
            FechaNacimiento = new DateTime(1994, 11, 11),
            Detalle = new PersonaDetalle { SignoZodiaco = "Escorpio", FaseLunar = "Llena", Elemento = "Agua" },
            Numerologia = new PersonaNumerologia { MisionVida = 9, NumeroAlma = 1, NumeroDestino = 2, NumeroPersonalidad = 3 }
        });
        await context.SaveChangesAsync();
        var controller = new PersonasController(context);

        var result = await controller.LoginInicial(new PersonasController.LoginRequest
        {
            Username = "mia",
            Password = "clave"
        });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ActualizarPersonaJson_CuandoNoExiste_RetornaNotFound()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new PersonasController(context);
        var body = JsonDocument.Parse("""
            {"nombres":"Juan","apellidos":"Lopez","email":"jl@test.com","fechaNacimiento":"1990-01-01"}
            """).RootElement.Clone();

        var result = await controller.ActualizarPersona(999, body);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetSignificado_Encontrado_RetornaOk()
    {
        await using var context = TestDbContextFactory.Create();
        context.Temas.Add(new Tema { Id = 1, Titulo = "Lección de Vida" });
        context.Significados.Add(new Significado
        {
            TemaId = 1,
            ValorNumero = 5,
            Apodo = "Explorador",
            Mision = "Mision",
            Reto = "Reto",
            Mantra = "Mantra",
            Amuleto = "Amuleto",
            MensajeMagico = "Mensaje"
        });
        await context.SaveChangesAsync();
        var controller = new PersonasController(context);

        var result = await controller.GetSignificado("Lección de Vida", 5);

        Assert.IsType<OkObjectResult>(result);
    }
}
