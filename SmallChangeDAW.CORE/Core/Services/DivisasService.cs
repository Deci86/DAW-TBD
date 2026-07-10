using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory; // 1. NUEVO IMPORT PARA EL CACHÉ

namespace SmallChangeDAW.CORE.Core.Services;

public class DivisasService : IDivisasService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly string _apiBaseUrl;
    private readonly IMemoryCache _cache; // 2. DECLARAMOS EL CACHÉ

    // 3. INYECTAMOS EL CACHÉ EN EL CONSTRUCTOR
    public DivisasService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _apiBaseUrl = configuration["UnirateApi:ApiUrl"];
        _cache = cache; // Asignamos la variable
    }

    public async Task<TipoCambioResponseDTO> ObtenerTipoCambioAsync(string monedaIn, string monedaOut)
    {
        // 4. Creamos una llave única, ej: "Tasa_USD_EUR"
        string cacheKey = $"Tasa_{monedaIn.ToUpper()}_{monedaOut.ToUpper()}";

        // 5. Envolvemos tu lógica original en GetOrCreateAsync
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            // Configuramos el caché para que este dato viva por 10 minutos
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            try
            {
                var apiKey = _configuration["UnirateApi:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                    throw new InvalidOperationException("API key de UniRate no está configurada.");

                var client = _httpClientFactory.CreateClient();

                var url = $"{_apiBaseUrl}convert?api_key={apiKey}&from={monedaIn.ToUpper()}&to={monedaOut.ToUpper()}&amount=1";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error al consultar la API de divisas: {response.StatusCode}. URL solicitada: {url}");

                var content = await response.Content.ReadAsStringAsync();

                var jsonResponse = JsonSerializer.Deserialize<UniRateConvertResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (jsonResponse == null || jsonResponse.Result <= 0)
                    throw new InvalidOperationException($"La API no devolvió un resultado válido para la conversión de {monedaIn} a {monedaOut}.");

                var tipoCambio = jsonResponse.Result;

                return new TipoCambioResponseDTO
                {
                    MonedaIn = monedaIn.ToUpper(),
                    MonedaOut = monedaOut.ToUpper(),
                    TipoCambio = tipoCambio,
                    FechaActualizacion = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                // Si ocurre un error, .NET es inteligente y NO guardará el error en caché
                throw new InvalidOperationException($"Error al obtener el tipo de cambio: {ex.Message}", ex);
            }
        });
    }

    public async Task<CambioMonedaResponseDTO> ConvertirMonedaAsync(string monedaIn, string monedaOut, decimal monto)
    {
        // Como este método usa ObtenerTipoCambioAsync, ¡automáticamente hereda el caché! No hay que tocar nada aquí.
        var tipoCambio = await ObtenerTipoCambioAsync(monedaIn, monedaOut);
        var montoConvertido = monto * tipoCambio.TipoCambio;

        return new CambioMonedaResponseDTO
        {
            MonedaIn = tipoCambio.MonedaIn,
            MonedaOut = tipoCambio.MonedaOut,
            TipoCambio = tipoCambio.TipoCambio,
            Monto = monto,
            MontoConvertido = decimal.Round(montoConvertido, 2),
            FechaActualizacion = tipoCambio.FechaActualizacion
        };
    }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }

    private class UniRateConvertResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; } = string.Empty;

        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty;

        [JsonPropertyName("result")]
        public decimal Result { get; set; }
    }

    public async Task<Dictionary<string, string>> ObtenerMonedasDisponiblesAsync()
    {
        // Llave fija porque la lista es global
        string cacheKey = "ListaMonedasDisponibles";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            // Las monedas no cambian seguido. Las guardamos por 12 horas.
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

            try
            {
                var apiKey = _configuration["UnirateApi:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                    throw new InvalidOperationException("API key de UniRate no está configurada.");

                var client = _httpClientFactory.CreateClient();
                var url = $"{_apiBaseUrl}currencies?api_key={apiKey}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error al consultar la API de divisas: {response.StatusCode}. URL solicitada: {url}");

                var content = await response.Content.ReadAsStringAsync();

                var jsonResponse = JsonSerializer.Deserialize<UniRateCurrenciesResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var resultado = new Dictionary<string, string>();

                if (jsonResponse?.Currencies != null)
                {
                    foreach (var codigo in jsonResponse.Currencies)
                    {
                        if (!string.IsNullOrWhiteSpace(codigo))
                        {
                            resultado[codigo.ToUpper()] = codigo.ToUpper();
                        }
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener u organizar la lista de monedas: {ex.Message}", ex);
            }
        });
    }

    private class UniRateCurrenciesResponse
    {
        [JsonPropertyName("currencies")]
        public List<string> Currencies { get; set; } = new();

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}