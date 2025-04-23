using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class PremioController
{
    private static PremioController instance;

    private PremioController()
    {
    }

    public static PremioController GetInstance()
    {
        if (instance == null)
        {
            instance = new PremioController();
        }
        return instance;
    }


    public List<Premio> GetPremios(string filePath)
    {
        List<Premio> premios = new List<Premio>();
        try
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            Premio currentPremio = null;
            List<Nominado> currentNominados = null;
            bool siguienteEsPremio = true;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    siguienteEsPremio = true;
                    continue;
                }

                if (siguienteEsPremio) // Es un encabezado de cargo
                {
                    currentPremio = new Premio { nombre = line };
                    currentNominados = new List<Nominado>();
                    currentPremio.nominados = currentNominados;
                    premios.Add(currentPremio);
                    siguienteEsPremio = false;
                }
                else if (currentPremio != null) // Es un nombre de persona
                {
                    if (line.StartsWith("#"))
                    {
                        currentPremio.entregadores.Add(line.Substring(1));
                    }
                    else
                    {
                        bool esGanador = line.StartsWith("*");
                        string contenido = esGanador ? line.Substring(1) : line;
                        var partes = contenido.Split("/");
                        var nominado = new Nominado
                        {
                            nombre = partes.ElementAtOrDefault(0)?.Trim(),
                            trabajo = partes.ElementAtOrDefault(1)?.Trim(),
                            recoge = partes.ElementAtOrDefault(2)?.Trim()
                        };
                        if (esGanador)
                        {
                            currentPremio.ganador = nominado;
                            nominado.ganador = true;
                        }
                        currentPremio?.nominados.Add(nominado);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }

        return premios;
    }


    public void SavePremio(string filePath, Premio premio)
    {
        try
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8)) // Forzar UTF-8
            {
                // foreach (var cargo in premio.cargos)
                //{
                //    writer.WriteLine(cargo.nombre); // Escribir el nombre del cargo
                //    foreach (var persona in cargo.personas)
                //    {
                //        writer.WriteLine($" {persona.nombre}"); // Indentar los nombres de personas
                //    }
                //    writer.WriteLine(); // Separador entre cargos
                //}
            }

            Console.WriteLine("Archivo guardado exitosamente en UTF-8.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
        }
    }
}
