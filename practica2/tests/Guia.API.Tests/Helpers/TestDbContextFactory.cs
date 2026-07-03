using Guia.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Guia.API.Tests.Helpers;

internal static class TestDbContextFactory
{
    internal static ApplicationDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString("N"))
            .EnableSensitiveDataLogging()
            .Options;

        return new ApplicationDbContext(options);
    }
}
