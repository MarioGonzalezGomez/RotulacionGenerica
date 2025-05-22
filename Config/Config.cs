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
    public int ColumnasRodillo
    {
        get; set;
    }
    public int LineasPorBloque
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
    public string RutaSubtitulos
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
    public bool Subtitulado
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

    // Método para obtener la instancia y cargar el archivo de configuración
    public static Config GetInstance()
    {
        if (instance == null)
        {
            // Asegurarse de que el archivo de configuración exista antes de cargarlo
            ConfigExists();
            instance = LoadConfig(configFilePath);
        }
        return instance;
    }

    // Método para comprobar si el archivo de configuración existe, si no lo copia
    public static void ConfigExists()
    {
        try
        {
            if (!File.Exists(configFilePath))
            {
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                var defaultConfig = GetDefaultConfig();
                string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configFilePath, json);

                Console.WriteLine("Archivo de configuración por defecto creado en AppData.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear el archivo de configuración por defecto: {ex.Message}");
        }
    }

    public static Config GetDefaultConfig()
    {
        return new Config
        {
            ApiOptions = new ApiOptions
            {
                BaseUri = "localhost",
                Port = "5000"
            },
            BrainStormOptions = new BrainStormOptions
            {
                Ip = "127.0.0.1",
                Database = "dbs1",
                Port = "5123",
                Auto_Connection = "0"
            },
            PrimeOptions = new PrimeOptions
            {
                Ip = "127.0.0.1",
                Port = "8080",
                Auto_Connection = "0"
            },
            RotulacionSettings = new RotulacionSettings
            {
                VelocidadCrawl = 200,
                RutaRodillo = "",
                TipoRodillo = "Vertical",
                ColumnasRodillo = 2,
                LineasPorBloque = 4,
                VelocidadRodillo = 120,
                RutaPremios = "",
                BtnCategoria = true,
                BtnNominado = true,
                BtnNominados = true,
                BtnGafas = true,
                BtnEntregadores = true,
                BtnGanador = true,
                RutaSubtitulos = ""
            },
            PestanasActivas = new PestanasActivas
            {
                Rotulos = true,
                Crawls = true,
                Creditos = true,
                Faldones = true,
                Premios = true,
                Subtitulado = true,
                Gafas = true,
                Varios = true,
                Reset = true,
                Mosca = true
            }
        };
    }

    // Método estático para cargar la configuración desde el archivo JSON
    public static Config LoadConfig(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                // Leer el archivo y deserializar el JSON
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Config>(jsonString);
            }
            else
            {
                Console.WriteLine("El archivo de configuración no existe en la ruta esperada.");
                return null;  // Retornar null si no existe el archivo
            }
        }
        catch (Exception ex)
        {
            // Capturar cualquier error durante la carga del archivo
            Console.WriteLine($"Error al cargar el archivo Config: {ex.Message}");
            return null;
        }
    }

    // Método para guardar la configuración actual en el archivo
    public static void SaveConfig(Config config)
    {
        try
        {
            // Verificar si la carpeta de destino existe, si no, crearla
            string directory = Path.GetDirectoryName(configFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Serializar el objeto y guardarlo en el archivo
            string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, jsonString);
            Console.WriteLine("Archivo Config guardado exitosamente.");
            instance = config;
        }
        catch (Exception ex)
        {
            // Manejo de excepciones en caso de fallo al guardar el archivo
            Console.WriteLine($"Error al guardar el archivo Config: {ex.Message}");
        }
    }
}
