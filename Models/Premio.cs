using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generico_Front.Models;
public class Premio
{
    public string nombre
    {
        get; set;
    }
    public List<Nominado> nominados
    {
        get; set;
    }
    public List<string> entregadores
    {
        get; set;
    }
    public Nominado? ganador
    {
        get; set;
    }

    public Premio()
    {
        nominados = new List<Nominado>();
        entregadores = new List<string>();
    }

    public Premio(string nombre, List<Nominado> nominados, List<string> entregadores, Nominado? ganador)
    {
        this.nombre = nombre;
        this.nominados = nominados;
        this.entregadores = entregadores;
        this.ganador = ganador;
    }
}
