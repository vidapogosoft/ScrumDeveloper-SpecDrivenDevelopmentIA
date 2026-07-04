using System.Net.Http;
using System.Text.Json;

namespace ApiTaller1.Tests.Integration;

public sealed record ApiCaseDefinition(
    string Id,
    string Endpoint,
    string ExpectedBody,
    IReadOnlyList<int> ExpectedStatusCodes)
{
    public string Method
    {
        get
        {
            var splitIndex = Endpoint.IndexOf(' ');
            return splitIndex > 0 ? Endpoint[..splitIndex] : Endpoint;
        }
    }

    public string RelativePath
    {
        get
        {
            var splitIndex = Endpoint.IndexOf(' ');
            var rawPath = splitIndex >= 0 ? Endpoint[(splitIndex + 1)..].Trim() : "/";
            return EncodePath(rawPath);
        }
    }

    public HttpMethod HttpMethod => new(Method);

    private static string EncodePath(string rawPath)
    {
        var segments = rawPath.Split('/', StringSplitOptions.None);
        for (var i = 0; i < segments.Length; i++)
        {
            if (string.IsNullOrEmpty(segments[i]))
            {
                continue;
            }

            segments[i] = Uri.EscapeDataString(Uri.UnescapeDataString(segments[i]));
        }

        return string.Join("/", segments);
    }
}

public static class ApiCaseCatalog
{
    private static readonly string[] RequiredBusinessIds =
    [
        "OR-001", "OR-002", "OR-010", "OR-011", "OR-012",
        "OR-020", "OR-021", "OR-030", "OR-031", "OR-032",
        "OR-040", "OR-041", "OR-042", "OR-043", "OR-044"
    ];

    public static IReadOnlyList<ApiCaseDefinition> LoadBusinessCases()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "testing", "ordenrecibo_expected_results.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "No se encontro el archivo de casos esperado en el output de pruebas.",
                path);
        }

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var allCases = document.RootElement.GetProperty("cases");

        var map = new Dictionary<string, ApiCaseDefinition>(StringComparer.OrdinalIgnoreCase);

        foreach (var caseElement in allCases.EnumerateArray())
        {
            var id = caseElement.GetProperty("id").GetString() ?? string.Empty;
            var endpoint = caseElement.GetProperty("endpoint").GetString() ?? string.Empty;
            var expectedBody = caseElement.GetProperty("expectedBody").GetString() ?? string.Empty;
            var expectedStatuses = ParseStatuses(caseElement.GetProperty("expectedStatus"));

            if (string.IsNullOrWhiteSpace(id))
            {
                continue;
            }

            map[id] = new ApiCaseDefinition(id, endpoint, expectedBody, expectedStatuses);
        }

        var missing = RequiredBusinessIds.Where(id => !map.ContainsKey(id)).ToArray();
        if (missing.Length > 0)
        {
            throw new InvalidOperationException(
                $"Faltan casos de negocio en el catalogo JSON: {string.Join(", ", missing)}");
        }

        return RequiredBusinessIds.Select(id => map[id]).ToArray();
    }

    private static IReadOnlyList<int> ParseStatuses(JsonElement statusElement)
    {
        if (statusElement.ValueKind == JsonValueKind.Number)
        {
            return [statusElement.GetInt32()];
        }

        if (statusElement.ValueKind == JsonValueKind.Array)
        {
            return statusElement.EnumerateArray()
                .Where(e => e.ValueKind == JsonValueKind.Number)
                .Select(e => e.GetInt32())
                .ToArray();
        }

        throw new InvalidOperationException("expectedStatus tiene un formato no soportado.");
    }
}
