# Plan de Pruebas Detallado v1 - Flujos de Negocio OrdenRecibo

## Resumen
- Objetivo: cubrir de extremo a extremo los flujos de negocio actuales de `OrdenRecibo` con ejecucion manual trazable y diseno listo para automatizacion.
- Cobertura confirmada: `GET /ORRevisadas`, `GET /ByNumero/{numrecibo}`, `GET /ORProductosRevisados`, `GET /ORProductosRevisados/{ordenrecibo}/{idorden}`, `GET /ORProductosRevisadosSP`, `GET /ORProductosRevisadosSP/{NumRecibo}`.
- Criterio de exito: 100% de casos ejecutados y evidenciados, sin respuestas `5xx`, y contrato JSON consistente por endpoint.

## Cambios/Interfaces relevantes
- APIs publicas: sin cambios de rutas, payloads ni codigos esperados de baseline.
- Tipos/interfaces publicas: sin cambios en `IOrdenRecibo` ni DTOs de respuesta.
- Activos de prueba a usar como fuente de verdad:
  - `testing/OrdenRecibo.manual.http` para ejecucion manual.
  - `testing/ordenrecibo_expected_results.json` para resultados esperados y trazabilidad.
- Base de ejecucion: entorno `Testing` (dataset deterministico), evitando dependencia de BD real.

## Plan de pruebas (manual, preparado para automatizar)
1. Preparacion
- Levantar API en `Testing` con perfil `testing` (puerto `5063`).
- Verificar salud inicial con `GET /api/OrdenRecibo/ORRevisadas`.

2. Matriz funcional por flujo
- OR-001: `ORRevisadas` devuelve `200` y arreglo no vacio.
- OR-002: validar contrato minimo por item (`idOrdenRecibo`, `numOrdenRecibo`, `rucProveedor`, `proveedor`).
- OR-010: `ByNumero/OR-1001` devuelve `200` con objeto esperado.
- OR-011: `ByNumero/OR-9999` devuelve `200` con `null` (baseline actual).
- OR-012: `ByNumero/or-1001` verifica comportamiento case-insensitive.
- OR-020: `ORProductosRevisados` devuelve `200` y arreglo no vacio.
- OR-021: validar consistencia de campos clave de producto (`idOR`, `numOrdenRecibo`, `skuProducto`).
- OR-030: `ORProductosRevisados/OR-1001/1` devuelve solo registros filtrados.
- OR-031: `ORProductosRevisados/OR-1001/999` devuelve `200` y `[]`.
- OR-032: `ORProductosRevisados/OR-1001/abc` devuelve `400` con `ProblemDetails`.
- OR-040: `ORProductosRevisadosSP` devuelve `200` y arreglo no vacio.
- OR-041: `ORProductosRevisadosSP/OR-1001` devuelve `200` con match.
- OR-042: `ORProductosRevisadosSP/OR-9999` devuelve `200` y `[]`.
- OR-043: `ORProductosRevisadosSP/' OR 1=1` debe responder controlado (`2xx/4xx`) y nunca `5xx`.
- OR-044: `ORProductosRevisadosSP/or-1001` verifica case-insensitive.

3. Pruebas transversales
- Validar `Content-Type: application/json` en respuestas `200`.
- Ejecutar cada caso 3 veces para detectar inestabilidad.
- Medir tiempo de respuesta y registrar outliers (>1s en local) como hallazgo no bloqueante.

4. Evidencia y criterio de aprobacion
- Registrar por caso: request, status, body, timestamp, resultado `PASS/FAIL`.
- Aprobacion: todos los casos criticos `PASS`, cero `5xx`, y coherencia con `ordenrecibo_expected_results.json`.

5. Conversion directa a automatizacion
- Crear suite de integracion (xUnit + `WebApplicationFactory`) leyendo casos desde `ordenrecibo_expected_results.json`.
- Mantener los mismos IDs de caso (`OR-001...`) para trazabilidad manual/automatica.
- Criterio CI: `dotnet test` en verde y cobertura de 100% de endpoints de negocio listados.

## Supuestos y defaults
- Se excluyen endpoints no de negocio (`WeatherForecast`, `POST/PUT/DELETE` placeholder).
- Se valida comportamiento actual (por ejemplo, no encontrado en algunos flujos devuelve `200` con `null`/`[]`).
- No se usa BD real en esta version del plan; el dataset del entorno `Testing` es la referencia.
