using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ApiTaller1.Tests.Integration;

public sealed class OrdenReciboBusinessFlowsTests : IClassFixture<TestingApiFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private static readonly IReadOnlyList<ApiCaseDefinition> Cases = ApiCaseCatalog.LoadBusinessCases();

    public OrdenReciboBusinessFlowsTests(TestingApiFactory factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    public static IEnumerable<object[]> CaseData()
    {
        return Cases.Select(testCase => new object[] { testCase });
    }

    [Theory]
    [MemberData(nameof(CaseData))]
    public async Task StatusCode_MatchesCatalog(ApiCaseDefinition testCase)
    {
        using var request = new HttpRequestMessage(testCase.HttpMethod, testCase.RelativePath);
        using var response = await _client.SendAsync(request);

        var status = (int)response.StatusCode;
        Assert.Contains(status, testCase.ExpectedStatusCodes);
    }

    [Theory]
    [MemberData(nameof(CaseData))]
    public async Task BodyContract_MatchesCatalog(ApiCaseDefinition testCase)
    {
        using var request = new HttpRequestMessage(testCase.HttpMethod, testCase.RelativePath);
        using var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        var status = (int)response.StatusCode;
        Assert.Contains(status, testCase.ExpectedStatusCodes);

        switch (testCase.ExpectedBody)
        {
            case "array_non_empty":
                AssertArrayNonEmpty(content);
                break;
            case "array_items_have_minimum_fields":
                AssertArrayItemsHaveProperties(content, "idOrdenRecibo", "numOrdenRecibo", "rucProveedor", "proveedor");
                break;
            case "object_with_NumOrdenRecibo_OR-1001":
                AssertObjectWithNumOrdenRecibo(content, "OR-1001");
                break;
            case "null":
                AssertJsonNull(content);
                break;
            case "null_or_empty":
                AssertNullOrEmptyBody(content);
                break;
            case "array_items_have_key_product_fields":
                AssertArrayItemsHaveProperties(content, "idOR", "numOrdenRecibo", "skuProducto");
                break;
            case "array_non_empty_filtered_by_numero_and_id":
                AssertFilteredProductos(content, "OR-1001", 1);
                break;
            case "empty_array":
                AssertEmptyArray(content);
                break;
            case "validation_problem_details":
                AssertValidationProblem(content, HttpStatusCode.BadRequest);
                break;
            case "controlled_response_no_5xx":
                Assert.True(status < 500, $"Caso {testCase.Id} devolvio {status}.");
                break;
            case "array_non_empty_case_insensitive":
                AssertArrayNonEmpty(content);
                AssertArrayItemsHaveFieldValue(content, "numOrdenRecibo", "OR-1001");
                break;
            default:
                throw new XunitException($"ExpectedBody no soportado: {testCase.ExpectedBody}");
        }
    }

    [Fact]
    public async Task BusinessFlows_AreStableAcrossThreeRuns()
    {
        foreach (var testCase in Cases)
        {
            for (var run = 1; run <= 3; run++)
            {
                using var request = new HttpRequestMessage(testCase.HttpMethod, testCase.RelativePath);
                using var response = await _client.SendAsync(request);
                var status = (int)response.StatusCode;

                Assert.True(status < 500, $"Caso {testCase.Id} devolvio {status} en corrida {run}.");
                Assert.Contains(status, testCase.ExpectedStatusCodes);
            }
        }
    }

    [Fact]
    public async Task SuccessfulResponses_AreJson()
    {
        foreach (var testCase in Cases)
        {
            using var request = new HttpRequestMessage(testCase.HttpMethod, testCase.RelativePath);
            using var response = await _client.SendAsync(request);
            var status = (int)response.StatusCode;

            if (status != 200)
            {
                continue;
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType;
            Assert.Equal("application/json", mediaType);
        }
    }

    [Fact]
    public async Task LogsLatencyOutliersOverOneSecond()
    {
        foreach (var testCase in Cases)
        {
            var stopwatch = Stopwatch.StartNew();
            using var request = new HttpRequestMessage(testCase.HttpMethod, testCase.RelativePath);
            using var response = await _client.SendAsync(request);
            stopwatch.Stop();

            Assert.True((int)response.StatusCode < 500, $"Caso {testCase.Id} devolvio {(int)response.StatusCode}.");

            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _output.WriteLine($"OUTLIER {testCase.Id}: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }

    private static void AssertArrayNonEmpty(string content)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.NotEmpty(doc.RootElement.EnumerateArray());
    }

    private static void AssertEmptyArray(string content)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Empty(doc.RootElement.EnumerateArray());
    }

    private static void AssertJsonNull(string content)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Null, doc.RootElement.ValueKind);
    }

    private static void AssertNullOrEmptyBody(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        AssertJsonNull(content);
    }

    private static void AssertObjectWithNumOrdenRecibo(string content, string expected)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);

        Assert.True(
            TryGetPropertyInsensitive(doc.RootElement, "numOrdenRecibo", out var numOrden),
            "No existe el campo numOrdenRecibo.");
        Assert.Equal(expected, numOrden.GetString());
    }

    private static void AssertFilteredProductos(string content, string numeroOrden, int idOrden)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);

        var items = doc.RootElement.EnumerateArray().ToArray();
        Assert.NotEmpty(items);

        foreach (var item in items)
        {
            Assert.True(TryGetPropertyInsensitive(item, "numOrdenRecibo", out var numOrden));
            Assert.True(TryGetPropertyInsensitive(item, "idOR", out var idOr));
            Assert.Equal(numeroOrden, numOrden.GetString());
            Assert.Equal(idOrden, idOr.GetInt32());
        }
    }

    private static void AssertArrayItemsHaveProperties(string content, params string[] propertyNames)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);

        var items = doc.RootElement.EnumerateArray().ToArray();
        Assert.NotEmpty(items);

        foreach (var item in items)
        {
            Assert.Equal(JsonValueKind.Object, item.ValueKind);

            foreach (var propertyName in propertyNames)
            {
                Assert.True(
                    TryGetPropertyInsensitive(item, propertyName, out _),
                    $"No existe la propiedad {propertyName} en un item del arreglo.");
            }
        }
    }

    private static void AssertArrayItemsHaveFieldValue(string content, string field, string expectedValue)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);

        var items = doc.RootElement.EnumerateArray().ToArray();
        Assert.NotEmpty(items);

        foreach (var item in items)
        {
            Assert.True(TryGetPropertyInsensitive(item, field, out var value));
            Assert.Equal(expectedValue, value.GetString());
        }
    }

    private static void AssertValidationProblem(string content, HttpStatusCode expectedStatus)
    {
        using var doc = JsonDocument.Parse(content);
        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);

        Assert.True(TryGetPropertyInsensitive(doc.RootElement, "title", out _));
        Assert.True(TryGetPropertyInsensitive(doc.RootElement, "status", out var statusElement));
        Assert.Equal((int)expectedStatus, statusElement.GetInt32());
    }

    private static bool TryGetPropertyInsensitive(JsonElement element, string propertyName, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}
