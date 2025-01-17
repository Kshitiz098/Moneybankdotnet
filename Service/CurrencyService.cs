using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Service
{
    internal class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "fadee6e8c62a471fa69d989026febd45"; // Replace with your actual API key

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // This method fetches the conversion rate for a specific currency pair (e.g., USD to EUR)
        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                // Build the API URL with your API key and the chosen currencies
                var apiUrl = $"https://v6.exchangerate-api.com/v6/{ApiKey}/latest/{fromCurrency}";

                // Make the API request
                var responseMessage = await _httpClient.GetAsync(apiUrl);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: API request failed with status code {responseMessage.StatusCode}");
                    return 1;
                }

                var response = await responseMessage.Content.ReadFromJsonAsync<CurrencyApiResponse>();

                // Check if the response is null or doesn't contain the conversion rates
                if (response == null)
                {
                    Console.WriteLine("Error: API response is null.");
                    return 1;
                }

                if (response.ConversionRates == null || !response.ConversionRates.ContainsKey(toCurrency))
                {
                    Console.WriteLine($"Error: No conversion rate found for {toCurrency}.");
                    return 1;
                }

                // Return the conversion rate for the specified currency
                return response.ConversionRates[toCurrency];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exchange rate: {ex.Message}");
            }

            return 1; // Return 1 if unable to fetch the conversion rate (no conversion)
        }
    }
    public class CurrencyApiResponse
    {
        public Dictionary<string, decimal> ConversionRates { get; set; }
    }
}
