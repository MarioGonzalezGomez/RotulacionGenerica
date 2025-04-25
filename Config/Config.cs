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
    public string TipoRodillo
    {
        get; set;
    }
    public int VelocidadRodillo
    {
        get; set;
    }
    public string RutaPremios
    {
        get; set;
    }
    public bool BtnCategoria
    {
        get; set;
    }
    public bool BtnNominado
    {
        get; set;
    }
    public bool BtnNominados
    {
        get; set;
    }
    public bool BtnGafas
    {
        get; set;
    }
    public bool BtnEntregadores
    {
        get; set;
    }
    public bool BtnGanador
    {
        get; set;
    }
}

public class PestanasActivas
{
    public bool Rotulos
    {
        get; set;
    }
    public bool Crawls
    {
        get; set;
    }
    public bool Creditos
    {
        get; set;
    }
    public bool Faldones
    {
        get; set;
    }
    public bool Premios
    {
        get; set;
    }
    public bool Gafas
    {
        get; set;
    }
    public bool Varios
    {
        get; set;
    }
    public bool Reset
    {
        get; set;
    }
    public bool Mosca
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
    public PestanasActivas PestanasActivas
    {
        get; set;
    }

    private static readonly string appDataPath = Path.Combine(
         Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
         "WinUI3", "ApplicationData");

    private static readonly string configFilePath = Path.Combine(appDataPath, "config.json");

    public Config()
    {

    }

    public static Config GetInstance()
    {
        if (instance == null)
        {
            instance = LoadConfig(configFilePath);
        }
        return instance;
    }

    //Comprobar si archivo config existe
    public static void ConfigExists()
    {
        if (!File.Exists(configFilePath))
        {
            string sourcePath = Path.Combine(AppContext.BaseDirectory, "Resources", "config.json");

            try
            {
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                File.Copy(sourcePath, configFilePath);
                Console.WriteLine("Archivo de configuración copiado a AppData.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al copiar el archivo de configuración: {ex.Message}");
            }
        }
    }

    // Método estático para cargar el JSON desde un archivo y deserializarlo
    public static Config LoadConfig(string filePath)
    {
        try
        {
            // Verifica si el archivo existe antes de intentar cargarlo
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Config>(jsonString);
            }
            else
            {
                Console.WriteLine("El archivo de configuración no existe.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar el archivo Config: {ex.Message}");
            return null;
        }
    }

    public static void SaveConfig(Config config)
    {
        try
        {
            // Verifica si la carpeta existe, si no, créala
            string directory = Path.GetDirectoryName(configFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, jsonString);
            Console.WriteLine("Archivo Config guardado exitosamente.");
            instance = config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el archivo Config: {ex.Message}");
        }
    }
}
