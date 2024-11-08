using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Generico_Front.Contracts.Services;
using Generico_Front.Services;
using Microsoft.Extensions.Configuration;
using Windows.UI.ApplicationSettings;

namespace Generico_Front.Controllers.Data;

class Cliente
{
    private static Cliente? cliente = null;
    private Config.Config config;
    private HttpClient _httpClient;

    private Cliente()
    {
        config = Config.Config.GetInstance();
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri($"https://{config.ApiOptions.BaseUri}:{config.ApiOptions.Port}");
        // Configuración de las cabeceras generales
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.Timeout = TimeSpan.FromSeconds(2);
    }

    public static Cliente GetInstance()
    {
        if (cliente == null)
        {
            cliente = new Cliente();
        }
        return cliente;
    }

    // Método para hacer una solicitud GET
    public async Task<string> GetAsync(string endpoint)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);
            response.EnsureSuccessStatusCode(); // Lanza una excepción si el código de respuesta no es exitoso (4xx, 5xx)
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false); // Retorna el contenido de la respuesta como string
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en GetAsync: {ex.Message}");
            return null;
        }
    }

    // Método para hacer una solicitud POST
    public async Task<string> PostAsync(string endpoint, string jsonContent)
    {
        try
        {
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en PostAsync: {ex.Message}");
            return null;
        }
    }

    // Método para hacer una solicitud PUT
    public async Task<string> PutAsync(string endpoint, string jsonContent)
    {
        try
        {
            HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en PutAsync: {ex.Message}");
            return null;
        }
    }

    // Método para hacer una solicitud DELETE
    public async Task<string> DeleteAsync(string endpoint)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
            return null;
        }
    }
}
