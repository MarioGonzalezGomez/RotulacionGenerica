using System;
using System.Collections.Generic;
using System.Linq;
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


    public Premio GetPremio(string filePath)
    {
        var premio = new Premio();
        try
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }

        return premio;
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
