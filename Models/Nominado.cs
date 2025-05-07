using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Nominado
{
    public string nombre
    {
        get; set;
    }
    public string trabajo
    {
        get; set;
    }
    public string? recoge
    {
        get; set;
    }
    public bool ganador
    {
        get; set;
    }

    public Nominado()
    {
        nombre = "";
        trabajo = "";
        recoge = "";
        ganador = false;
    }

    public Nominado(string nombre, string trabajo, string? recoge)
    {
        this.nombre = nombre;
        this.trabajo = trabajo;
        this.recoge = recoge;
    }
}
