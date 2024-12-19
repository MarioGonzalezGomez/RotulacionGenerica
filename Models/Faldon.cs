using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Faldon
{
    public int id
    {
        get; set;
    }
    public int posicion
    {
        get; set;
    }
    public Linea titulo
    {
        get; set;
    }
    public Linea texto
    {
        get; set;
    }
    public Tipo tipo
    {
        get; set;
    }
    public string ruta
    {
        get; set;
    }

    public override string? ToString()
    {
        string faldon;
        if (titulo != null && !string.IsNullOrEmpty(titulo.texto))
        {
            faldon = titulo.texto;
        }
        else
        {
            faldon = texto.texto;
        }
        return faldon;
    }
}
