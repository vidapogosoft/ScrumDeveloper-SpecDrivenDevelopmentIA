# Diagnóstico Arquitectónico
El proyecto está construido como una API ASP.NET Core `.NET 8` con una arquitectura por capas simple: `Controller -> Service -> Data Access`, usando EF Core y Swagger ([Program.cs:4](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Program.cs:4), [ApiTaller1.csproj:10](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\ApiTaller1.csproj:10)).

## Patrones identificados
1. Patrón por capas (presentación, servicio, acceso a datos):  
[OrdenReciboController.cs:9](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Controllers\OrdenReciboController.cs:9), [ServicesOrdenRecibo.cs:8](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Service\ServicesOrdenRecibo.cs:8), [DataRepositoryOrdenRecibo.cs:8](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\DA\DataRepositoryOrdenRecibo.cs:8).
2. Uso parcial de DI y DIP: el controlador depende de interfaz (`IOrdenRecibo`) correctamente ([OrdenReciboController.cs:11](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Controllers\OrdenReciboController.cs:11)).
3. Patrón Repository “manual” en `DA`, pero sin interfaz de repositorio y sin inyección formal.

## Brechas frente a estándares
1. **Ruptura de inversión de dependencias en Service**: instancia directa de repositorio con `new` ([ServicesOrdenRecibo.cs:10](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Service\ServicesOrdenRecibo.cs:10)). Esto reduce testabilidad y flexibilidad.
2. **Lifetime no ideal**: `IOrdenRecibo` está como `Singleton` ([Program.cs:9](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Program.cs:9)); para lógica de negocio + acceso a datos normalmente se usa `Scoped`.
3. **Sin asincronía**: consultas y endpoints síncronos (`ToList`, `FirstOrDefault`) ([DataRepositoryOrdenRecibo.cs:14](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\DA\DataRepositoryOrdenRecibo.cs:14)).
4. **REST no homogéneo**: rutas con verbos/nombres de operación (`ORProductosRevisadosSP`) en lugar de recursos consistentes ([OrdenReciboController.cs:44](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Controllers\OrdenReciboController.cs:44)).
5. **Higiene arquitectónica**: queda controlador plantilla (`WeatherForecastController`) ajeno al dominio ([WeatherForecastController.cs:7](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\Controllers\WeatherForecastController.cs:7)).
6. **Acoplamiento fuerte a DLL externa** `ormdb.dll` ([ApiTaller1.csproj:16](D:\vidapogosoft\cursos\2026\sinergiass\automotors\vpr\ApiTaller1\ApiTaller1.csproj:16)).
7. **No hay proyecto de pruebas** en el repositorio actual.

## Nivel de madurez
Arquitectura funcional para MVP interno, pero aún lejos de estándares enterprise (Clean Architecture, testabilidad alta, contratos REST consistentes, observabilidad y manejo de errores transversal).
