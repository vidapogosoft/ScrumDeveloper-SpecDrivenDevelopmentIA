# Lista de Casos de Prueba v1

| Categoría | Caso de Prueba | Datos de Entrada / Condición | Resultado Esperado | controller | ruta para probar en postman |
|---|---|---|---|---|---|
| Órdenes revisadas | OR-001 Listar órdenes revisadas | `GET` sin parámetros | `200 OK`, arreglo no vacío | `OrdenReciboController` | `GET /api/OrdenRecibo/ORRevisadas` |
| Órdenes revisadas | OR-002 Validar contrato de ORRevisadas | Ejecutar OR-001 | Cada item contiene al menos `idOrdenRecibo`, `numOrdenRecibo`, `rucProveedor`, `proveedor` | `OrdenReciboController` | `GET /api/OrdenRecibo/ORRevisadas` |
| Órdenes revisadas | OR-010 Consultar orden por número existente | `numrecibo=OR-1001` | `200 OK`, objeto con `numOrdenRecibo=OR-1001` | `OrdenReciboController` | `GET /api/OrdenRecibo/ByNumero/OR-1001` |
| Órdenes revisadas | OR-011 Consultar orden por número inexistente | `numrecibo=OR-9999` | `200 OK` con `null` (comportamiento actual) | `OrdenReciboController` | `GET /api/OrdenRecibo/ByNumero/OR-9999` |
| Órdenes revisadas | OR-012 Búsqueda por número case-insensitive | `numrecibo=or-1001` | `200 OK`, retorna la orden `OR-1001` | `OrdenReciboController` | `GET /api/OrdenRecibo/ByNumero/or-1001` |
| Productos revisados | OR-020 Listar productos revisados | `GET` sin parámetros | `200 OK`, arreglo no vacío | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisados` |
| Productos revisados | OR-021 Validar contrato de productos | Ejecutar OR-020 | Cada item contiene al menos `idOR`, `numOrdenRecibo`, `skuProducto` | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisados` |
| Productos revisados | OR-030 Filtrar productos por orden e id válidos | `ordenrecibo=OR-1001`, `idorden=1` | `200 OK`, solo registros que cumplan ambos filtros | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisados/OR-1001/1` |
| Productos revisados | OR-031 Filtrar productos sin coincidencias | `ordenrecibo=OR-1001`, `idorden=999` | `200 OK`, arreglo vacío `[]` | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisados/OR-1001/999` |
| Productos revisados | OR-032 Validar error por `idorden` no numérico | `idorden=abc` | `400 Bad Request` con `ProblemDetails` | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisados/OR-1001/abc` |
| Productos SP | OR-040 Listar productos SP | `GET` sin parámetros | `200 OK`, arreglo no vacío | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisadosSP` |
| Productos SP | OR-041 Consultar productos SP por número existente | `NumRecibo=OR-1001` | `200 OK`, arreglo no vacío | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisadosSP/OR-1001` |
| Productos SP | OR-042 Consultar productos SP por número inexistente | `NumRecibo=OR-9999` | `200 OK`, arreglo vacío `[]` | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisadosSP/OR-9999` |
| Productos SP | OR-043 Probar entrada con caracteres especiales | `NumRecibo=' OR 1=1` | Respuesta controlada (`2xx`/`4xx`), nunca `5xx` | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisadosSP/' OR 1=1` |
| Productos SP | OR-044 Búsqueda SP case-insensitive | `NumRecibo=or-1001` | `200 OK`, devuelve datos de `OR-1001` | `OrdenReciboController` | `GET /api/OrdenRecibo/ORProductosRevisadosSP/or-1001` |
| Métodos placeholder | OR-060 POST con body válido | Body: `"texto de prueba"` | `204 No Content` (método vacío actual) | `OrdenReciboController` | `POST /api/OrdenRecibo` |
| Métodos placeholder | OR-061 POST sin body | Sin body | `400 Bad Request` por validación de body | `OrdenReciboController` | `POST /api/OrdenRecibo` |
| Métodos placeholder | OR-070 PUT con id válido | `id=1`, body `"actualizacion de prueba"` | `204 No Content` (método vacío actual) | `OrdenReciboController` | `PUT /api/OrdenRecibo/1` |
| Métodos placeholder | OR-071 PUT con id inválido | `id=abc` | `400 Bad Request` por error de binding | `OrdenReciboController` | `PUT /api/OrdenRecibo/abc` |
| Métodos placeholder | OR-080 DELETE con id válido | `id=1` | `204 No Content` (método vacío actual) | `OrdenReciboController` | `DELETE /api/OrdenRecibo/1` |
| Métodos placeholder | OR-081 DELETE con id inválido | `id=abc` | `400 Bad Request` por error de binding | `OrdenReciboController` | `DELETE /api/OrdenRecibo/abc` |
| Controlador plantilla | WF-001 Endpoint template activo | `GET` sin parámetros | `200 OK`, arreglo de 5 registros | `WeatherForecastController` | `GET /WeatherForecast` |
| Transversal funcional | TR-001 Content-Type correcto | Ejecutar endpoints `GET` de negocio | Respuestas `200` con `application/json` | `OrdenReciboController` | Cualquiera de rutas `GET` de `OrdenRecibo` |
| Transversal funcional | TR-002 Estabilidad de respuesta | Repetir cada caso `GET` 3 veces | Sin `5xx`, respuestas consistentes | `OrdenReciboController` | Todas las rutas `GET` de `OrdenRecibo` |

Base URL recomendada para Postman:
- `Testing`: `http://localhost:5063`
- `Development`: `http://localhost:5062`
