using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Crawl
{
    public int id
    {
        get; set;
    }
    public int posicion
    {
        get; set;
    }
    public Linea? linea
    {
        get; set;
    }
    public bool esTitulo
    {
        get; set;
    }
    public int velocidad
    {
        get; set;
    }

    public override string? ToString() => linea.texto;
}
