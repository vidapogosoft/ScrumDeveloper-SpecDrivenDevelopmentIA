# language: es
@allure.suite:Credito
@allure.subSuite:Seguridad
Característica: Evaluación de Crédito - Pruebas de Seguridad
  Como sistema de evaluación crediticia
  Quiero validar los criterios de pruebas de seguridad
  Para garantizar una correcta asignación y seguridad del proceso.

  Escenario: Intento de inyección SQL en formulario de solicitud
    Dado que un usuario ingresa el valor malicioso "' OR '1'='1" en el formulario de solicitud
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Sistema bloquea entrada / error controlado"

  Escenario: Acceso no autorizado a módulo de asignación
    Dado que el usuario intenta acceder al módulo de asignación sin el rol de analista
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Acceso denegado"

  Escenario: Manipulación de datos en tránsito
    Dado que se altera el payload de los datos de la solicitud en tránsito
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Datos rechazados / alerta"

  Escenario: Cliente intenta modificar su score manualmente
    Dado que un cliente intenta modificar su score mediante herramientas del front-end
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Validación en backend evita fraude"

  Escenario: Prueba de fuerza bruta en login del sistema
    Dado que se realizan múltiples intentos fallidos de inicio de sesión en el sistema
    Cuando el sistema procesa la solicitud de evaluación de crédito
    Entonces el resultado debe ser: "Cuenta bloqueada / alerta de seguridad"
