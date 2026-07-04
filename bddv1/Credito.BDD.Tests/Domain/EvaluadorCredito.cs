using System.Text.RegularExpressions;

namespace Credito.BDD.Tests.Domain;

public sealed class EvaluadorCredito
{
    public string Evaluar(SolicitudCredito solicitud)
    {
        if (solicitud.FuerzaBrutaLogin)
            return "Cuenta bloqueada / alerta de seguridad";

        if (solicitud.SinRolAnalista)
            return "Acceso denegado";

        if (EsInyeccionSql(solicitud.ValorMaliciosoFormulario))
            return "Sistema bloquea entrada / error controlado";

        if (solicitud.PayloadAlterado)
            return "Datos rechazados / alerta";

        if (solicitud.ManipulacionScoreFrontend)
            return "Validación en backend evita fraude";

        if (solicitud.DocumentosIncompletos)
            return "Proceso detenido / error";

        if (solicitud.NombreNoCoincideDocumentos)
            return "Solicitud rechazada";

        if (solicitud.Edad < 18)
            return "Crédito rechazado";

        if (solicitud.Ingresos < SolicitudCredito.UmbralIngresoMinimo)
            return "Crédito rechazado";

        if (solicitud.MorosidadActiva)
            return "Crédito rechazado";

        if (solicitud.HistorialVacio)
            return "Evaluación especial / crédito limitado";

        if (solicitud.DeudaActual == SolicitudCredito.LimiteDeudaPermitido)
            return "Crédito aprobado con condiciones";

        if (solicitud.Ingresos == SolicitudCredito.UmbralIngresoMinimo)
            return "Crédito aprobado si no hay otras restricciones";

        if (solicitud.Edad is >= 18 and <= 70)
            return "Crédito asignado si cumple demás criterios";

        return "Crédito rechazado";
    }

    private static bool EsInyeccionSql(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return false;

        return Regex.IsMatch(valor, @"('|--|;|\bOR\b|\bSELECT\b|\bDROP\b)", RegexOptions.IgnoreCase);
    }
}
