using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Generico_Front.Models;

public class Rotulo
{
    public int id
    {
        get; set;
    }
    public int posicion
    {
        get; set;
    }
    public Tipo? tipo
    {
        get; set;
    }
    public List<Linea>? lineas
    {
        get; set;
    }

    public override string? ToString()
    {
        string text = "";
        for (int i = 1; i < lineas.Count; i++)
        {
            text += lineas[i].texto + "\n";
        }
        return text;
    }
}
