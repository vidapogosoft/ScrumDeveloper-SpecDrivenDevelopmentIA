using ApiTaller1.Interface;
using ApiTaller1.Model.dto;
using ormdb.Models.Database;
using SpProductoDto = ormdb.Models.DTO.DtoORProductosRevisados;

namespace ApiTaller1.Service
{
    public sealed class FakeOrdenReciboService : IOrdenRecibo
    {
        private static readonly List<OrdenReciboRevisada> Ordenes =
        [
            new OrdenReciboRevisada
            {
                IdOrdenRecibo = 1,
                NumOrdenRecibo = "OR-1001",
                CodigoCedis = "CED-01",
                Cedis = "NORTE",
                RucProveedor = "0990001112001",
                Proveedor = "Proveedor Uno"
            },
            new OrdenReciboRevisada
            {
                IdOrdenRecibo = 2,
                NumOrdenRecibo = "OR-1002",
                CodigoCedis = "CED-02",
                Cedis = "SUR",
                RucProveedor = "0990001112002",
                Proveedor = "Proveedor Dos"
            }
        ];

        private static readonly List<DtoORProductosRevisados> Productos =
        [
            new DtoORProductosRevisados
            {
                IdOR = 1,
                NumOrdenRecibo = "OR-1001",
                CodigoCedis = "CED-01",
                Cedis = "NORTE",
                RucProveedor = "0990001112001",
                Proveedor = "Proveedor Uno",
                SkuProducto = "SKU-001",
                DescripcionProducto = "Filtro de aceite",
                Ubicacion = "A1",
                UxC = 12,
                CantidadCompra = 12,
                CantidadRevisada = 12
            },
            new DtoORProductosRevisados
            {
                IdOR = 1,
                NumOrdenRecibo = "OR-1001",
                CodigoCedis = "CED-01",
                Cedis = "NORTE",
                RucProveedor = "0990001112001",
                Proveedor = "Proveedor Uno",
                SkuProducto = "SKU-002",
                DescripcionProducto = "Bujia",
                Ubicacion = "A2",
                UxC = 24,
                CantidadCompra = 24,
                CantidadRevisada = 23
            },
            new DtoORProductosRevisados
            {
                IdOR = 2,
                NumOrdenRecibo = "OR-1002",
                CodigoCedis = "CED-02",
                Cedis = "SUR",
                RucProveedor = "0990001112002",
                Proveedor = "Proveedor Dos",
                SkuProducto = "SKU-003",
                DescripcionProducto = "Pastilla de freno",
                Ubicacion = "B1",
                UxC = 10,
                CantidadCompra = 10,
                CantidadRevisada = 10
            }
        ];

        private static readonly Dictionary<string, List<SpProductoDto>> ProductosSpByNumero =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["OR-1001"] =
                [
                    CreateSpItem(1, "OR-1001", "SKU-001", "Filtro de aceite"),
                    CreateSpItem(1, "OR-1001", "SKU-002", "Bujia")
                ],
                ["OR-1002"] =
                [
                    CreateSpItem(2, "OR-1002", "SKU-003", "Pastilla de freno")
                ]
            };

        public IEnumerable<OrdenReciboRevisada> ConsultaOrdenesRevisadas => Ordenes;

        public IEnumerable<OrdenReciboRevisada> ConsultaOrdenRevisada(string ruc)
        {
            return Ordenes.Where(o => string.Equals(o.RucProveedor, ruc, StringComparison.OrdinalIgnoreCase));
        }

        public OrdenReciboRevisada? ConsultaOrdenRevisadaObject(string numorden)
        {
            return Ordenes.FirstOrDefault(o =>
                string.Equals(o.NumOrdenRecibo, numorden, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<DtoORProductosRevisados> ConsultaProductosRevisados => Productos;

        public IEnumerable<DtoORProductosRevisados> ConsultaProductosRevisadosv2(string ordenrecibo, int idorden)
        {
            return Productos.Where(p =>
                string.Equals(p.NumOrdenRecibo, ordenrecibo, StringComparison.OrdinalIgnoreCase) &&
                p.IdOR == idorden);
        }

        public IEnumerable<SpProductoDto> ConsultaProductosRevisadosSP =>
            ProductosSpByNumero.SelectMany(kv => kv.Value);

        public IEnumerable<SpProductoDto> ConsultaProductosRevisadosSPv2(string NumRecibo)
        {
            if (string.IsNullOrWhiteSpace(NumRecibo))
            {
                return Array.Empty<SpProductoDto>();
            }

            return ProductosSpByNumero.TryGetValue(NumRecibo, out var productos)
                ? productos
                : Array.Empty<SpProductoDto>();
        }

        private static SpProductoDto CreateSpItem(int idOrden, string numeroOrden, string sku, string descripcion)
        {
            var item = new SpProductoDto();
            SetIfExists(item, "IdOR", idOrden);
            SetIfExists(item, "NumOrdenRecibo", numeroOrden);
            SetIfExists(item, "SkuProducto", sku);
            SetIfExists(item, "DescripcionProducto", descripcion);
            return item;
        }

        private static void SetIfExists(SpProductoDto item, string propertyName, object? value)
        {
            var property = typeof(SpProductoDto).GetProperty(propertyName);
            if (property is null || !property.CanWrite)
            {
                return;
            }

            property.SetValue(item, value);
        }
    }
}
