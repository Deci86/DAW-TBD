using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace SmallChangeDAW.CORE.Core.Services;

public class DivisasService : IDivisasService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly string _apiBaseUrl = "https://openexchangerates.org/api";

    public DivisasService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<TipoCambioResponseDTO> ObtenerTipoCambioAsync(string monedaIn, string monedaOut)
    {
        try
        {
            var apiKey = _configuration["OpenExchangeRates:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("API key de Open Exchange Rates no está configurada.");

            var client = _httpClientFactory.CreateClient();
            var url = $"{_apiBaseUrl}/latest.json?app_id={apiKey}&base={monedaIn.ToUpper()}&symbols={monedaOut.ToUpper()}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error al consultar la API de divisas: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<OpenExchangeRatesResponse>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (jsonResponse?.Rates == null || !jsonResponse.Rates.ContainsKey(monedaOut.ToUpper()))
                throw new KeyNotFoundException($"No se encontró el tipo de cambio para {monedaOut}");

            var tipoCambio = jsonResponse.Rates[monedaOut.ToUpper()];

            return new TipoCambioResponseDTO
            {
                MonedaIn = monedaIn.ToUpper(),
                MonedaOut = monedaOut.ToUpper(),
                TipoCambio = tipoCambio,
                FechaActualizacion = UnixTimeStampToDateTime(jsonResponse.Timestamp)
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

    private class OpenExchangeRatesResponse
    {
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new();

        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
    }
}
