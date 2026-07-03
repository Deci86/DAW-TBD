using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace SmallChangeDAW.CORE.Core.Services;

public class DivisasService : IDivisasService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly string _apiBaseUrl;

    public DivisasService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        // Asignamos el valor del appsettings.json o un fallback a la URL oficial
        _apiBaseUrl = configuration["UnirateApi:ApiUrl"];
    }

    public async Task<TipoCambioResponseDTO> ObtenerTipoCambioAsync(string monedaIn, string monedaOut)
    {
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

            // Validamos que el resultado exista y sea mayor a 0
            if (jsonResponse == null || jsonResponse.Result <= 0)
                throw new InvalidOperationException($"La API no devolvió un resultado válido para la conversión de {monedaIn} a {monedaOut}.");

            // Como le enviamos amount=1 en la URL, el "result" es exactamente nuestra tasa de cambio unitari   a
            var tipoCambio = jsonResponse.Result;

            return new TipoCambioResponseDTO
            {
                MonedaIn = monedaIn.ToUpper(),
                MonedaOut = monedaOut.ToUpper(),
                TipoCambio = tipoCambio,
                // Como este endpoint de UniRate no devuelve un "timestamp", registramos la hora del sistema
                FechaActualizacion = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al obtener el tipo de cambio: {ex.Message}", ex);
        }
    }

    public async Task<CambioMonedaResponseDTO> ConvertirMonedaAsync(string monedaIn, string monedaOut, decimal monto)
    {
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

    // Clase interna renombrada y adaptada a la estructura de UniRate
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
}