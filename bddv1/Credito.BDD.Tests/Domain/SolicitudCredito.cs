namespace Credito.BDD.Tests.Domain;

public sealed class SolicitudCredito
{
    public int Edad { get; set; } = 30;
    public decimal Ingresos { get; set; } = UmbralIngresoMinimo + 800m;
    public bool HistorialVacio { get; set; }
    public bool MorosidadActiva { get; set; }
    public bool DocumentosIncompletos { get; set; }
    public bool NombreNoCoincideDocumentos { get; set; }
    public bool SinRolAnalista { get; set; }
    public bool PayloadAlterado { get; set; }
    public bool ManipulacionScoreFrontend { get; set; }
    public bool FuerzaBrutaLogin { get; set; }
    public string? ValorMaliciosoFormulario { get; set; }
    public decimal DeudaActual { get; set; } = 0m;

    public const decimal UmbralIngresoMinimo = 1200m;
    public const decimal LimiteDeudaPermitido = 5000m;
}
