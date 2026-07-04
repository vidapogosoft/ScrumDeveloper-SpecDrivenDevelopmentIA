using Credito.BDD.Tests.Domain;
using Reqnroll;
using Xunit;

namespace Credito.BDD.Tests.Steps;

[Binding]
public sealed class EvaluacionCreditoSteps
{
    private readonly SolicitudCredito _solicitud = new();
    private readonly EvaluadorCredito _evaluador = new();
    private string _resultado = string.Empty;

    [Given(@"que el cliente tiene una edad de (\d+) años")]
    public void DadoQueElClienteTieneUnaEdadDeAnos(int edad)
    {
        _solicitud.Edad = edad;
    }

    [Given(@"que el cliente tiene ingresos exactamente en el umbral mínimo")]
    public void DadoQueElClienteTieneIngresosExactamenteEnElUmbralMinimo()
    {
        _solicitud.Ingresos = SolicitudCredito.UmbralIngresoMinimo;
    }

    [Given(@"que el cliente no cuenta con registros previos en su historial crediticio")]
    public void DadoQueElClienteNoCuentaConRegistrosPreviosEnSuHistorialCrediticio()
    {
        _solicitud.HistorialVacio = true;
    }

    [Given(@"que el cliente posee una deuda exactamente igual al límite permitido")]
    public void DadoQueElClientePoseeUnaDeudaExactamenteIgualAlLimitePermitido()
    {
        _solicitud.DeudaActual = SolicitudCredito.LimiteDeudaPermitido;
    }

    [Given(@"que el cliente tiene ingresos inferiores al umbral mínimo")]
    public void DadoQueElClienteTieneIngresosInferioresAlUmbralMinimo()
    {
        _solicitud.Ingresos = SolicitudCredito.UmbralIngresoMinimo - 1;
    }

    [Given(@"que el cliente tiene un reporte financiero con morosidad activa")]
    public void DadoQueElClienteTieneUnReporteFinancieroConMorosidadActiva()
    {
        _solicitud.MorosidadActiva = true;
    }

    [Given(@"que el cliente inicia la solicitud con falta de identificación o comprobante")]
    public void DadoQueElClienteIniciaLaSolicitudConFaltaDeIdentificacionOComprobante()
    {
        _solicitud.DocumentosIncompletos = true;
    }

    [Given(@"que el nombre del cliente no coincide con los documentos oficiales")]
    public void DadoQueElNombreDelClienteNoCoincideConLosDocumentosOficiales()
    {
        _solicitud.NombreNoCoincideDocumentos = true;
    }

    [Given(@"que un usuario ingresa el valor malicioso ""(.*)"" en el formulario de solicitud")]
    public void DadoQueUnUsuarioIngresaElValorMaliciosoEnElFormularioDeSolicitud(string valor)
    {
        _solicitud.ValorMaliciosoFormulario = valor;
    }

    [Given(@"que el usuario intenta acceder al módulo de asignación sin el rol de analista")]
    public void DadoQueElUsuarioIntentaAccederAlModuloDeAsignacionSinElRolDeAnalista()
    {
        _solicitud.SinRolAnalista = true;
    }

    [Given(@"que se altera el payload de los datos de la solicitud en tránsito")]
    public void DadoQueSeAlteraElPayloadDeLosDatosDeLaSolicitudEnTransito()
    {
        _solicitud.PayloadAlterado = true;
    }

    [Given(@"que un cliente intenta modificar su score mediante herramientas del front-end")]
    public void DadoQueUnClienteIntentaModificarSuScoreMedianteHerramientasDelFrontEnd()
    {
        _solicitud.ManipulacionScoreFrontend = true;
    }

    [Given(@"que se realizan múltiples intentos fallidos de inicio de sesión en el sistema")]
    public void DadoQueSeRealizanMultiplesIntentosFallidosDeInicioDeSesionEnElSistema()
    {
        _solicitud.FuerzaBrutaLogin = true;
    }

    [When(@"el sistema procesa la solicitud de evaluación de crédito")]
    public void CuandoElSistemaProcesaLaSolicitudDeEvaluacionDeCredito()
    {
        _resultado = _evaluador.Evaluar(_solicitud);
    }

    [Then(@"el resultado debe ser: ""(.*)""")]
    public void EntoncesElResultadoDebeSer(string esperado)
    {
        Assert.Equal(esperado, _resultado);
    }
}
