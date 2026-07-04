# language: es
@allure.suite:Credito
@allure.subSuite:CasosBorde
Característica: Evaluación de Crédito - Casos de Borde
  Como sistema de evaluación crediticia
  Quiero validar los criterios de casos de borde
  Para garantizar una correcta asignación y seguridad del proceso.

  Escenario: Cliente con edad mínima permitida (ej. 18 años)
    Dado que el cliente tiene una edad de 18 años
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Crédito asignado si cumple demás criterios"

  Escenario: Cliente con edad máxima permitida (ej. 70 años)
    Dado que el cliente tiene una edad de 70 años
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Crédito asignado si cumple demás criterios"

  Escenario: Ingreso justo en el umbral mínimo requerido
    Dado que el cliente tiene ingresos exactamente en el umbral mínimo
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Crédito aprobado si no hay otras restricciones"

  Escenario: Cliente con historial crediticio vacío
    Dado que el cliente no cuenta con registros previos en su historial crediticio
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Evaluación especial / crédito limitado"

  Escenario: Cliente con deuda exactamente igual al límite permitido
    Dado que el cliente posee una deuda exactamente igual al límite permitido
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Crédito aprobado con condiciones"
