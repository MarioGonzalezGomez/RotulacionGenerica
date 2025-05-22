using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class SubtituladoController
{
    private static SubtituladoController instance;

    private SubtituladoController()
    {
    }

    public static SubtituladoController GetInstance()
    {
        if (instance == null)
        {
            instance = new SubtituladoController();
        }
        return instance;
    }


    public List<string> GetSubtitulos(string filePath)
    {
        var Subtitulado = new List<string>();
        try
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            foreach (var line in lines)
            {
                Subtitulado.Add(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al leer el archivo: {ex.Message}");
        }

        return Subtitulado;
    }
}
