using Guia.API.Data;
using Guia.API.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Guia.API.Tests.Controllers;

public class BitacoraControllerMoqTests
{
    [Fact]
    public async Task RegistrarEntrada_UsaDbContextAddYSaveChanges()
    {
        var mockSet = new Mock<DbSet<Bitacora>>();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
        var mockContext = new Mock<ApplicationDbContext>(options);

        mockContext.Setup(c => c.Bitacoras).Returns(mockSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var controller = new BitacoraController(mockContext.Object);

        await controller.RegistrarEntrada(new Bitacora
        {
            PersonaId = 1,
            Fecha = DateTime.UtcNow,
            Tipo = "Reto",
            Contenido = "Contenido"
        });

        mockSet.Verify(s => s.Add(It.IsAny<Bitacora>()), Times.Once);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
