using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Generico_Front.Config;
public class ApiOptions
{
    public string BaseUri
    {
        get; set;
    }
    public string Port
    {
        get; set;
    }
}

public class BrainStormOptions
{
    public string Ip
    {
        get; set;
    }
    public string Database
    {
        get; set;
    }
    public string Port
    {
        get; set;
    }
    public string Auto_Connection
    {
        get; set;
    }
}

public class PrimeOptions
{
    public string Ip
    {
        get; set;
    }
    public string Port
    {
        get; set;
    }
    public string Auto_Connection
    {
        get; set;
    }
}

public class RotulacionSettings
{
    public int VelocidadCrawl
    {
        get; set;
    }
    public string RutaRodillo
    {
        get; set;
    }
    public int VelocidadRodillo
    {
        get; set;
    }
}

public class Config
{
    private static Config? instance = null;
    public ApiOptions ApiOptions
    {
        get; set;
    }
    public BrainStormOptions BrainStormOptions
    {
        get; set;
    }
    public PrimeOptions PrimeOptions
    {
        get; set;
    }
    public RotulacionSettings RotulacionSettings
    {
        get; set;
    }

    public Config()
    {

    }

    public static Config GetInstance()
    {
        if (instance == null)
        {
            string relativePath = Path.Combine(AppContext.BaseDirectory, "config.json");
            instance = LoadConfig(relativePath);
        }
        return instance;
    }

    // Método estático para cargar el JSON desde un archivo y deserializarlo
    public static Config LoadConfig(string filePath)
    {
        try
        {
            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Config>(jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar el archivo Config: {ex.Message}");
            return null;
        }
    }

    public static void SaveConfig(string filePath, Config config)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine("Archivo Config guardado exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el archivo Config: {ex.Message}");
        }
    }
}
