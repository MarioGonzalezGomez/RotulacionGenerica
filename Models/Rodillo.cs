using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Rodillo
{
    public int id
    {
        get; set;
    }
    public int velocidad
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

   // public override string? ToString() => linea.texto;
}
