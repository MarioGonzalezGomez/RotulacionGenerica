using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class RodilloController
{
    private static RodilloController instance;

    private RodilloController()
    {
    }

    public static RodilloController GetInstance()
    {
        if (instance == null)
        {
            instance = new RodilloController();
        }
        return instance;
    }


    public Rodillo GetRodillo(string filePath)
    {
        var rodillo = new Rodillo();
        try
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            Cargo currentCargo = null;
            int personaOrden = 0;
            bool siguienteEsCargo = true;

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    siguienteEsCargo = true;
                    continue;
                }

                if (siguienteEsCargo) // Es un encabezado de cargo
                {
                    currentCargo = new Cargo { nombre = line };
                    rodillo.cargos.Add(currentCargo);
                    personaOrden = 0; // Reiniciar el orden para cada cargo
                    siguienteEsCargo = false;
                }
                else if (currentCargo != null) // Es un nombre de persona
                {
                    currentCargo.personas.Add(new Persona
                    {
                        nombre = line.Trim(),
                        orden = ++personaOrden
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }

        return rodillo;
    }


    public void SaveRodillo(string filePath, Rodillo rodillo)
    {
        try
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8)) // Forzar UTF-8
            {
                foreach (var cargo in rodillo.cargos)
                {
                    writer.WriteLine(cargo.nombre); // Escribir el nombre del cargo
                    foreach (var persona in cargo.personas)
                    {
                        writer.WriteLine($" {persona.nombre}"); // Indentar los nombres de personas
                    }
                    writer.WriteLine(); // Separador entre cargos
                }
            }

            Console.WriteLine("Archivo guardado exitosamente en UTF-8.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
        }
    }
}
