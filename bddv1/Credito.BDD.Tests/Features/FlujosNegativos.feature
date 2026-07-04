# language: es
@allure.suite:Credito
@allure.subSuite:FlujosNegativos
Característica: Evaluación de Crédito - Flujos Negativos
  Como sistema de evaluación crediticia
  Quiero validar los criterios de flujos negativos
  Para garantizar una correcta asignación y seguridad del proceso.

  Escenario: Cliente menor de edad
    Dado que el cliente tiene una edad de 17 años
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Crédito rechazado"

  Escenario: Cliente con ingresos insuficientes
    Dado que el cliente tiene ingresos inferiores al umbral mínimo
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Crédito rechazado"

  Escenario: Cliente con historial crediticio negativo
    Dado que el cliente tiene un reporte financiero con morosidad activa
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Crédito rechazado"

  Escenario: Cliente con documentos incompletos
    Dado que el cliente inicia la solicitud con falta de identificación o comprobante
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Proceso detenido / error"

  Escenario: Cliente con inconsistencias en datos
    Dado que el nombre del cliente no coincide con los documentos oficiales
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Solicitud rechazada"
